using EditorModel.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EditorModel.Geometry.GeometryBasket
{
    /// <summary>
    /// GEOMETRY: pallone basket
    /// </summary>
    [Serializable]
    public sealed class BallBasketGeometry : Geometry, IDisposable
    {
        private const float Diameter = 16f;
        private readonly SerializableGraphicsPath _path = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _seams = new SerializableGraphicsPath();
        private readonly AllowedOperations _allowedOperations = AllowedOperations.Select;

        public override SerializableGraphicsPath Path { get { return _path; } }
        public override AllowedOperations AllowedOperations { get { return _allowedOperations; } }
        public SerializableGraphicsPath Seams { get { return _seams; } }

        public BallBasketGeometry()
        {
            Name = "BallBasket";
            Title = Name;
            _path.Path.AddEllipse(-Diameter / 2f, -Diameter / 2f, Diameter, Diameter);

            // Due cuciture a S, verticale e orizzontale, incrociate al centro
            // per formare pannelli curvi leggibili anche alla dimensione dell'icona.
            _seams.Path.StartFigure();
            _seams.Path.AddBezier(0f, -6.8f, 4f, -4f, -4f, 4f, 0f, 6.8f);
            _seams.Path.StartFigure();
            _seams.Path.AddBezier(-6.8f, 0f, -4f, -4f, 4f, 4f, 6.8f, 0f);
        }

        public override string ToString()
        {
            return "BallBasket";
        }

        public void Dispose()
        {
            _path.Dispose();
            _seams.Dispose();
        }
    }
}
