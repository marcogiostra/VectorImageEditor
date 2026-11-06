using EditorModel.Common;
using System;
using System.Drawing;
using EditorModel.Figures;
using EditorModel.Figures.Extentions;

namespace EditorModel.Geometry
{

    public enum BasketPolylineKind
    {
        None,
        Spostamento,
        Blocco,
        PalleggioTratteggiato,
        TiroPuntinato,
        PalleggioSinusoidale
    }

 

    [Serializable]
    public sealed class PolygoneGeometry : Geometry, IDisposable, ITransformedGeometry
    {
        private PointF[] _points;

      
        private readonly SerializableGraphicsPath _path = new SerializableGraphicsPath();

        
        private readonly AllowedOperations _allowedOperations;

      
        public bool IsClosed { get; set; } = true;

        public bool IsSmoothed { get; set; }
        public bool IsSinusoidal { get; set; }
        public BasketPolylineKind BasketKind { get; set; }

   

        public byte[] GetTransformedPointTypes(Figure owner)
        {
            return (byte[])_path.Path.PathTypes.Clone();
        }

      
        public PointF[] Points
        {
            get { return _points; }
            set { _points = value; }
        }

       
        public PointF[] GetTransformedPoints(Figure owner)
        {
            var points = (PointF[])Points.Clone();
            owner.Transform.Matrix.TransformPoints(points);
            return points;
        }

      
        public void SetTransformedPoints(Figure owner, PointF[] points)
        {
            points = (PointF[])points.Clone();
            var m = owner.Transform.Matrix.Clone();
            m.Invert();
            m.TransformPoints(points);
            Points = points;
        }

        public void Dispose()
        {
            _path?.Dispose();
        }

        
        internal PolygoneGeometry(bool isClosed = true, PointF[] points = null )
        {
           
            IsClosed = isClosed;
            _allowedOperations = AllowedOperations.All ^ AllowedOperations.Pathed;
            var rect = new RectangleF(-0.5f, -0.5f, 1, 1);
            if (points != null)
            {
                _points = (PointF[])points.Clone();
            }
            else if (isClosed)
                _points = new[]
                    {
                        new PointF(rect.Left, rect.Top),
                        new PointF(rect.Left + rect.Width, rect.Top),
                        new PointF(rect.Left + rect.Width, rect.Top + rect.Height),
                        new PointF(rect.Left, rect.Top + rect.Height)
                    };
            else
                _points = new[]
                    {
                        new PointF(rect.Left, rect.Top),
                        new PointF(rect.Left + rect.Width, rect.Top + rect.Height)
                    };
        }

       
        public override SerializableGraphicsPath Path
        {
            get
            {
                
                _path.Path.Reset();
               
                if (IsClosed)
                {
                    if (IsSinusoidal)
                        _path.Path.AddSinuousBasketLines(_points, true);
                    else if (IsSmoothed)
                        _path.Path.AddClosedCurve(_points);
                    else
                        _path.Path.AddPolygon(_points);
                }
                else
                {
                    if (IsSinusoidal)
                        _path.Path.AddSinuousBasketLines(_points, false);
                    else if (IsSmoothed)
                        _path.Path.AddCurve(_points);
                    else
                        _path.Path.AddLines(_points);

                }

                if (!IsClosed && BasketKind == BasketPolylineKind.Blocco && _points.Length >= 2)
                    AddBlockMark(_path.Path, _points[_points.Length - 2], _points[_points.Length - 1]);
                
                return _path;
            }
        }

        private static void AddBlockMark(System.Drawing.Drawing2D.GraphicsPath path, PointF previous, PointF endpoint)
        {
            const float halfLength = 6f;
            float dx = endpoint.X - previous.X;
            float dy = endpoint.Y - previous.Y;
            float length = (float)Math.Sqrt(dx * dx + dy * dy);
            if (length < 0.0001f) return;
            float nx = -dy / length * halfLength;
            float ny = dx / length * halfLength;
            path.StartFigure();
            path.AddLine(endpoint.X - nx, endpoint.Y - ny, endpoint.X + nx, endpoint.Y + ny);
        }

    
        public override AllowedOperations AllowedOperations { get { return _allowedOperations; } }

        public override string ToString()
        {
            return IsClosed ? "Polygon" : "Polyline";
        }

    }
}
