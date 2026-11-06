using EditorModel.Common;
using EditorModel.Figures;
using EditorModel.Geometry;
using EditorModel.Renderers;
using EditorModel.Selections;
using EditorModel.Style;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using VectorImageEditor.Common;
using VectorImageEditor.DevWithGiosh;

namespace VectorImageEditor.Controllers
{
    /// <summary>
    /// Elenco delle modalità dell'EditorMode
    /// </summary>
    public enum EditorMode
    {
        Select,
        FrameSelect,
        AddLine,
        ChangeGeometry,
        Drag,
        CreateFigure,
        Skew,
        Verticies,
        Warp,
        AddSinuousBasketLines

    }

    /// <summary>
    /// Elabora i movimenti del mouse adeguando il tipo di operazione
    /// </summary>
    public class SelectionController
    {
        #region DICHIARAZIONI
        private readonly Selection _selection;
        private readonly List<Marker> _markers;
        private readonly Layer _layer;

        public Func<Figure> CreateFigureRequest;
        public Cursor CreateFigureCursor = Cursors.Default;

        public float ScaleFactor { get; set; }

        public Layer CurrentLayer { get; set; }

        public Selection Selection { get { return _selection; } }

        public List<Marker> Markers { get { return _markers; } }

        private bool _wasMouseMoving;
        private bool _wasVertexMoving;
        private bool _isMouseDown;
        private bool _isFigureCreated;
        private Point _firstMouseDown;
        private Marker _movedMarker;

        private EditorMode _editorMode = EditorMode.Select;
        private EditorMode _lastMode = EditorMode.Select;
        public EditorMode EditorMode
        {
            get { return _editorMode; }
            set
            {

                if (_editorMode == EditorMode.Select ||
                    _editorMode == EditorMode.Skew ||
                    _editorMode == EditorMode.Warp ||
                    _editorMode == EditorMode.Verticies)
                {
                    _lastMode = _editorMode;
                }
                _editorMode = value;
                _selection.IsFrameVisible = _editorMode != EditorMode.Verticies && (_editorMode != EditorMode.ChangeGeometry || _lastMode != EditorMode.Verticies);
                if (_editorMode == EditorMode.Select ||
                    _editorMode == EditorMode.Skew ||
                    _editorMode == EditorMode.Warp ||
                    _editorMode == EditorMode.Verticies)
                {

                    UpdateMarkers();
                }
                OnEditorModeChanged(EditorMode);
            }
        }
        #endregion DICHIARAZIONI

        #region EVENTI
        public event Action SelectedFigureChanged = delegate { };


        public event Action SelectedTransformChanging = delegate { };


        public event Action SelectedTransformChanged = delegate { };


        public event Action<Rectangle, float> SelectedRangeChanging = delegate { };


        public event Action<EditorMode> EditorModeChanged = delegate { };


        public event Action LayerStartChanging = delegate { };


        public event Action LayerChanged = delegate { };


        public event Action ResetFigureCreator = delegate { };        
        #endregion EVENTI

        #region Class
        public SelectionController(Layer layer)
        {
            _selection = new Selection();
            _markers = new List<Marker>();
            _layer = layer;
            ScaleFactor = 1f;
        }
        #endregion Class

        public void OnDblClick(Point location, Keys modifierKeys, bool finishAtLastFixedPoint = false)
        {
            var activeLine = _selection.Geometry as AddLineGeometry;
            if (finishAtLastFixedPoint && activeLine != null && activeLine.Points.Count > 0)
                location = Point.Round(activeLine.Points[activeLine.Points.Count - 1]);
            var point = Point.Ceiling(new PointF(location.X / ScaleFactor, location.Y / ScaleFactor));
            if (RibbonLineUpdated(point))
            {

                var line = new Figure();
                var lineGeometry = _selection.Geometry as AddLineGeometry;
                if (lineGeometry != null)
                {
                    if (lineGeometry.IsSmoothed)
                    {
                        if (lineGeometry.BasketKind != BasketPolylineKind.None)
                        {
                            var route = lineGeometry.Points.ToList();
                            if (route.Count == 0 || route[route.Count - 1] != lineGeometry.EndPoint)
                                route.Add(lineGeometry.EndPoint);
                            using (var baseline = new System.Drawing.Drawing2D.GraphicsPath())
                            {
                                if (route.Count > 2)
                                    baseline.AddCurve(route.ToArray());
                                else
                                    baseline.AddLines(route.ToArray());
                                FigureBuilder.BuildBasketBezierGeometry(line, lineGeometry.BasketKind, baseline.PathPoints, baseline.PathTypes);
                            }
                        }
                        else
                        {
                            var points = lineGeometry.Path.Path.PathPoints;
                            var types = lineGeometry.Path.Path.PathTypes;
                            if (!finishAtLastFixedPoint)
                                Helper.CutLastBezierPoints(ref points, ref types);
                            FigureBuilder.BuildBezierGeometry(line, points, types);
                        }
                    }
                    else
                    {
                        var points = lineGeometry.Points.ToList();
                        if (points.Count == 0 || points[points.Count - 1] != lineGeometry.EndPoint)
                            points.Add(lineGeometry.EndPoint);
                        var finalPoints = points.ToArray();
                        if (lineGeometry.BasketKind != BasketPolylineKind.None)
                            FigureBuilder.BuildBasketPolylineGeometry(line, lineGeometry.BasketKind, lineGeometry.IsClosed, finalPoints);
                        else
                            FigureBuilder.BuildPolyGeometry(line, lineGeometry.IsClosed, finalPoints);
                    }
                    OnLayerStartChanging();
                    _layer.Figures.Add(line);
                    OnLayerChanged();
                    if (finishAtLastFixedPoint)
                    {
                        EditorMode = EditorMode.Select;
                        Clear();
                        _isMouseDown = false;
                        _isFigureCreated = false;
                    }
                }
                return;
            }


        }

