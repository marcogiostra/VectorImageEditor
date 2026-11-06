using EditorModel.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EditorModel.Geometry.GeometryBasket
{
    public enum OggettoBasketKind
    {
        Cono = 0,
        Coach = 1,
        Assistente = 2,
        CoachConPalla = 3,
        AssistenteConPalla = 4,
        RotazioneAntioraria = 5,
        RotazioneOraria = 6,
        ConoPiccolo = 7
    }

    /// <summary>
    /// GEOMETRY: oggetto basket
    /// </summary>
    [Serializable]
    public sealed class OggettoBasketGeometry : Geometry, IDisposable
    {
        private const float ConeHeight = 22f;
        private readonly SerializableGraphicsPath _path = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _glyphPath = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _arrowPath = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _arrowHeadPath = new SerializableGraphicsPath();
        private readonly AllowedOperations _allowedOperations = AllowedOperations.Select;
        private OggettoBasketKind _kind;

        public override SerializableGraphicsPath Path { get { return _path; } }
        public override AllowedOperations AllowedOperations { get { return _allowedOperations; } }
        public SerializableGraphicsPath GlyphPath { get { return _glyphPath; } }
        public SerializableGraphicsPath ArrowPath { get { return _arrowPath; } }
        public SerializableGraphicsPath ArrowHeadPath { get { return _arrowHeadPath; } }

        public OggettoBasketKind Kind
        {
            get { return _kind; }
            set
            {
                if (_kind == value) return;
                _kind = value;
                Title = GetKindTitle(value);
                RebuildPath();
            }
        }

        public OggettoBasketGeometry()
        {
            Name = "OggettoBasket";
            _kind = OggettoBasketKind.Cono;
            Title = GetKindTitle(_kind);
            RebuildPath();
        }

        private static string GetKindTitle(OggettoBasketKind kind)
        {
            switch (kind)
            {
                case OggettoBasketKind.Cono: return "Cono";
                case OggettoBasketKind.Coach: return "Coach";
                case OggettoBasketKind.Assistente: return "Assistente";
                case OggettoBasketKind.CoachConPalla: return "Coach con palla";
                case OggettoBasketKind.AssistenteConPalla: return "Assistente con palla";
                case OggettoBasketKind.RotazioneAntioraria: return "Rotazione antioraria";
                case OggettoBasketKind.RotazioneOraria: return "Rotazione oraria";
                case OggettoBasketKind.ConoPiccolo: return "ConoPiccolo";
                default: return "OggettoBasket";
            }
        }

        private void RebuildPath()
        {
            _path.Path.Reset();
            _glyphPath.Path.Reset();
            _arrowPath.Path.Reset();
            _arrowHeadPath.Path.Reset();
            if (_kind == OggettoBasketKind.Cono)
            {
                BuildCone(1f, 1f);
            }
            else if (_kind == OggettoBasketKind.Coach)
                BuildCoachPath(_path.Path, 1f);
            else if (_kind == OggettoBasketKind.Assistente)
                BuildAssistantPath(_path.Path, 1f);
            else if (_kind == OggettoBasketKind.CoachConPalla)
            {
                _path.Path.AddEllipse(-23f, -23f, 46f, 46f);
                BuildCoachPath(_glyphPath.Path, 0.68f);
            }
            else if (_kind == OggettoBasketKind.AssistenteConPalla)
            {
                _path.Path.AddEllipse(-23f, -23f, 46f, 46f);
                BuildAssistantPath(_glyphPath.Path, 0.68f);
            }
            else if (_kind == OggettoBasketKind.RotazioneAntioraria || _kind == OggettoBasketKind.RotazioneOraria)
            {
                BuildRotationLetter();
                BuildRotationArrow(_kind == OggettoBasketKind.RotazioneOraria);
                _path.Path.AddPath(_glyphPath.Path, false);
                _path.Path.AddPath(_arrowPath.Path, false);
                _path.Path.AddPath(_arrowHeadPath.Path, false);
            }
            else if (_kind == OggettoBasketKind.ConoPiccolo)
                BuildCone(1f, 0.5f);
        }

        private void BuildCone(float scaleX, float scaleY)
        {
            // Triangolo isoscele stretto, con la punta appena arrotondata.
            var path = _path.Path;
            path.StartFigure();
            path.AddBezier(ScalePoint(-6f, 10f, scaleX, scaleY), ScalePoint(-4f, 4f, scaleX, scaleY),
                ScalePoint(-1f, -8f, scaleX, scaleY), ScalePoint(0f, -12f, scaleX, scaleY));
            path.AddBezier(ScalePoint(0f, -12f, scaleX, scaleY), ScalePoint(1f, -8f, scaleX, scaleY),
                ScalePoint(4f, 4f, scaleX, scaleY), ScalePoint(6f, 10f, scaleX, scaleY));
            path.AddLine(ScalePoint(6f, 10f, scaleX, scaleY), ScalePoint(-6f, 10f, scaleX, scaleY));
            path.CloseFigure();

            // Le due taglie restano centrate; ConoPiccolo dimezza solo l'altezza.
            using (var center = new Matrix(1f, 0f, 0f, 1f, 0f, scaleY * (ConeHeight / 2f - 10f)))
                path.Transform(center);
        }

        private static PointF ScalePoint(float x, float y, float scaleX, float scaleY)
        {
            return new PointF(x * scaleX, y * scaleY);
        }

        private static PointF ScalePoint(float x, float y, float scale)
        {
            return ScalePoint(x, y, scale, scale);
        }

        private void BuildRotationLetter()
        {
            var glyph = _glyphPath.Path;
            glyph.FillMode = FillMode.Alternate;
            glyph.StartFigure();
            glyph.AddPolygon(new[]
            {
                new PointF(-8f, -11f), new PointF(2f, -11f), new PointF(7f, -9f),
                new PointF(9f, -6f), new PointF(9f, -2f), new PointF(7f, 1f),
                new PointF(4f, 3f), new PointF(11f, 11f), new PointF(5f, 11f),
                new PointF(-1f, 4f), new PointF(-4f, 4f), new PointF(-4f, 11f),
                new PointF(-8f, 11f)
            });
            glyph.StartFigure();
            glyph.AddPolygon(new[]
            {
                new PointF(-4f, -7f), new PointF(1f, -7f), new PointF(4f, -6f),
                new PointF(5f, -4f), new PointF(5f, -2f), new PointF(3f, 0f),
                new PointF(-4f, 0f)
            });
        }

        private void BuildRotationArrow(bool clockwise)
        {
            var arrow = _arrowPath.Path;
            arrow.StartFigure();
            if (clockwise)
            {
                arrow.AddArc(new RectangleF(-17f, -17f, 34f, 34f), 135f, 270f);
                _arrowHeadPath.Path.AddPolygon(new[]
                {
                    new PointF(12f, 12f), new PointF(13.5f, 6.5f), new PointF(18f, 10.5f)
                });
            }
            else
            {
                arrow.AddArc(new RectangleF(-17f, -17f, 34f, 34f), 45f, -270f);
                _arrowHeadPath.Path.AddPolygon(new[]
                {
                    new PointF(-12f, 12f), new PointF(-18f, 10.5f), new PointF(-13.5f, 6.5f)
                });
            }
        }

        private static void BuildCoachPath(GraphicsPath path, float scale)
        {
            // C maiuscola piena, alta circa 32 unità; nel cerchio viene ridotta.
            path.FillMode = FillMode.Alternate;
            path.StartFigure();
            path.AddBezier(ScalePoint(10f, -14f, scale), ScalePoint(2f, -18f, scale), ScalePoint(-10f, -16f, scale), ScalePoint(-14f, -8f, scale));
            path.AddBezier(ScalePoint(-14f, -8f, scale), ScalePoint(-17f, -3f, scale), ScalePoint(-17f, 3f, scale), ScalePoint(-14f, 8f, scale));
            path.AddBezier(ScalePoint(-14f, 8f, scale), ScalePoint(-10f, 16f, scale), ScalePoint(2f, 18f, scale), ScalePoint(10f, 14f, scale));
            path.AddLine(ScalePoint(10f, 14f, scale), ScalePoint(7f, 8f, scale));
            path.AddBezier(ScalePoint(7f, 8f, scale), ScalePoint(1f, 11f, scale), ScalePoint(-7f, 9f, scale), ScalePoint(-8f, 4f, scale));
            path.AddBezier(ScalePoint(-8f, 4f, scale), ScalePoint(-10f, 1f, scale), ScalePoint(-10f, -1f, scale), ScalePoint(-8f, -4f, scale));
            path.AddBezier(ScalePoint(-8f, -4f, scale), ScalePoint(-7f, -9f, scale), ScalePoint(1f, -11f, scale), ScalePoint(7f, -8f, scale));
            path.AddLine(ScalePoint(7f, -8f, scale), ScalePoint(10f, -14f, scale));
            path.CloseFigure();
        }

        private static void BuildAssistantPath(GraphicsPath path, float scale)
        {
            // A maiuscola con gambe rastremate e controforma interna.
            path.FillMode = FillMode.Alternate;
            path.StartFigure();
            path.AddLine(ScalePoint(-13f, 16f, scale), ScalePoint(-4f, -12f, scale));
            path.AddBezier(ScalePoint(-4f, -12f, scale), ScalePoint(-3f, -15f, scale), ScalePoint(-1f, -16f, scale), ScalePoint(0f, -16f, scale));
            path.AddBezier(ScalePoint(0f, -16f, scale), ScalePoint(2f, -16f, scale), ScalePoint(4f, -14f, scale), ScalePoint(5f, -11f, scale));
            path.AddLine(ScalePoint(5f, -11f, scale), ScalePoint(13f, 16f, scale));
            path.AddLine(ScalePoint(13f, 16f, scale), ScalePoint(7f, 16f, scale));
            path.AddLine(ScalePoint(7f, 16f, scale), ScalePoint(4f, 5f, scale));
            path.AddLine(ScalePoint(4f, 5f, scale), ScalePoint(-4f, 5f, scale));
            path.AddLine(ScalePoint(-4f, 5f, scale), ScalePoint(-7f, 16f, scale));
            path.CloseFigure();

            path.StartFigure();
            path.AddPolygon(new[]
            {
                ScalePoint(-3f, 0f, scale), ScalePoint(-1f, -9f, scale),
                ScalePoint(1f, -9f, scale), ScalePoint(3f, 0f, scale)
            });
        }

        public override string ToString()
        {
            return "OggettoBasket";
        }

        public void Dispose()
        {
            _path.Dispose();
            _glyphPath.Dispose();
            _arrowPath.Dispose();
            _arrowHeadPath.Dispose();
        }
    }
}
