using EditorModel.Common;
using EditorModel.Figures;
using EditorModel.Figures.Extentions;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EditorModel.Geometry
{
    [Serializable]
    public sealed class BezierGeometry : Geometry, IDisposable, ITransformedGeometry
    {
        private PointF[] _points;
        private byte[] _types;

        private readonly SerializableGraphicsPath _path = new SerializableGraphicsPath();

        private readonly AllowedOperations _allowedOperations;

        internal BezierGeometry(PointF[] points, byte[] types)
        {
            _allowedOperations = AllowedOperations.All ^ AllowedOperations.Pathed;
            Points = points;
            Types = types;
        }

        public PointF[] Points
        {
            get { return _points; }
            set { _points = value; }
        }

    
        public byte[] Types
        {
            get { return _types; }
            set { _types = value; }
        }

     
        public PointF[] GetTransformedPoints(Figure owner)
        {
            var points = (PointF[])Points.Clone();
            owner.Transform.Matrix.TransformPoints(points);
            return points;
        }

        public byte[] GetTransformedPointTypes(Figure owner)
        {
            return (byte[])_types.Clone();
        }

        public bool IsFlatten { get; set; }

        public float Flatness { get; set; } = 0.25f;

        public bool IsClosed { get; set; }

        public BasketPolylineKind BasketKind { get; set; }

        public void SetTransformedPoints(Figure owner, PointF[] points)
        {
            points = (PointF[])points.Clone();
            var m = owner.Transform.Matrix.Clone();
            m.Invert();
            m.TransformPoints(points);
            Points = points;
        }

        
        public override SerializableGraphicsPath Path
        {
            get
            {
                var types = (byte[])_types.Clone();
                if (IsClosed)
                    types[types.Length - 1] |= 0x80;
                else
                    types[types.Length - 1] &= 0x7f;
                _path.Path = new GraphicsPath(_points, types);
                if (BasketKind == BasketPolylineKind.PalleggioSinusoidale)
                {
                    using (var baseline = (GraphicsPath)_path.Path.Clone())
                        _path.Path.AddSinuousCurve(baseline);
                }
                else if (BasketKind == BasketPolylineKind.Blocco)
                {
                    using (var flattened = (GraphicsPath)_path.Path.Clone())
                    {
                        flattened.Flatten(new Matrix(), 0.5f);
                        var points = flattened.PathPoints;
                        if (points.Length >= 2)
                        {
                            var previous = points[points.Length - 2];
                            var endpoint = points[points.Length - 1];
                            float dx = endpoint.X - previous.X;
                            float dy = endpoint.Y - previous.Y;
                            float length = (float)Math.Sqrt(dx * dx + dy * dy);
                            if (length > 0.0001f)
                            {
                                float nx = -dy / length * 6f;
                                float ny = dx / length * 6f;
                                _path.Path.StartFigure();
                                _path.Path.AddLine(endpoint.X - nx, endpoint.Y - ny, endpoint.X + nx, endpoint.Y + ny);
                            }
                        }
                    }
                }
                 if (IsFlatten)
                    _path.Path.Flatten(new Matrix(), Flatness);
                return _path;
            }
        }

       
        public override AllowedOperations AllowedOperations { get { return _allowedOperations; } }

        public override string ToString()
        {
            return "Bezier";
        }

        public void Dispose()
        {
            _path?.Dispose();
        }
    }
}