        /// <summary>
        /// OnMouseDown su CONVAS
        /// </summary>
        /// <param name="location"></param>
        /// <param name="modifierKeys"></param>
        public void OnMouseDown(Point location, Keys modifierKeys)
        {
            //inizializza i vari flag
            _wasMouseMoving = false;
            _wasVertexMoving = false;
            _isMouseDown = true;
            _movedMarker = null;
  
            //inizializza il primo punto del click
            var point = Point.Ceiling(new PointF(location.X / ScaleFactor, location.Y / ScaleFactor));
            _firstMouseDown = point;

            if (CreateFigureRequest != null)
            {
           
                //Imposta il modo editor come CREARE UNA FIGURA
                EditorMode = EditorMode.CreateFigure;

                //imposta il layer in  modalità cambiamento
                OnLayerStartChanging();

                CreateFigure(point);
                _isFigureCreated = true;
                return;
            }

            if (DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.Polyline ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.PalleggioBasket ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.PassaggioBasket ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.SpostamentoBasket ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.BloccoBasket ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.TiroBasket ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.SpostamentoBasketBezier ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.BloccoBasketBezier ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.PalleggioBasketBezier ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.PassaggioBasketBezier ||
                DWG_GeometrySelected.GeometrySelected == MacroGeometrySelectionMode.TiroBasketBezier)
                if (RibbonLineUpdated(point, true))
                    return;
           
            if (EditorMode != EditorMode.Select &&
                EditorMode != EditorMode.Skew &&
                EditorMode != EditorMode.Warp &&
                EditorMode != EditorMode.Verticies)
                Clear();   
            
            FindMarkerAt(point, out _movedMarker);
            if (_movedMarker == null)   
            {
                Figure fig;
                if ((EditorMode == EditorMode.Select ||
                     EditorMode == EditorMode.Skew ||
                     EditorMode == EditorMode.Warp ||
                     EditorMode == EditorMode.Verticies) &&
                    _selection.FindFigureAt(_layer, point, out fig)) 
                {
                   
                    if (!_selection.Contains(fig))
                    {
                    
                        if (!modifierKeys.HasFlag(Keys.Shift))
                            _selection.Clear(); 
                                                
                        _selection.Add(fig);
                        OnSelectedFigureChanged();
                    }
                    else 
                    {
                        if (EditorMode == EditorMode.Verticies && modifierKeys.HasFlag(Keys.Control))
                        {
                            OnLayerStartChanging();
                            _selection.InsertVertex(fig, point);
                            OnLayerChanged();
                        }         
                        else
                        if (_selection.Count > 1 && modifierKeys.HasFlag(Keys.Shift))
                        {
                            _selection.Remove(fig);
                            OnSelectedFigureChanged();
                        }
                    }
              
                    if (_selection.Count > 0)
                        EditorMode = EditorMode.Drag;
                }
                else
                {
                 
                    Clear();
                    OnSelectedFigureChanged();

                    if (CreateFigureRequest == null)
                    {
                        
                        FigureBuilder.BuildFrameGeometry(_selection, point);
                        EditorMode = EditorMode.FrameSelect;
                    }
                }
            }
            else
            {
                if (EditorMode == EditorMode.Verticies)
                {
                    if (modifierKeys.HasFlag(Keys.Control))
                    {
                        if (_movedMarker.MarkerType == MarkerType.Vertex)
                        {
                         
                            OnLayerStartChanging();
                            _selection.RemoveVertex(((VertexMarker)_movedMarker).Owner, ((VertexMarker)_movedMarker).Index);
                            OnLayerChanged();
                            UpdateMarkers();
                            OnSelectedFigureChanged();
                        }
                    }
                }
                else
                    EditorMode = EditorMode.ChangeGeometry;
            }
        }

        private bool RibbonLineUpdated(Point point, bool fixing = false)
        {
            var addLineGeometry = _selection.Geometry as AddLineGeometry;
            if (addLineGeometry != null)
            {
                if (fixing)
                    addLineGeometry.AddPoint(point);
                else
                    addLineGeometry.EndPoint = point;
                OnSelectedTransformChanging();
                return true;
            }
            return false;

        }



