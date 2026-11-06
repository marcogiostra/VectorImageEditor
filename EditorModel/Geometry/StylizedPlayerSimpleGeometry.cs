using EditorModel.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EditorModel.Geometry
{
    [Serializable]
    public sealed class StylizedPlayerSimpleGeometry : Geometry, IDisposable
    {
        private readonly SerializableGraphicsPath _armsPath;
        private readonly SerializableGraphicsPath _headPath;
        private readonly SerializableGraphicsPath _path;
        private readonly AllowedOperations _allowedOperations = AllowedOperations.Select | AllowedOperations.Rotate;

        public override SerializableGraphicsPath Path { get { return _path; } }
        public override AllowedOperations AllowedOperations { get { return _allowedOperations; } }
        public SerializableGraphicsPath ArmsPath { get { return _armsPath; } }
        public SerializableGraphicsPath HeadPath { get { return _headPath; } }

        public StylizedPlayerSimpleGeometry()
        {
            Name = "GiocatoreStilizzatoSemplice";
            Title = Name;
            _armsPath = new SerializableGraphicsPath();
            _headPath = new SerializableGraphicsPath();
            _path = new SerializableGraphicsPath();

            // Continuous silhouette: left arm, rounded bottom arc, right arm.
            // The arms rise equally, leaving the V open toward the top; the head
            // is centered above the arc and rests on its lower edge.
            _armsPath.Path.StartFigure();
            _armsPath.Path.AddLine(-0.50f, -0.16f, -0.10f, 0.075f);
            _armsPath.Path.AddBezier(-0.10f, 0.075f, -0.055f, 0.145f, 0.055f, 0.145f, 0.10f, 0.075f);
            _armsPath.Path.AddLine(0.10f, 0.075f, 0.50f, -0.16f);
            _headPath.Path.AddEllipse(new RectangleF(-0.105f, -0.135f, 0.21f, 0.21f));
            _path.Path.AddPath(_armsPath.Path, false);
            _path.Path.AddPath(_headPath.Path, false);



            using (var initialSize = new Matrix())
            {
                initialSize.Scale(40f, 40f);
                _armsPath.Path.Transform(initialSize);
                _headPath.Path.Transform(initialSize);
                _path.Path.Transform(initialSize);
            }
        }

        public void Dispose()
        {
            _armsPath.Dispose();
            _headPath.Dispose();
            _path.Dispose();
        }
    }
}
