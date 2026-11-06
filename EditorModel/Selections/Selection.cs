using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using EditorModel.Common;
using EditorModel.Figures;
using EditorModel.Geometry;
using EditorModel.Style;
using EditorModel.Renderers;

namespace EditorModel.Selections
{
 
    [Serializable]
    public class Selection : Figure, IEnumerable<Figure>
    {
      
        private readonly HashSet<Figure> _selected = new HashSet<Figure>();
        private bool _isFrameVisible = true;
        public Figure CurrentFigure { get; private set; }

      
        public void Clear()
        {
            _selected.Clear();
            CurrentFigure = new Figure();
            GrabGeometry();
        }

      
        public void Add(Figure fig)
        {
            _selected.Add(fig);
            CurrentFigure = fig;
            GrabGeometry();
        }

       
        public void Remove(Figure fig)
        {
            _selected.Remove(fig);
            GrabGeometry();
        }

       
        public bool IsFrameVisible
        {
            private get { return _isFrameVisible; }
            set
            {
                if (_isFrameVisible == value) return;
                _isFrameVisible = value;
                GrabGeometry();
            }
        }

     
        public bool IsHit(Layer layer, Point point)
        {
            var found = false;
            using (var pen = new Pen(Color.Black, 5))
            {
               
                for (var i = layer.Figures.Count - 1; i >= 0; i--)
                {
                    var fig = layer.Figures[i];
                    var path = fig.GetTransformedPath();
                    
                    if (!path.Path.IsOutlineVisible(point, pen)) continue;
                    found = true;
                    break;
                }
            }
            return found;
        }

       
        public bool FindFigureAt(Layer layer, Point point, out Figure figure)
        {
            figure = null;
            var found = false;
            using (var pen = new Pen(Color.Black, 5))
            {
               
                for (var i = layer.Figures.Count - 1; i >= 0; i--)
                {
                    var fig = layer.Figures[i];
                    if (!layer.IsVisible(fig) || layer.IsLocked(fig)) continue;
                    var closedGeometry = fig.Geometry as IGeometry;
                    var isClosed = closedGeometry == null || 
                        closedGeometry != null && closedGeometry.IsClosed;
                    var passaggioGeometry = fig.Geometry as PolygoneGeometry;
                    if (passaggioGeometry != null &&
                        passaggioGeometry.BasketKind == BasketPolylineKind.PalleggioTratteggiato &&
                        IsNearPolyline(passaggioGeometry.GetTransformedPoints(fig), point, 12f))
                    {
                        figure = fig;
                        found = true;
                        break;
                    }
                    var path = fig.GetTransformedPath();
                    if (isClosed && path.Path.IsVisible(point))
                    {
                        figure = fig;
                        found = true;
                        break;
                    }
                   
                    if (!path.Path.IsOutlineVisible(point, pen)) continue;
                    figure = fig;
                    found = true;
                    break;
                }
            }
            return found;
        }

        private static bool IsNearPolyline(PointF[] points, Point point, float tolerance)
        {
            if (points == null || points.Length < 2) return false;
            float toleranceSquared = tolerance * tolerance;
            for (int i = 0; i < points.Length - 1; i++)
            {
                float dx = points[i + 1].X - points[i].X;
                float dy = points[i + 1].Y - points[i].Y;
                float lengthSquared = dx * dx + dy * dy;
                float t = lengthSquared <= 0.000001f
                    ? 0f
                    : ((point.X - points[i].X) * dx + (point.Y - points[i].Y) * dy) / lengthSquared;
                t = Math.Max(0f, Math.Min(1f, t));
                float nearestX = points[i].X + t * dx;
                float nearestY = points[i].Y + t * dy;
                float distanceX = point.X - nearestX;
                float distanceY = point.Y - nearestY;
                if (distanceX * distanceX + distanceY * distanceY <= toleranceSquared)
                    return true;
            }
            return false;
        }