        /// <summary>
        /// OnMouseMove su CONVAS
        /// </summary>
        /// <param name="location"></param>
        /// <param name="modifierKeys"></param>
        public void OnMouseMove(Point location, Keys modifierKeys)
        {
            var point = Point.Ceiling(new PointF(location.X / ScaleFactor, location.Y / ScaleFactor));
            if (!_isMouseDown)
            {
                RibbonLineUpdated(point);
                return;
            }
            _wasMouseMoving = true;


            if (_movedMarker != null)
            {
                OnMarkerMoved(_movedMarker, point);
                OnSelectedTransformChanging();
            }
            else
            {
                switch (EditorMode)
                {
                    case EditorMode.FrameSelect:
                        var frameGeometry = _selection.Geometry as FrameGeometry;
                        if (frameGeometry != null)
                            frameGeometry.EndPoint = point;
                        break;

                    case EditorMode.AddLine:
                        RibbonLineUpdated(point);
                        break;

                   

                    case EditorMode.CreateFigure:
                        var startPoint = _firstMouseDown;
                        var scale = new PointF(point.X - startPoint.X, point.Y - startPoint.Y);
                        if (Math.Abs(scale.X) < Helper.EPSILON) scale.X = Helper.EPSILON;
                        if (Math.Abs(scale.Y) < Helper.EPSILON) scale.Y = Helper.EPSILON;
                        _selection.Scale(scale.X, scale.Y, startPoint);
                        OnSelectedTransformChanging();
                        break;

                    default:
                        if (EditorMode == EditorMode.Drag)
                        {
                            if (_lastMode == EditorMode.Verticies || _lastMode == EditorMode.Skew || _lastMode == EditorMode.Warp) break;
                            var mouseOffset = new Point(point.X - _firstMouseDown.X, point.Y - _firstMouseDown.Y);
                            _selection.Translate(mouseOffset.X, mouseOffset.Y);
                            OnSelectedTransformChanging();
                        }
                        break;
                }
            }
            var selrect = Rectangle.Ceiling(_selection.GetTransformedPath().Path.GetBounds());
            var angle = Helper.GetAngle(_selection.Transform);
            OnSelectedRangeChanging(selrect, angle);
        }

        /// <summary>
        /// ONMouseUp su CONVAS
        /// </summary>
        /// <param name="location"></param>
        /// <param name="modifierKeys"></param>
        public void OnMouseUp(Point location, Keys modifierKeys)
        {
            var point = Point.Ceiling(new PointF(location.X / ScaleFactor, location.Y / ScaleFactor));
            if (_isMouseDown)
            {
                switch (EditorMode)
                {
                    case EditorMode.FrameSelect:
                        var rect = _selection.GetTransformedPath().Path.GetBounds();
                        foreach (var fig in _layer.Figures.Where(fig =>  rect.Contains(Rectangle.Ceiling(fig.GetTransformedPath().Path.GetBounds()))))
                        {
                            if (!_layer.IsVisible(fig) || _layer.IsLocked(fig)) continue;
                            _selection.Add(fig);
                        }

                        if (_selection.Count == 0)
                            _selection.PushTransformToSelectedFigures();

                        OnSelectedFigureChanged();
                        break;
                    case EditorMode.AddLine:
                        RibbonLineUpdated(point, true);
                        _isMouseDown = false;
                        return;
                    case EditorMode.CreateFigure:
                        _selection.PushTransformToSelectedFigures();
                        OnLayerChanged();
                        break;
                    case EditorMode.Verticies:
                        if (_wasVertexMoving)
                        {
                            OnLayerChanged();
                            _wasVertexMoving = false;
                        }
                        _isMouseDown = false;
                        return;
                    default:
                        if (_firstMouseDown == point) break;
                        FixFiguresTranslation(modifierKeys);
                        break;
                }

                if (_wasMouseMoving)
                {
                    _wasMouseMoving = false;
                    OnSelectedTransformChanged();
                }
            }

            UpdateMarkers();

            EditorMode = _lastMode;

            if (_isFigureCreated)
            {
                _isFigureCreated = false;
                OnResetFigureCreator();
            }

            _isMouseDown = false;
        }


        private void CreateFigure(Point location)
        {
   
            var newFig = CreateFigureRequest();
            if (newFig == null) return;
            var pg = newFig.Geometry as PolygoneGeometry;
            if (pg != null)
            {
                if (!(_selection.Geometry is AddLineGeometry))
                    FigureBuilder.BuildAddLineGeometry(_selection, location, pg.IsClosed, false, pg.IsSinusoidal, pg.BasketKind);
                EditorMode = EditorMode.AddLine;
                CreateFigureRequest = null;
                return;
            }
            var cg = newFig.Geometry as BezierGeometry;
            if (cg != null)
            {
                if (!(_selection.Geometry is AddLineGeometry))
                    FigureBuilder.BuildAddLineGeometry(_selection, location, cg.IsClosed, true, false, cg.BasketKind);
                EditorMode = EditorMode.AddLine;
                CreateFigureRequest = null;
                return;
            }
            newFig.Transform.Matrix.Translate(location.X + 0.5f, location.Y + 0.5f);
            _layer.Figures.Add(newFig);
            _selection.Add(newFig);
            CreateFigureRequest = null;
            OnSelectedFigureChanged();
        }


