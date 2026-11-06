using EditorModel.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EditorModel.Geometry.GeometryBasket
{
    public enum GiocatorePose
    {
        GiocatoreAttacco = 0,
        GiocatoreCheBlocca = 1,
        GiocatoreAnticipo = 2,
        GiocatorePalleggio = 3,
        GiocatoreTiro = 4,
        GiocatoreRimbalzo = 5,
        GiocatoreStance = 6,
        GiocatoreBlocco = 7
    }

    [Serializable]
    public sealed class GiocatoreGeometry : Geometry, IDisposable
    {
        private const float SourceWidth = 58f;
        private const float SourceHeight = 40f;
        private const float TargetWidth = 46f;
        private readonly SerializableGraphicsPath _leftArmFill = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _rightArmFill = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _bodyFill = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _headFill = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _leftArmBorder = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _rightArmBorder = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _bodyBorder = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _headBorder = new SerializableGraphicsPath();
        private readonly SerializableGraphicsPath _path = new SerializableGraphicsPath();
        private readonly AllowedOperations _allowedOperations = AllowedOperations.Select;
        private int _headColorArgb = Color.White.ToArgb();
        private int _bodyColorArgb = Color.White.ToArgb();
        private int _armsColorArgb = Color.White.ToArgb();
        private GiocatorePose _pose;

        public override SerializableGraphicsPath Path { get { return _path; } }
        public override AllowedOperations AllowedOperations { get { return _allowedOperations; } }
        public SerializableGraphicsPath LeftArmFill { get { return _leftArmFill; } }
        public SerializableGraphicsPath RightArmFill { get { return _rightArmFill; } }
        public SerializableGraphicsPath BodyFill { get { return _bodyFill; } }
        public SerializableGraphicsPath HeadFill { get { return _headFill; } }
        public SerializableGraphicsPath LeftArmBorder { get { return _leftArmBorder; } }
        public SerializableGraphicsPath RightArmBorder { get { return _rightArmBorder; } }
        public SerializableGraphicsPath BodyBorder { get { return _bodyBorder; } }
        public SerializableGraphicsPath HeadBorder { get { return _headBorder; } }

        public Color HeadColor
        {
            get { return Color.FromArgb(_headColorArgb); }
            set { _headColorArgb = value.ToArgb(); }
        }

        public Color BodyColor
        {
            get { return Color.FromArgb(_bodyColorArgb); }
            set { _bodyColorArgb = value.ToArgb(); }
        }

        public Color ArmsColor
        {
            get { return Color.FromArgb(_armsColorArgb); }
            set { _armsColorArgb = value.ToArgb(); }
        }

        public GiocatorePose Pose
        {
            get { return _pose; }
            set
            {
                if (_pose == value) return;
                _pose = value;
                RebuildPose();
            }
        }

        public GiocatoreGeometry()
        {
            Name = "Giocatore";
            Title = Name;
            _pose = GiocatorePose.GiocatoreAttacco;
            RebuildPose();
        }

        private void RebuildPose()
        {
            _leftArmFill.Path.Reset(); _rightArmFill.Path.Reset();
            _bodyFill.Path.Reset(); _headFill.Path.Reset();
            _leftArmBorder.Path.Reset(); _rightArmBorder.Path.Reset();
            _bodyBorder.Path.Reset(); _headBorder.Path.Reset(); _path.Path.Reset();

            if (_pose == GiocatorePose.GiocatoreCheBlocca)
            {
                AddOpenDefenseArm(_rightArmFill.Path, false);
                AddOpenDefenseArm(_leftArmFill.Path, true);
            }
            else if (_pose == GiocatorePose.GiocatoreBlocco)
            {
                AddBlockingArm(_rightArmFill.Path, false);
                AddBlockingArm(_leftArmFill.Path, true);
            }
            else if (_pose == GiocatorePose.GiocatoreRimbalzo)
            {
                AddReboundArm(_rightArmFill.Path, false);
                AddReboundArm(_leftArmFill.Path, true);
            }
            else if (_pose == GiocatorePose.GiocatoreAnticipo)
            {
                AddExtendedRightArm(_rightArmFill.Path);
                AddArm(_leftArmFill.Path, true);
            }
            else if (_pose == GiocatorePose.GiocatorePalleggio || _pose == GiocatorePose.GiocatoreTiro)
            {
                AddVerticalRightArm(_rightArmFill.Path);
                AddArm(_leftArmFill.Path, true);
            }
            else if (_pose == GiocatorePose.GiocatoreStance)
            {
                AddStraightArm(_rightArmFill.Path, false);
                AddStraightArm(_leftArmFill.Path, true);
            }
            else
            {
                AddScaledArm(_rightArmFill.Path, false, 1.12f);
                AddScaledArm(_leftArmFill.Path, true, 1.12f);
            }
            _rightArmBorder.Path.AddPath(_rightArmFill.Path, false);
            _leftArmBorder.Path.AddPath(_leftArmFill.Path, false);

            BuildBodyAndHead();
            FitSourceArtboard();
        }

        private void BuildBodyAndHead()
        {
            // Il riempimento chiude solo il lato superiore, che resta senza bordo
            // visibile per conservare l'apertura tra le due spalle del PNG.
            var body = _bodyFill.Path;
            body.StartFigure();
            body.AddLine(12f, 23f, 46f, 23f);
            body.AddLine(46f, 23f, 46f, 28f);
            body.AddBezier(46f, 28f, 45f, 34f, 43f, 39f, 39f, 39f);
            body.AddLine(39f, 39f, 18f, 39f);
            body.AddBezier(18f, 39f, 14f, 39f, 12f, 34f, 11f, 28f);
            body.AddLine(11f, 28f, 12f, 23f);
            body.CloseFigure();

            var outline = _bodyBorder.Path;
            outline.StartFigure();
            outline.AddLine(12f, 23f, 18f, 23f);
            outline.StartFigure();
            outline.AddLine(46f, 23f, 46f, 28f);
            outline.AddBezier(46f, 28f, 45f, 34f, 43f, 39f, 39f, 39f);
            outline.AddLine(39f, 39f, 18f, 39f);
            outline.AddBezier(18f, 39f, 14f, 39f, 12f, 34f, 11f, 28f);
            outline.AddLine(11f, 28f, 12f, 23f);
            outline.StartFigure();
            outline.AddLine(40f, 23f, 46f, 23f);

            var head = new RectangleF(19f, 13f, 20f, 19f);
            _headFill.Path.AddEllipse(head);
            _headBorder.Path.AddEllipse(head);

            if (_pose == GiocatorePose.GiocatoreTiro)
            {
                // Nel tiro il busto avanza leggermente con il lato destro,
                // inclinando la figura e aumentando un poco la sua altezza.
                using (var tilt = new Matrix())
                {
                    tilt.RotateAt(-12f, new PointF(29f, 25f));
                    _bodyFill.Path.Transform(tilt);
                    _bodyBorder.Path.Transform(tilt);
                    _headFill.Path.Transform(tilt);
                    _headBorder.Path.Transform(tilt);
                }
            }

            _path.Path.AddPath(_leftArmFill.Path, false);
            _path.Path.AddPath(_rightArmFill.Path, false);
            _path.Path.AddPath(_bodyFill.Path, false);
            _path.Path.AddPath(_headFill.Path, false);
        }

        private static void AddArm(GraphicsPath path, bool mirror)
        {
            PointF Map(float x, float y)
            {
                return mirror ? new PointF(SourceWidth - x, y) : new PointF(x, y);
            }

            path.StartFigure();
            path.AddBezier(Map(39f, 1f), Map(44f, 0f), Map(49f, 4f), Map(52f, 8f));
            path.AddBezier(Map(52f, 8f), Map(58f, 15f), Map(56f, 26f), Map(49f, 33f));
            path.AddBezier(Map(49f, 33f), Map(47f, 35f), Map(45f, 37f), Map(43f, 37f));
            path.AddBezier(Map(43f, 37f), Map(43f, 32f), Map(47f, 26f), Map(48f, 20f));
            path.AddBezier(Map(48f, 20f), Map(50f, 13f), Map(46f, 8f), Map(40f, 5f));
            path.AddBezier(Map(40f, 5f), Map(37f, 4f), Map(37f, 2f), Map(39f, 1f));
            path.CloseFigure();
        }

        private static void AddScaledArm(GraphicsPath path, bool mirror, float horizontalScale)
        {
            AddArm(path, mirror);
            float offset = 29f * (1f - horizontalScale);
            using (var widen = new Matrix(horizontalScale, 0f, 0f, 1f, offset, 0f))
            {
                path.Transform(widen);
            }
        }

        private static void AddBlockingArm(GraphicsPath path, bool mirror)
        {
            PointF Map(float x, float y)
            {
                return mirror ? new PointF(SourceWidth - x, y) : new PointF(x, y);
            }

            // Il braccio sale lungo il fianco e piega la mano verso il viso.
            // Le mani quasi si toccano e restano staccate sopra la testa.
            path.StartFigure();
            path.AddBezier(Map(48f, 36f), Map(55f, 31f), Map(58f, 22f), Map(56f, 15f));
            path.AddBezier(Map(56f, 15f), Map(54f, 8f), Map(48f, 4f), Map(43f, 6f));
            path.AddBezier(Map(43f, 6f), Map(38f, 5f), Map(34f, 7f), Map(30f, 9f));
            path.AddBezier(Map(30f, 9f), Map(29f, 10f), Map(30f, 12f), Map(32f, 11f));
            path.AddBezier(Map(32f, 11f), Map(38f, 8f), Map(43f, 10f), Map(46f, 15f));
            path.AddBezier(Map(46f, 15f), Map(49f, 19f), Map(48f, 24f), Map(44f, 28f));
            path.AddBezier(Map(44f, 31f), Map(42f, 33f), Map(42f, 35f), Map(44f, 37f));
            path.AddBezier(Map(44f, 37f), Map(45f, 37f), Map(47f, 36f), Map(48f, 36f));
            path.CloseFigure();
        }

        private static void AddOpenDefenseArm(GraphicsPath path, bool mirror)
        {
            PointF Map(float x, float y)
            {
                return mirror ? new PointF(SourceWidth - x, y + 4f) : new PointF(x, y + 4f);
            }

            // Braccio a falce: largo sul raccordo al corpo, si assottiglia
            // e sale verso la mano all'esterno. L'intera sagoma è abbassata,
            // mantenendo arrotondata la zona di raccordo al corpo.
            path.StartFigure();
            path.AddBezier(Map(43f, 18f), Map(50f, 17f), Map(59f, 24f), Map(68f, 14f));
            path.AddBezier(Map(68f, 14f), Map(71f, 11f), Map(74f, 12f), Map(72f, 15f));
            path.AddBezier(Map(72f, 15f), Map(67f, 21f), Map(60f, 28f), Map(53f, 26f));
            path.AddBezier(Map(53f, 26f), Map(48f, 25f), Map(45f, 29f), Map(43f, 27f));
            path.AddBezier(Map(43f, 27f), Map(41f, 25f), Map(41f, 20f), Map(43f, 18f));
            path.CloseFigure();
        }

        private static void AddStraightArm(GraphicsPath path, bool mirror)
        {
            PointF Map(float x, float y)
            {
                return mirror ? new PointF(SourceWidth - x, y) : new PointF(x, y);
            }

            // Sagoma affusolata con i due lati rettilinei e la punta arrotondata.
            path.StartFigure();
            path.AddLine(Map(41f, 33f), Map(54f, 9f));
            path.AddBezier(Map(54f, 9f), Map(56f, 5f), Map(60f, 4f), Map(62f, 7f));
            path.AddBezier(Map(62f, 7f), Map(63f, 9f), Map(61f, 12f), Map(60f, 14f));
            path.AddLine(Map(60f, 14f), Map(47f, 35f));
            path.AddBezier(Map(47f, 35f), Map(45f, 38f), Map(41f, 37f), Map(41f, 33f));
            path.CloseFigure();
        }

        private static void AddReboundArm(GraphicsPath path, bool mirror)
        {
            PointF Map(float x, float y)
            {
                return mirror ? new PointF(SourceWidth - x, y) : new PointF(x, y);
            }

            // Braccio sollevato come nella difesa, con la mano aperta verso l'esterno.
            path.StartFigure();
            path.AddBezier(Map(48f, 36f), Map(55f, 31f), Map(58f, 22f), Map(56f, 15f));
            path.AddBezier(Map(56f, 15f), Map(54f, 10f), Map(51f, 7f), Map(47f, 6f));
            path.AddBezier(Map(47f, 6f), Map(50f, 4f), Map(54f, 3f), Map(57f, 1f));
            path.AddBezier(Map(57f, 1f), Map(59f, 0f), Map(60f, 2f), Map(58f, 4f));
            path.AddBezier(Map(58f, 4f), Map(57f, 6f), Map(55f, 8f), Map(53f, 10f));
            path.AddBezier(Map(53f, 10f), Map(58f, 12f), Map(60f, 16f), Map(60f, 21f));
            path.AddBezier(Map(60f, 21f), Map(60f, 28f), Map(54f, 35f), Map(47f, 38f));
            path.AddBezier(Map(47f, 38f), Map(45f, 39f), Map(43f, 37f), Map(44f, 35f));
            path.AddBezier(Map(44f, 35f), Map(45f, 31f), Map(48f, 27f), Map(49f, 22f));
            path.AddBezier(Map(49f, 22f), Map(50f, 17f), Map(48f, 13f), Map(44f, 10f));
            path.AddBezier(Map(44f, 10f), Map(42f, 8f), Map(44f, 6f), Map(47f, 6f));
            path.CloseFigure();
        }

        private static void AddExtendedRightArm(GraphicsPath path)
        {
            // Braccio destro aperto verso l'esterno, quasi disteso e più lungo
            // della sagoma standard per dare più larghezza alla posa anticipo.
            path.StartFigure();
            path.AddBezier(42f, 25f, 48f, 21f, 55f, 14f, 62f, 7f);
            path.AddBezier(62f, 7f, 64f, 5f, 66f, 4f, 67f, 5f);
            path.AddBezier(67f, 5f, 68f, 7f, 65f, 10f, 63f, 12f);
            path.AddBezier(63f, 12f, 57f, 20f, 51f, 28f, 46f, 34f);
            path.AddBezier(46f, 34f, 45f, 36f, 43f, 37f, 42f, 36f);
            path.AddBezier(42f, 36f, 41f, 33f, 42f, 29f, 42f, 25f);
            path.CloseFigure();
        }

        private static void AddVerticalRightArm(GraphicsPath path)
        {
            // Braccio destro sollevato verticalmente, con estremità arrotondata.
            path.StartFigure();
            path.AddBezier(42f, 25f, 45f, 20f, 47f, 13f, 47f, 5f);
            path.AddBezier(47f, 5f, 47f, 2f, 48f, 0f, 50f, 0f);
            path.AddBezier(50f, 0f, 52f, 0f, 53f, 2f, 53f, 5f);
            path.AddBezier(53f, 5f, 54f, 14f, 51f, 24f, 47f, 34f);
            path.AddBezier(47f, 34f, 46f, 36f, 44f, 37f, 42f, 36f);
            path.AddBezier(42f, 36f, 41f, 33f, 42f, 29f, 42f, 25f);
            path.CloseFigure();
        }

        private void FitSourceArtboard()
        {
            float scale = TargetWidth / SourceWidth;
            using (var fit = new Matrix(scale, 0f, 0f, scale, -SourceWidth * scale / 2f, -SourceHeight * scale / 2f))
            {
                _leftArmFill.Path.Transform(fit);
                _rightArmFill.Path.Transform(fit);
                _bodyFill.Path.Transform(fit);
                _headFill.Path.Transform(fit);
                _leftArmBorder.Path.Transform(fit);
                _rightArmBorder.Path.Transform(fit);
                _bodyBorder.Path.Transform(fit);
                _headBorder.Path.Transform(fit);
                _path.Path.Transform(fit);
            }
        }

        public void Dispose()
        {
            _leftArmFill.Dispose(); _rightArmFill.Dispose(); _bodyFill.Dispose(); _headFill.Dispose();
            _leftArmBorder.Dispose(); _rightArmBorder.Dispose(); _bodyBorder.Dispose(); _headBorder.Dispose(); _path.Dispose();
        }

        public override string ToString()
        {
            return "Giocatore";
        }
    }
}