       /// <summary>
       /// Aggiorna la geometria del modello (figura)
       /// </summary>
        private void GrabGeometry()
        {
           
            var path = new SerializableGraphicsPath();
            foreach (var fig in _selected)
            {
                path.Path.SetMarkers();
                path.Path.AddPath(fig.GetTransformedPath(), false);
            }

            
            if (IsFrameVisible)
            {
                var bounds = path.Path.GetBounds();
                path.Path.AddRectangle(bounds);
            }
            
            var allowedOperations = _selected.Count == 1
                ? _selected.First().Geometry.AllowedOperations
                : AllowedOperations.All;

           
            Geometry = new PrimitiveGeometry(path, allowedOperations);

            
            Transform = new SerializableGraphicsMatrix();
        }

        /// <summary>
        /// trasforma il punto di onMouseUp in una figura
        /// </summary>
        public void PushTransformToSelectedFigures()
        {
            foreach (var fig in _selected)
                fig.PushTransform(Transform);
            GrabGeometry();
        }

      
        public void Translate(float offsetX, float offsetY)
        {
            var m = new SerializableGraphicsMatrix();
            m.Matrix.Translate(offsetX, offsetY);
            Transform = m;
        }

       
        public void Scale(float scaleX, float scaleY, PointF anchor)
        {
            
            var allowSize = Geometry.AllowedOperations.HasFlag(AllowedOperations.Size);
            var allowScale = Geometry.AllowedOperations.HasFlag(AllowedOperations.Scale);

            if (!allowScale && !allowSize)
                return; 

            if (allowScale && !allowSize)             
                scaleX = scaleY = Math.Max(scaleX, scaleY);

            
            var m = new SerializableGraphicsMatrix();
            m.Matrix.Translate(anchor.X, anchor.Y);    
            m.Matrix.Scale(scaleX, scaleY);            
            m.Matrix.Translate(-anchor.X, -anchor.Y);  

            //
            Transform = m;
        }

      
        public void Skew(float skewX, float skewY, PointF anchor)
        {
       
            var allowSkew = Geometry.AllowedOperations.HasFlag(AllowedOperations.Skew);
            if (!allowSkew)
                return; 

           
            var m = new SerializableGraphicsMatrix();
            m.Matrix.Translate(anchor.X, anchor.Y);    
            m.Matrix.Shear(skewX, skewY);              
            m.Matrix.Translate(-anchor.X, -anchor.Y);  

            //
            Transform = m;
        }

       
        public void Rotate(float angle, PointF center)
        {
            
            var allowRotate = Geometry.AllowedOperations.HasFlag(AllowedOperations.Rotate);

            if (!allowRotate)
                return; 

            
            var m = new SerializableGraphicsMatrix();
            m.Matrix.RotateAt(angle, center);      

            //
            Transform = m;
        }

    
        public bool Contains(Figure figure)
        {
            return _selected.Contains(figure);
        }

        
        public Selection()
        {
        
            Style.BorderStyle.DashStyle = DashStyle.Dash;
            Style.BorderStyle.Color = Color.Magenta;
            Style.FillStyle.IsVisible = false;
            
            GrabGeometry();
        }

      
        public int Count { get { return _selected.Count; } }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<Figure> GetEnumerator()
        {
            return _selected.GetEnumerator();
        }

      
        public void MoveGradient(Figure owner, int index, PointF newPosition)
        {
            var gradient = owner.Style.FillStyle as IGradientFill;
            if (gradient == null)
                return; // работаем только с градиентами
            
            //get points in world coordinates
            var points = gradient.GetGradientPoints(owner);

            //move point
            points[index] = newPosition;

            //push
            gradient.SetGradientPoints(owner, points);         
        }

       
        public void MoveWarpNode(Figure owner, int index, PointF newPosition)
        {
            var warped = (WarpRendererDecorator)RendererDecorator.GetDecorator(owner.Renderer, typeof(WarpRendererDecorator));
            if (warped == null)
                return; 

            //get points in world coordinates
            var points = warped.GetWarpPoints(owner);

            //move point
            points[index] = newPosition;

            //push
            warped.SetWarpPoints(owner, points);
        }

  
        public void MoveVertex(Figure owner, int index, PointF newPosition)
        {
          
            var allowVertex = owner.Geometry.AllowedOperations.HasFlag(AllowedOperations.Vertex);

            if (!allowVertex)
                return; 

            var transformed = owner.Geometry as ITransformedGeometry;
            if (transformed == null)
                return; 

            //get points in world coordinates
            var points = transformed.GetTransformedPoints(owner);

            //move point
            if (index >= 0 && index < points.Length)
                points[index] = newPosition;
  
            //push
            transformed.SetTransformedPoints(owner, points);
            //
            GrabGeometry();
        }

