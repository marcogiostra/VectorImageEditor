using EditorModel.Figures;
using Svg;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Windows.Forms;

namespace EditorModel.Renderers
{
   

    [Serializable]
    public sealed class MioSvgRenderer : Renderer
    {
        /// <summary>
        /// Il documento SVG caricato. Se null, mostriamo un placeholder.
        /// (SvgDocument non è serializzabile di default: se ti serve la persistenza
        ///  salva il testo SVG in una stringa e ricostruisci on demand.)
        /// </summary>
        [NonSerialized]
        private SvgDocument _document;

        public string SvgSource { get; set; }   // XML originale, per la serializzazione

        public SvgDocument Document
        {
            get
            {
                if (_document == null && !string.IsNullOrEmpty(SvgSource))
                    _document = SvgDocument.FromSvg<SvgDocument>(SvgSource);
                return _document;
            }
            set
            {
                _document = value;
                SvgSource = value?.GetXML(); // memorizza l'XML per la serializzazione
            }
        }

        public MioSvgRenderer() { }

        public MioSvgRenderer(SvgDocument doc)
        {
            Document = doc;
        }


        public override void Render(Graphics graphics, Figure figure)
        {
            var doc = Document;

            if (doc == null)
            {
                using (var path = figure.GetTransformedPath().Path)
                {
                    var bounds = path.GetBounds();

                    using (var pen = new Pen(Color.Black, 0f)
                    {
                        DashStyle = DashStyle.Dot
                    })
                    {
                        graphics.DrawRectangles(pen, new[] { bounds });
                    }

                    using (var sf = new StringFormat(StringFormat.GenericTypographic))
                    {
                        sf.Alignment = StringAlignment.Center;
                        sf.LineAlignment = StringAlignment.Center;

                        graphics.DrawString(
                            "SVG Place Holder",
                            SystemFonts.DefaultFont,
                            Brushes.Black,
                            bounds,
                            sf);
                    }
                }

                RenderPlayerNumberAnnotation(graphics, figure);
                return;
            }

            // La figura rappresenta un quadrato unitario.
            PointF[] dst =
            {
        new PointF(-0.5f, -0.5f),
        new PointF( 0.5f, -0.5f),
        new PointF(-0.5f,  0.5f)
    };

            figure.Transform.Matrix.TransformPoints(dst);

            var m = new Matrix(
                dst[1].X - dst[0].X,
                dst[1].Y - dst[0].Y,
                dst[2].X - dst[0].X,
                dst[2].Y - dst[0].Y,
                dst[0].X,
                dst[0].Y);

            var state = graphics.Save();

            try
            {
                graphics.MultiplyTransform(m);

                // =====================================================
                // USA IL VIEWBOX, NON Width/Height
                // =====================================================

                var viewBox = doc.ViewBox;

                float vbX = (float)viewBox.MinX;
                float vbY = (float)viewBox.MinY;
                float vbWidth = (float)viewBox.Width;
                float vbHeight = (float)viewBox.Height;

                if (vbWidth <= 0 || vbHeight <= 0)
                    return;

                // Porta il viewBox a 0,0
                graphics.TranslateTransform(-vbX, -vbY);

                // Normalizza il viewBox nel quadrato unitario
                graphics.ScaleTransform(
                    1.0f / vbWidth,
                    1.0f / vbHeight);

                doc.Draw(graphics);
            }
            finally
            {
                graphics.Restore(state);
            }

            RenderPlayerNumberAnnotation(graphics, figure);
        }

        public static void RenderPlayerNumberAnnotation(Graphics graphics, Figure figure)
        {
            var annotation = figure.PlayerNumber;
            if (annotation == null || !annotation.IsVisible || string.IsNullOrWhiteSpace(annotation.Text))
                return;

            var localBounds = figure.Geometry.Path.Path.GetBounds();
            PointF anchor;
            switch (annotation.Position)
            {
                case PlayerNumberPosition.NE: anchor = new PointF(0.58f, -0.58f); break;
                case PlayerNumberPosition.E: anchor = new PointF(0.68f, 0f); break;
                case PlayerNumberPosition.SE: anchor = new PointF(0.58f, 0.58f); break;
                case PlayerNumberPosition.S: anchor = new PointF(0f, 0.68f); break;
                case PlayerNumberPosition.SW: anchor = new PointF(-0.58f, 0.58f); break;
                case PlayerNumberPosition.W: anchor = new PointF(-0.68f, 0f); break;
                case PlayerNumberPosition.NW: anchor = new PointF(-0.58f, -0.58f); break;
                default: anchor = new PointF(0f, -0.68f); break;
            }

            anchor = new PointF(localBounds.Left + localBounds.Width / 2f + anchor.X * localBounds.Width,
                localBounds.Top + localBounds.Height / 2f + anchor.Y * localBounds.Height);

            var anchorPoint = new[] { anchor };
            figure.Transform.Matrix.TransformPoints(anchorPoint);
            anchor = anchorPoint[0];
            var matrix = figure.Transform.Matrix.Elements;
            var horizontalScale = (float)Math.Sqrt(matrix[0] * matrix[0] + matrix[1] * matrix[1]);
            var verticalScale = (float)Math.Sqrt(matrix[2] * matrix[2] + matrix[3] * matrix[3]);
            var fontScale = annotation.FontScale > 0 ? annotation.FontScale : 0.2f;
            var fontSize = Math.Max(1f, Math.Min(horizontalScale * localBounds.Width, verticalScale * localBounds.Height) * fontScale);

            using (var font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold, GraphicsUnit.World))
            using (var brush = new SolidBrush(annotation.Color))
            using (var format = new StringFormat(StringFormat.GenericTypographic)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                var size = graphics.MeasureString(annotation.Text, font);
                var textBounds = new RectangleF(anchor.X - size.Width / 2f, anchor.Y - size.Height / 2f,
                    size.Width, size.Height);
                graphics.DrawString(annotation.Text, font, brush, textBounds, format);
            }
        }
 
        public override AllowedRendererDecorators AllowedDecorators
        {
            get
            {
                return AllowedRendererDecorators.All ^
                       (AllowedRendererDecorators.Shadow |
                        AllowedRendererDecorators.Glow |
                        AllowedRendererDecorators.TextBlock);
            }
        }
    }
}