        private void FixFiguresTranslation(Keys modifierKeys)
        {
            if (!_selection.Transform.Matrix.IsIdentity)
            {
                OnLayerStartChanging();
                if (modifierKeys.HasFlag(Keys.Control))
                {
                    var list = new List<Figure>();
                    foreach (var fig in Selection.Select(figure => figure.DeepClone()))
                    {
                        list.Add(fig);
                        _layer.Figures.Add(fig);
                    }
                }
                _selection.PushTransformToSelectedFigures();
                OnLayerChanged();
            }
        }

 
        public void FlipX()
        {
            OnLayerStartChanging();
            _selection.FlipX();
            _selection.PushTransformToSelectedFigures();
            OnLayerChanged();

            UpdateMarkers();
        }

        public void FlipY()
        {
            OnLayerStartChanging();
            _selection.FlipY();
            _selection.PushTransformToSelectedFigures();
            OnLayerChanged();

            UpdateMarkers();
        }

        public void Rotate90Cw()
        {
            OnLayerStartChanging();
            _selection.Rotate90Cw();
            _selection.PushTransformToSelectedFigures();
            OnLayerChanged();

            UpdateMarkers();
        }

        public void Rotate90Ccw()
        {
            OnLayerStartChanging();
            _selection.Rotate90Ccw();
            _selection.PushTransformToSelectedFigures();
            OnLayerChanged();

            UpdateMarkers();
        }

 
        public void Rotate180()
        {
            OnLayerStartChanging();
            _selection.Rotate180();
            _selection.PushTransformToSelectedFigures();
            OnLayerChanged();

            UpdateMarkers();
        }

        public void AlignHorizontal(FigureAlignment alignment)
        {
            OnLayerStartChanging();
            _selection.AlignHorizontal(alignment);
            _selection.PushTransformToSelectedFigures();
            OnLayerChanged();

            UpdateMarkers();
        }

        public void AlignVertical(FigureAlignment alignment)
        {
            OnLayerStartChanging();
            _selection.AlignVertical(alignment);
            _selection.PushTransformToSelectedFigures();
            OnLayerChanged();

            UpdateMarkers();
        }


        public void SameResize(FigureResize resize)
        {
            OnLayerStartChanging();
            _selection.SameResize(resize);
            _selection.PushTransformToSelectedFigures();
            OnLayerChanged();

            UpdateMarkers();
        }

        public void EvenSpaces(FigureArrange arrange)
        {
            OnLayerStartChanging();
            _selection.EvenSpaces(arrange);
            _selection.PushTransformToSelectedFigures();
            OnLayerChanged();

            UpdateMarkers();
        }

        public void UpdateMarkers()
        {
            var list = new List<Figure>(_selection);
            _selection.Clear();
            foreach (var figure in list) _selection.Add(figure);
            BuildMarkers();
            UpdateMarkerPositions();            
        }


        public void Clear()
        {
            _markers.Clear();
            _selection.Clear();
            OnSelectedFigureChanged();
        }

        #region RILEVATORI_DI_EVENTI
        private void OnSelectedFigureChanged()
        {
            SelectedFigureChanged();
        }

        private void OnSelectedTransformChanging()
        {
            SelectedTransformChanging();
        }

        private void OnSelectedTransformChanged()
        {
            SelectedTransformChanged();
        }

        private void OnSelectedRangeChanging(Rectangle rect, float angle)
        {
            SelectedRangeChanging(rect, angle);
        }

  
        private void OnEditorModeChanged(EditorMode mode)
        {
            EditorModeChanged(mode);
        }

  
        private void OnLayerStartChanging()
        {
            LayerStartChanging();
        }

  
        private void OnLayerChanged()
        {
            LayerChanged();
        }

 
        private void OnResetFigureCreator()
        {
            ResetFigureCreator();
        }

        #endregion RILEVATORI_DI_EVENTI

        #region MARKER
        private bool FindMarkerAt(Point point, out Marker marker)
        {
            marker = null;
            var found = false;
            // просматриваем маркеры с конца списка
            for (var i = Markers.Count - 1; i >= 0; i--)
            {
                var fig = Markers[i];
                fig.Transform.Matrix.Scale(1 / ScaleFactor, 1 / ScaleFactor);
                var path = fig.GetTransformedPath();
                fig.Transform.Matrix.Scale(ScaleFactor, ScaleFactor);
                if (!path.Path.IsVisible(point)) continue;
                marker = fig;
                found = true;
                break;
            }
            return found;
        }