        public void RemoveVertex(Figure owner, int index)
        {
            
            var allowVertex = Geometry.AllowedOperations.HasFlag(AllowedOperations.Vertex);

            if (!allowVertex)
                return; 
            List<PointF> points;
            var polygone = owner.Geometry as PolygoneGeometry;
            if (polygone != null)
            {
               
                points = new List<PointF>(polygone.Points);

                if (polygone.IsClosed && points.Count < 4) return;
                if (!polygone.IsClosed && points.Count < 3) return;

               
                points.RemoveAt(index);
              
                polygone.Points = points.ToArray();
                //
                GrabGeometry();
                return;
            }
            var bezier = owner.Geometry as BezierGeometry;
            if (bezier == null) return;
            points = new List<PointF>(bezier.Points);
           
            if (points.Count < 5) return;
            var types = new List<byte>(bezier.Types);
           
            if ((types[index] & 0x27) == (byte)PathPointType.Line)
            {
            
                points.RemoveAt(index);
              
                types.RemoveAt(index);
            }
           
            else if ((types[index] & 0x07) == (byte)PathPointType.Start)
            {
                
                var typ = types[index];
                if ((types[index + 1] & 0x07) == (byte)PathPointType.Line)
                {
                   
                    points.RemoveAt(index);
                    types.RemoveAt(index);
                    
                    types[index] = typ;
                }
                else
                {
                    
                    for (var i = 0; i < 3; i++)
                    {
                        points.RemoveAt(index);
                        types.RemoveAt(index);
                    }
                    
                    types[index] = typ;
                }
            }
            else
            {
                // удаляем две предыдущие контрольные точки и вершину
                index -= 2;
                for (var i = 0; i < 3; i++)
                {
                    points.RemoveAt(index);
                    types.RemoveAt(index);
                }
            }
            //push
            bezier.Points = points.ToArray();
            bezier.Types = types.ToArray();
            //
            GrabGeometry();
        }

       
        public void InsertVertex(Figure owner, PointF point)
        {
            
            var allowVertex = Geometry.AllowedOperations.HasFlag(AllowedOperations.Vertex);

            if (!allowVertex)
                return; 

            var transformed = owner.Geometry as ITransformedGeometry;
            if (transformed == null)
                return; 

            //get points in world coordinates
            var points = transformed.GetTransformedPoints(owner).ToList();

            using (var pen = new Pen(Color.Black, 5))
            {
                
                var k = transformed.IsClosed ? 0 : 1;
                for (var i = k; i < points.Count; i++)
                {
                    
                    var pt1 = i == 0 ? points[points.Count - 1] : points[i - 1];
                    var pt2 = points[i];
                    using (var path = new GraphicsPath())
                    {
                        path.AddLine(pt1, pt2);
                        if (!path.IsOutlineVisible(point, pen)) continue;
                        if (owner.Geometry is PolygoneGeometry)
                        {
                            
                            points.Insert(i, point);
                            //push
                            transformed.SetTransformedPoints(owner, points.ToArray());
                        }
                        else
                        {
                            var bezier = owner.Geometry as BezierGeometry;
                            if (bezier == null) break;
                            var types = bezier.Types.ToList();
                            
                            for (var j = 0; j < 3; j++)
                            {
                                points.Insert(i, point);
                                types.Insert(i, (byte)PathPointType.Bezier); 
                            }
                            transformed.SetTransformedPoints(owner, points.ToArray());
                            bezier.Types = types.ToArray();
                        }
                        break;
                    }
                }
            }

            //
            GrabGeometry();
        }

    }
}
