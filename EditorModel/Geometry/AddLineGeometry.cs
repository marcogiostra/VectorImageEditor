using EditorModel.Common;
using EditorModel.Figures;
using EditorModel.Figures.Extentions;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace EditorModel.Geometry
{
    [Serializable]
    public sealed class AddLineGeometry : Geometry, IDisposable, IGeometry
    {
        public readonly List<PointF> Points = new List<PointF>();

        public PointF EndPoint { get; set; }

        public bool IsClosed { get; set; } = true;

        public bool IsSmoothed { get; set; }
        public bool IsSinusoidal { get; set; }
        public BasketPolylineKind BasketKind { get; set; }

        private readonly SerializableGraphicsPath _path = new SerializableGraphicsPath();

        public override AllowedOperations AllowedOperations { get { return AllowedOperations.None; } }

        public override SerializableGraphicsPath Path
        {
            get
            {
                _path.Path.Reset();
                if (Points.Count > 0)
                {
                    if (IsSinusoidal)
                    {
                        var list = new List<PointF>(Points);
                        if (list.Count == 0 || list[list.Count - 1] != EndPoint)
                            list.Add(EndPoint);
                        if (list.Count > 1)
                            _path.Path.AddSinuousBasketLines(list.ToArray(), IsClosed);
                    }
                    else if (IsSmoothed)
                    {
                        var list = new List<PointF>(Points);
                        if (list.Count == 0 || list[list.Count - 1] != EndPoint)
                            list.Add(EndPoint);
                        if (list.Count == 1)
                            list.Add(EndPoint);
                        if (list.Count >= 2)
                        {
                            using (var baseline = new System.Drawing.Drawing2D.GraphicsPath())
                            {
                                if (list.Count > 2)
                                    baseline.AddCurve(list.ToArray());
                                else
                                    baseline.AddLines(list.ToArray());
                                if (BasketKind == BasketPolylineKind.PalleggioSinusoidale)
                                    _path.Path.AddSinuousCurve(baseline);
                                else
                                    _path.Path.AddPath(baseline, false);
                            }
                        }
                        if (BasketKind == BasketPolylineKind.Blocco && list.Count > 1)
                            AddBlockMark(_path.Path, list[list.Count - 2], list[list.Count - 1]);
                    }
                    else
                    {
                        var linePoints = new List<PointF>(Points);
                        if (linePoints.Count == 0 || linePoints[linePoints.Count - 1] != EndPoint)
                            linePoints.Add(EndPoint);
                        if (linePoints.Count > 1)
                            _path.Path.AddLines(linePoints.ToArray());
                        if (IsClosed)
                            _path.Path.CloseFigure();
                        else if (BasketKind == BasketPolylineKind.Blocco && linePoints.Count > 1)
                            AddBlockMark(_path.Path, linePoints[linePoints.Count - 2], linePoints[linePoints.Count - 1]);
                    }
                }
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

        internal AddLineGeometry(PointF startPoint)
        {
            Points.Add(startPoint);
            EndPoint = startPoint;
        }

        public void Dispose()
        {
            _path?.Dispose();
        }

        public void AddPoint(Point point)
        {
            if (!Points.Contains(point))
                Points.Add(point);
            EndPoint = point;
        }
    }
}