       public void RemoveLockedOrNotVisible()
        {
            var list = new List<Figure>();
            foreach (var fig in _selection)
            {
                foreach (var layer in _layer.Layers.Where(layer =>
                     !layer.AllowedOperations.HasFlag(LayerAllowedOperations.Visible) ||
                      layer.AllowedOperations.HasFlag(LayerAllowedOperations.Locking)))
                {
                    if (!list.Contains(fig))
                        list.Add(fig);
                }
            }
            foreach (var fig in list)
                _selection.Remove(fig);

        }


        private Marker CreateMarker(MarkerType type, float posX, float posY, UserCursor cursor,
                                    float anchorX, float anchorY, float offsetX = 0, float offsetY = 0)
        {
            var normPoint = new PointF(posX, posY);
            var anchPoint = new PointF(anchorX, anchorY);
            return new Marker
            {
                MarkerType = type,
                Cursor = CursorFactory.GetCursor(cursor),
                Position = _selection.ToWorldCoordinates(normPoint).Add(new PointF(offsetX, offsetY)),
                AnchorPosition = _selection.ToWorldCoordinates(anchPoint)
            };
        }

        private void OnMarkerMoved(Marker marker, Point mousePos)
        {
            var scale = Helper.GetScale(marker.Position, marker.AnchorPosition, mousePos);
            switch (marker.MarkerType)
            {
                case MarkerType.SizeX:
                    if (_selection.GetTransformedPath().Path.GetBounds().Width > Helper.EPSILON)
                        _selection.Scale(scale, 1, marker.AnchorPosition);
                    break;
                case MarkerType.SizeY:
                    if (_selection.GetTransformedPath().Path.GetBounds().Height > Helper.EPSILON)
                        _selection.Scale(1, scale, marker.AnchorPosition);
                    break;
                case MarkerType.SkewX:
                    var dx = (marker.Position.X - mousePos.X)/(marker.AnchorPosition.Y - marker.Position.Y);
                    _selection.Skew(dx, 0, marker.AnchorPosition);
                    break;
                case MarkerType.SkewY:
                    var dy = (marker.Position.Y - mousePos.Y)/(marker.AnchorPosition.X - marker.Position.X);
                    _selection.Skew(0, dy, marker.AnchorPosition);
                    break;
                case MarkerType.Scale:
                    _selection.Scale(scale, scale, marker.AnchorPosition);
                    break;
                case MarkerType.Rotate:
                    var angle = Helper.GetAngle(marker.Position, marker.AnchorPosition, mousePos);
                    _selection.Rotate(angle, marker.AnchorPosition);
                    break;
                case MarkerType.Vertex:
                case MarkerType.ControlBezier:
                case MarkerType.Gradient:
                case MarkerType.Warp:
                    if (!_wasVertexMoving)
                        OnLayerStartChanging();
                    
                    var vm = (VertexMarker) marker;
                    if (marker.MarkerType == MarkerType.Gradient)
                        _selection.MoveGradient(vm.Owner, vm.Index, mousePos);
                    else if (marker.MarkerType == MarkerType.Warp)
                        _selection.MoveWarpNode(vm.Owner, vm.Index, mousePos);
                    else
                        _selection.MoveVertex(vm.Owner, vm.Index, mousePos);

                    vm.Position = mousePos;
                    UpdateMarkerPositions();

                    _wasVertexMoving = true;
                    break;
            }
        }

