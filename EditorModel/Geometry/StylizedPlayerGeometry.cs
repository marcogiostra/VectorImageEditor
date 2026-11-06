using EditorModel.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EditorModel.Geometry
{
    [Serializable]
    public sealed class StylizedPlayerGeometry : Geometry, IDisposable
    {
        private readonly SerializableGraphicsPath _jerseyPath;
        private readonly SerializableGraphicsPath _headPath;
        private readonly SerializableGraphicsPath _path;
        private readonly AllowedOperations _allowedOperations = AllowedOperations.Select | AllowedOperations.Rotate;

        public override SerializableGraphicsPath Path { get { return _path; } }
        public override AllowedOperations AllowedOperations { get { return _allowedOperations; } }
        public SerializableGraphicsPath JerseyPath { get { return _jerseyPath; } }
        public SerializableGraphicsPath HeadPath { get { return _headPath; } }

        public StylizedPlayerGeometry()
        {
            Name = "GiocatoreStilizzato";
            Title = Name;
            _jerseyPath = new SerializableGraphicsPath();
            _headPath = new SerializableGraphicsPath();
            _path = new SerializableGraphicsPath();

            // Head and body are composed horizontally, following the supplied arms-out player icon.
            _headPath.Path.AddEllipse(new RectangleF(-0.105f, -0.16f, 0.21f, 0.21f));

            // Broad, low arms-out silhouette modeled on the supplied 47x15 reference.
            // Matching tangents keep the front and back contours continuous.
            var p = _jerseyPath.Path;
            p.StartFigure();
            p.AddBezier(-0.10f, -0.045f, -0.20f, -0.045f, -0.38f, -0.14f, -0.47f, -0.14f);
            p.AddBezier(-0.47f, -0.14f, -0.51f, -0.14f, -0.51f, -0.08f, -0.47f, -0.08f);
            p.AddBezier(-0.47f, -0.08f, -0.38f, -0.08f, -0.24f, 0.15f, -0.10f, 0.15f);
            p.AddLine(-0.10f, 0.15f, 0.10f, 0.15f);
            p.AddBezier(0.10f, 0.15f, 0.24f, 0.15f, 0.38f, -0.08f, 0.47f, -0.08f);
            p.AddBezier(0.47f, -0.08f, 0.51f, -0.08f, 0.51f, -0.14f, 0.47f, -0.14f);
            p.AddBezier(0.47f, -0.14f, 0.38f, -0.14f, 0.20f, -0.045f, 0.10f, -0.045f);
            p.AddLine(0.10f, -0.045f, -0.10f, -0.045f);
            p.CloseFigure();

            _path.Path.AddPath(_jerseyPath.Path, false);
            _path.Path.AddPath(_headPath.Path, false);
            using (var initialSize = new Matrix())
            {
                initialSize.Scale(46f, 46f);
                _jerseyPath.Path.Transform(initialSize);
                _headPath.Path.Transform(initialSize);
                _path.Path.Transform(initialSize);
            }
        }

        public void Dispose()
        {
            _jerseyPath.Dispose();
            _headPath.Dispose();
            _path.Dispose();
        }
    }
}