        private void BuildMarkers()
        {
            Markers.Clear();
            if (Selection.Count == 0) return;
            switch (_editorMode)
            {
                case EditorMode.Warp:
                    if (_selection.Geometry.AllowedOperations.HasFlag(AllowedOperations.Warp)) 
                    {
                        foreach (var fig in RendererDecorator.WhereContainsDecorator(_selection, typeof(WarpRendererDecorator)))
                        {
                            var warped = fig.Renderer as WarpRendererDecorator;
                            //get transformed points
                            if (warped == null) continue;
                            var points = warped.GetWarpPoints(fig);
                            for (var i = 0; i < points.Length; i++)
                                Markers.Add(CreateMarker(MarkerType.Warp, points[i], i, fig));
                        }
                    }
                    break;
                case EditorMode.Skew:
                    if (_selection.Geometry.AllowedOperations.HasFlag(AllowedOperations.Skew)) //если разрешён скос
                    {
                        Markers.Add(CreateMarker(MarkerType.SkewX, 0.5f, 0, UserCursor.SkewHorizontal, 1, 1));
                        Markers.Add(CreateMarker(MarkerType.SkewY, 0, 0.5f, UserCursor.SkewVertical, 1, 1));
                        Markers.Add(CreateMarker(MarkerType.SkewY, 1, 0.5f, UserCursor.SkewVertical, 0, 0));
                        Markers.Add(CreateMarker(MarkerType.SkewX, 0.5f, 1, UserCursor.SkewHorizontal, 0, 0));
                    }
                    break;
                case EditorMode.Verticies:
                    if (_selection.Geometry.AllowedOperations.HasFlag(AllowedOperations.Vertex)) //если разрешено редактирование вершин
                    {
                        foreach (var fig in _selection.Where(figure => figure.Geometry is ITransformedGeometry))
                        {
                            var transformed = fig.Geometry as ITransformedGeometry;
                            //get transformed points
                            if (transformed == null) continue;
                            var points = transformed.GetTransformedPoints(fig);
                            var types = transformed.GetTransformedPointTypes(fig);
                            Markers.Add(CreateMarker(MarkerType.Vertex, points[0], 0, fig));
                            var nt = 0;
                            MarkerType mt;
                            for (var i = 1; i < points.Length; i++)
                            {
                                var typ = types[i];
                                if ((typ & 0x07) == 3)
                                {
                                    mt = nt % 3 == 2 ? MarkerType.Vertex : MarkerType.ControlBezier;
                                    nt++;
                                }
                                else
                                {
                                    mt = MarkerType.Vertex;
                                    nt = 0;
                                }
                                Markers.Add(CreateMarker(mt, points[i], i, fig));
                            }
                        }
                    }
                    foreach (var fig in _selection.Where(figure => figure.Style.FillStyle is IGradientFill))
                    {
                        var gradient = fig.Style.FillStyle as IGradientFill;
                        //get transformed points
                        if (gradient == null) continue;
                        var points = gradient.GetGradientPoints(fig);
                        for (var i = 0; i < points.Length; i++)
                            Markers.Add(CreateMarker(MarkerType.Gradient, points[i], i, fig));
                    }
                    break;
                case EditorMode.Select:
                    if (Selection.Geometry.AllowedOperations.HasFlag(AllowedOperations.Scale)) //если разрешено масштабирование
                    {
                        Markers.Add(CreateMarker(MarkerType.Scale, 0, 0, UserCursor.SizeNWSE, 1, 1));
                        Markers.Add(CreateMarker(MarkerType.Scale, 1, 0, UserCursor.SizeNESW, 0, 1));
                        Markers.Add(CreateMarker(MarkerType.Scale, 1, 1, UserCursor.SizeNWSE, 0, 0));
                        Markers.Add(CreateMarker(MarkerType.Scale, 0, 1, UserCursor.SizeNESW, 1, 0));
                    }

                    if (Selection.Geometry.AllowedOperations.HasFlag(AllowedOperations.Size)) //если разрешено изменение размера
                    {
                        Markers.Add(CreateMarker(MarkerType.SizeY, 0.5f, 0, UserCursor.SizeNS, 0.5f, 1));
                        Markers.Add(CreateMarker(MarkerType.SizeX, 1, 0.5f, UserCursor.SizeWE, 0, 0.5f));
                        Markers.Add(CreateMarker(MarkerType.SizeY, 0.5f, 1, UserCursor.SizeNS, 0.5f, 0));
                        Markers.Add(CreateMarker(MarkerType.SizeX, 0, 0.5f, UserCursor.SizeWE, 1, 0.5f));
                    }

                    if (Selection.Geometry.AllowedOperations.HasFlag(AllowedOperations.Rotate)) //если разрешено вращение
                    {
                        var sf = ScaleFactor;
                        var rotateMarker = CreateMarker(MarkerType.Rotate, 1, 0, UserCursor.Rotate, 0.5f, 0.5f, 15 / sf, -15 / sf);
                        Markers.Add(rotateMarker);
                    }
                    break;
            }

            foreach (var marker in Markers)
                FigureBuilder.BuildMarkerGeometry(marker);
        }

        private VertexMarker CreateMarker(MarkerType markerType, PointF point, int index, Figure fig)
        {
            var marker = new VertexMarker
            {
                MarkerType = markerType,
                Cursor = CursorFactory.GetCursor(UserCursor.SizeAll),
                Position = point,
                Index = index,
                Owner = fig
            };
            return marker;
        }

        private void UpdateMarkerPositions()
        {
            foreach (var marker in Markers)
                marker.PushPositionToTransform();
        }

        #endregion MARKER

        public Cursor GetCursor(Point location, Keys modifierKeys, MouseButtons button)
        {
            var point = Point.Ceiling(new PointF(location.X / ScaleFactor, location.Y / ScaleFactor));
            if (CreateFigureRequest != null)
                return CreateFigureCursor;

            switch (EditorMode)
            {
                case EditorMode.Select:
                case EditorMode.Skew:
                case EditorMode.Warp:
                case EditorMode.Verticies:
                    Marker marker;
                    if (FindMarkerAt(point, out marker))
                    {
                        return modifierKeys.HasFlag(Keys.Control) && marker.MarkerType == MarkerType.Vertex 
                            ? CursorFactory.GetCursor(UserCursor.RemoveVertex) 
                            : marker.Cursor;
                    }
                    Figure fig;
                    if (_selection.FindFigureAt(_layer, point, out fig))
                    {
                        if (EditorMode == EditorMode.Verticies &&
                            _selection.Contains(fig) && _selection.IsHit(_layer, point) && 
                            modifierKeys.HasFlag(Keys.Control))
                            return CursorFactory.GetCursor(UserCursor.AddVertex);
                        return CursorFactory.GetCursor(UserCursor.MoveAll);
                    }
                    return Cursors.Default;
                case EditorMode.FrameSelect:
                    return _lastMode == EditorMode.Verticies
                                          ? Cursors.Default
                                          : CursorFactory.GetCursor(UserCursor.SelectByRibbonRect);
                case EditorMode.AddLine:
                    return CursorFactory.GetCursor(UserCursor.CreatePolyline);
                case EditorMode.CreateFigure:
                    return CreateFigureCursor;
                case EditorMode.Drag:
                    return _selection.Count > 0
                               ? modifierKeys.HasFlag(Keys.Control)
                                    ? CursorFactory.GetCursor(UserCursor.DragCopy) 
                                    : CursorFactory.GetCursor(UserCursor.SizeAll)
                               : _lastMode == EditorMode.Verticies 
                                    ? Cursors.Default 
                                    : CursorFactory.GetCursor(UserCursor.SelectByRibbonRect);
                case EditorMode.ChangeGeometry:
                    if (button == MouseButtons.Left && _lastMode == EditorMode.Verticies)
                        return CursorFactory.GetCursor(UserCursor.MoveVertex);
                    return _movedMarker.Cursor;
            }

            return Cursors.Default;
        }

         public void BringToFront()
        {
            LayerStartChanging();
            foreach (var fig in _selection)
            {
                _layer.Figures.Remove(fig);
                _layer.Figures.Add(fig);
            }
            LayerChanged();
            OnSelectedFigureChanged();
        }

         public void UpToFront()
        {
            LayerStartChanging();
            foreach (var fig in _selection)
            {
                var i = _layer.Figures.IndexOf(fig);
                if (i < _layer.Figures.Count - 1)
                {
                    _layer.Figures.Remove(fig);
                    _layer.Figures.Insert(i + 1, fig);
                }
            }
            LayerChanged();
            OnSelectedFigureChanged();
        }

        public void SendToDown()
        {
            LayerStartChanging();
            foreach (var fig in _selection)
            {
                var i = _layer.Figures.IndexOf(fig);
                if (i > 0)
                {
                    _layer.Figures.Remove(fig);
                    _layer.Figures.Insert(i - 1,fig);
                }
            }
            LayerChanged();
            OnSelectedFigureChanged();
        }

         public void SendToBack()
        {
            LayerStartChanging();
            foreach (var fig in _selection)
            {
                _layer.Figures.Remove(fig);
                _layer.Figures.Insert(0, fig);
            }
            LayerChanged();
            OnSelectedFigureChanged();
        }

        public void ConvertToPath()
        {
            if (_selection.Count(fig => fig.Geometry.AllowedOperations.HasFlag(AllowedOperations.Pathed)) == 0) return;
            LayerStartChanging();
            _selection.ConvertToPath();
            LayerChanged();
            BuildMarkers();
            UpdateMarkerPositions();
        }

        [NonSerialized]
        readonly DataFormats.Format _drawsFormat = DataFormats.GetFormat("clipboardVectorFiguresFormat");

        // Shared by all canvases in this process. A future secondary canvas can
        // paste by calling PasteBasketObjectsFromMemory() on its own controller.
        private static List<Figure> _figureMemoryClipboard = new List<Figure>();

        private static bool IsBasketObject(Figure figure)
        {
            if (figure == null || figure.Geometry == null) return false;
            var name = figure.Geometry.Name;
            return name == "Oggetto" || name == "GiocatoreAttacco" || name == "GiocatoreDifesa";
        }

        public void CutSelectedToClipboard()
        {
            if (_selection.Count == 0) return;
            var forcopy = _selection.Select(fig => fig.DeepClone()).ToList();
            var clipboardDataObject = new DataObject(_drawsFormat.Name, forcopy);
            Clipboard.SetDataObject(clipboardDataObject, false);
            LayerStartChanging();
            foreach (var fig in _selection)
                _layer.Figures.Remove(fig);
            LayerChanged();
            _selection.Clear();
            BuildMarkers();
            UpdateMarkerPositions();
            OnSelectedFigureChanged();
        }

        public void CopySelectedToClipboard()
        {
            if (_selection.Count == 0) return;
            var forcopy = _selection.Select(fig => fig.DeepClone()).ToList();
            var clipboardDataObject = new DataObject(_drawsFormat.Name, forcopy);
            Clipboard.SetDataObject(clipboardDataObject, false);
        }

        public void CopySelectedBasketObjectsToClipboard()
        {
            var figures = _selection.Where(IsBasketObject).Select(figure => figure.DeepClone()).ToList();
            if (figures.Count == 0) return;
            var clipboardDataObject = new DataObject(_drawsFormat.Name, figures);
            Clipboard.SetDataObject(clipboardDataObject, false);
        }

        public void CopySelectedBasketObjectsToMemory()
        {
            _figureMemoryClipboard = _selection.Where(IsBasketObject)
                .Select(figure => figure.DeepClone()).ToList();
        }

        public bool CanPasteBasketObjectsFromMemory
        {
            get { return _figureMemoryClipboard != null && _figureMemoryClipboard.Count > 0; }
        }

        /// <summary>Paste shared in-memory figures into this controller's canvas/layer.</summary>
        public void PasteBasketObjectsFromMemory()
        {
            if (!CanPasteBasketObjectsFromMemory) return;
            PasteFiguresAndSelect(_figureMemoryClipboard);
        }

        public bool CanPasteFromClipboard
        {
            get { return Clipboard.ContainsData(_drawsFormat.Name); }
        }

        public void PasteFromClipboardAndSelected()
        {
            if (!Clipboard.ContainsData(_drawsFormat.Name)) return;
            var clipboardRetrievedObject = Clipboard.GetDataObject();
            if (clipboardRetrievedObject == null) return;
            var pastedObject = clipboardRetrievedObject.GetData(_drawsFormat.Name) as List<Figure>;
            if (pastedObject == null) return;
            PasteFiguresAndSelect(pastedObject);
        }

        /// <summary>Paste clipboard figures centered at a destination canvas point.</summary>
        public void PasteFromClipboardAt(PointF targetCenter)
        {
            if (!Clipboard.ContainsData(_drawsFormat.Name)) return;
            var clipboardRetrievedObject = Clipboard.GetDataObject();
            if (clipboardRetrievedObject == null) return;
            var pastedObject = clipboardRetrievedObject.GetData(_drawsFormat.Name) as List<Figure>;
            if (pastedObject == null) return;
            PasteFiguresAndSelect(pastedObject, targetCenter);
        }

        /// <summary>Paste clipboard figures with their group bounds starting at the canvas origin.</summary>
        public void PasteFromClipboardAtOrigin()
        {
            if (!Clipboard.ContainsData(_drawsFormat.Name)) return;
            var clipboardRetrievedObject = Clipboard.GetDataObject();
            if (clipboardRetrievedObject == null) return;
            var pastedObject = clipboardRetrievedObject.GetData(_drawsFormat.Name) as List<Figure>;
            if (pastedObject == null) return;
            PasteFiguresAndSelect(pastedObject, PointF.Empty, true);
        }

        public void PasteBasketObjectsFromClipboard()
        {
            if (!CanPasteFromClipboard) return;
            var clipboardData = Clipboard.GetDataObject();
            var figures = clipboardData?.GetData(_drawsFormat.Name) as List<Figure>;
            if (figures == null) return;
            PasteFiguresAndSelect(figures.Where(IsBasketObject));
        }

        private void PasteFiguresAndSelect(IEnumerable<Figure> figures, PointF? targetCenter = null, bool alignTopLeft = false)
        {
            var list = new List<Figure>();
            foreach (var sourceFigure in figures)
            {
                var fig = sourceFigure.DeepClone();
                list.Add(fig);
            }
            if (targetCenter.HasValue && list.Count > 0)
            {
                RectangleF bounds = RectangleF.Empty;
                var hasBounds = false;
                foreach (var figure in list)
                {
                    using (var path = figure.GetTransformedPath().Path)
                    {
                        var figureBounds = path.GetBounds();
                        bounds = hasBounds ? RectangleF.Union(bounds, figureBounds) : figureBounds;
                        hasBounds = true;
                    }
                }
                if (hasBounds)
                {
                    var offsetX = targetCenter.Value.X - (alignTopLeft ? bounds.Left : bounds.Left + bounds.Width / 2f);
                    var offsetY = targetCenter.Value.Y - (alignTopLeft ? bounds.Top : bounds.Top + bounds.Height / 2f);
                    foreach (var figure in list)
                        figure.Transform.Matrix.Translate(offsetX, offsetY);
                }
            }
            LayerStartChanging();
            foreach (var figure in list)
                _layer.Figures.Add(figure);
            LayerChanged();
            _selection.Clear();
            foreach (var fig in list)
                _selection.Add(fig);
            BuildMarkers();
            UpdateMarkerPositions();
            OnSelectedFigureChanged();
        }

        public void SelectAllFigures()
        {
            foreach (var fig in _layer.Figures)
            {
                if (!_layer.IsVisible(fig) || _layer.IsLocked(fig)) continue;
                _selection.Add(fig);
            }
            BuildMarkers();
            UpdateMarkerPositions();
            OnSelectedFigureChanged();
        }

   
        public void MoveByKeys(float offsetX, float offsetY, Keys modifierKeys)
        {
            _selection.Translate(offsetX, offsetY);
            FixFiguresTranslation(modifierKeys);
            BuildMarkers();
            UpdateMarkerPositions();
            var selrect = Rectangle.Ceiling(_selection.GetTransformedPath().Path.GetBounds());
            var angle = Helper.GetAngle(_selection.Transform);
            OnSelectedRangeChanging(selrect, angle);
        }

    }
}
