using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using EditorModel.Figures;
using EditorModel.Geometry;
using EditorModel.Geometry.GeometryBasket;

namespace EditorModel.Renderers
{
    [Serializable]
    public sealed class RoundAttackPlayerRenderer : Renderer
    {
        public override void Render(Graphics graphics, Figure figure)
        {
            new DefaultRenderer().Render(graphics, figure);
            var number = figure.PlayerNumber;
            if (number == null || !number.IsVisible || string.IsNullOrWhiteSpace(number.Text)) return;
            using (var path = figure.GetTransformedPath().Path)
            {
                var bounds = path.GetBounds();
                var fontSize = Math.Max(1f, Math.Min(bounds.Width, bounds.Height) * (number.FontScale > 0 ? number.FontScale : 0.45f));
                var center = new[] { new PointF(bounds.Left + bounds.Width / 2f, bounds.Top + bounds.Height / 2f) };
                if (figure.Geometry.Name == "GiocatoreDifesaTriangolare")
                    center[0] = new PointF(bounds.Left + bounds.Width / 2f, bounds.Top + bounds.Height * (2f / 3f));
            using (var font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold, GraphicsUnit.World))
            using (var brush = new SolidBrush(number.Color))
            using (var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center })
                if (figure.Geometry.Name == "GiocatoreDifesaTriangolare")
                {
                    var textSize = graphics.MeasureString(number.Text, font);
                    var textBounds = new RectangleF(center[0].X - textSize.Width / 2f, center[0].Y - textSize.Height / 2f, textSize.Width, textSize.Height);
                    graphics.DrawString(number.Text, font, brush, textBounds, format);
                }
                else graphics.DrawString(number.Text, font, brush, bounds, format);
            }
        }

        public override AllowedRendererDecorators AllowedDecorators { get { return AllowedRendererDecorators.None; } }
    }

    [Serializable]
    public sealed class StylizedPlayerRenderer : Renderer
    {
        public override void Render(Graphics graphics, Figure figure)
        {
            var geometry = figure.Geometry as StylizedPlayerGeometry;
            if (geometry == null) return;
            using (var jerseyPath = (GraphicsPath)geometry.JerseyPath.Path.Clone())
            using (var headPath = (GraphicsPath)geometry.HeadPath.Path.Clone())
            {
                jerseyPath.Transform(figure.Transform.Matrix);
                headPath.Transform(figure.Transform.Matrix);
                if (figure.Style.FillStyle != null && figure.Style.FillStyle.IsVisible)
                    using (var jerseyBrush = figure.Style.FillStyle.GetBrush(figure))
                        graphics.FillPath(jerseyBrush, jerseyPath);
                var edgeColor = figure.Style.BorderStyle == null ? Color.Black : figure.Style.BorderStyle.Color;
                using (var headBrush = new SolidBrush(Color.FromArgb(
                    figure.Style.BorderStyle == null ? 255 : figure.Style.BorderStyle.Opacity, edgeColor)))
                {
                    graphics.FillPath(headBrush, headPath);
                }
                if (figure.Style.BorderStyle != null && figure.Style.BorderStyle.IsVisible)
                    using (var pen = figure.Style.BorderStyle.GetPen(figure))
                    {
                        graphics.DrawPath(pen, jerseyPath);
                        graphics.DrawPath(pen, headPath);
                    }
            }
            MioSvgRenderer.RenderPlayerNumberAnnotation(graphics, figure);
        }

        public override AllowedRendererDecorators AllowedDecorators { get { return AllowedRendererDecorators.None; } }
    }

    [Serializable]
    public sealed class StylizedPlayerSimpleRenderer : Renderer
    {
        public bool HeadFilled { get; set; }

        public StylizedPlayerSimpleRenderer()
        {
            HeadFilled = false;
        }

        public override void Render(Graphics graphics, Figure figure)
        {
            var geometry = figure.Geometry as StylizedPlayerSimpleGeometry;
            if (geometry == null) return;
            using (var arms = (GraphicsPath)geometry.ArmsPath.Path.Clone())
            using (var head = (GraphicsPath)geometry.HeadPath.Path.Clone())
            {
                arms.Transform(figure.Transform.Matrix);
                head.Transform(figure.Transform.Matrix);
                var border = figure.Style.BorderStyle;
                var borderColor = border == null ? Color.Black : Color.FromArgb(border.Opacity, border.Color);
                var borderWidth = border == null ? 1f : border.Width;
                var jerseyColor = figure.Style.FillStyle == null ? Color.White : figure.Style.FillStyle.Color;
                using (var outerArm = new Pen(borderColor, borderWidth * 2f))
                using (var innerArm = new Pen(Color.FromArgb(
                    figure.Style.FillStyle == null ? 255 : figure.Style.FillStyle.Opacity, jerseyColor), borderWidth))
                {
                    if (border == null || border.IsVisible) graphics.DrawPath(outerArm, arms);
                    if (figure.Style.FillStyle == null || figure.Style.FillStyle.IsVisible) graphics.DrawPath(innerArm, arms);
                }
                if (HeadFilled)
                    using (var headBrush = new SolidBrush(borderColor))
                        graphics.FillPath(headBrush, head);
                if (border != null && border.IsVisible)
                    using (var headPen = border.GetPen(figure))
                        graphics.DrawPath(headPen, head);
            }
            MioSvgRenderer.RenderPlayerNumberAnnotation(graphics, figure);
        }

        public override AllowedRendererDecorators AllowedDecorators { get { return AllowedRendererDecorators.None; } }
    }

    [Serializable]
    public sealed class GiocatoreRenderer : Renderer
    {
        public override void Render(Graphics graphics, Figure figure)
        {
            var geometry = figure.Geometry as GiocatoreGeometry;
            if (geometry == null) return;

            var border = figure.Style.BorderStyle;
            using (var leftArm = (GraphicsPath)geometry.LeftArmFill.Path.Clone())
            using (var rightArm = (GraphicsPath)geometry.RightArmFill.Path.Clone())
            using (var body = (GraphicsPath)geometry.BodyFill.Path.Clone())
            using (var head = (GraphicsPath)geometry.HeadFill.Path.Clone())
            using (var leftArmEdge = (GraphicsPath)geometry.LeftArmBorder.Path.Clone())
            using (var rightArmEdge = (GraphicsPath)geometry.RightArmBorder.Path.Clone())
            using (var bodyEdge = (GraphicsPath)geometry.BodyBorder.Path.Clone())
            using (var headEdge = (GraphicsPath)geometry.HeadBorder.Path.Clone())
            {
                leftArm.Transform(figure.Transform.Matrix);
                rightArm.Transform(figure.Transform.Matrix);
                body.Transform(figure.Transform.Matrix);
                head.Transform(figure.Transform.Matrix);
                leftArmEdge.Transform(figure.Transform.Matrix);
                rightArmEdge.Transform(figure.Transform.Matrix);
                bodyEdge.Transform(figure.Transform.Matrix);
                headEdge.Transform(figure.Transform.Matrix);

                using (var bodyBrush = new SolidBrush(geometry.BodyColor))
                using (var headBrush = new SolidBrush(geometry.HeadColor))
                using (var armsBrush = new SolidBrush(geometry.ArmsColor))
                {
                    graphics.FillPath(bodyBrush, body);
                    graphics.FillPath(headBrush, head);
                    graphics.FillPath(armsBrush, leftArm);
                    graphics.FillPath(armsBrush, rightArm);
                }

                if (border != null && border.IsVisible)
                {
                    using (var pen = border.GetPen(figure))
                    {
                        pen.LineJoin = LineJoin.Round;
                        graphics.DrawPath(pen, bodyEdge);
                        graphics.DrawPath(pen, headEdge);
                        graphics.DrawPath(pen, leftArmEdge);
                        graphics.DrawPath(pen, rightArmEdge);
                    }
                }
            }

            MioSvgRenderer.RenderPlayerNumberAnnotation(graphics, figure);
        }

        public override AllowedRendererDecorators AllowedDecorators { get { return AllowedRendererDecorators.None; } }
    }

    [Serializable]
    public sealed class GiocatoreXRenderer : Renderer
    {
        public override void Render(Graphics graphics, Figure figure)
        {
            new DefaultRenderer().Render(graphics, figure);
            MioSvgRenderer.RenderPlayerNumberAnnotation(graphics, figure);
        }

        public override AllowedRendererDecorators AllowedDecorators { get { return AllowedRendererDecorators.None; } }
    }

    [Serializable]
    public sealed class GiocatoreNumeroRenderer : Renderer
    {
        public override void Render(Graphics graphics, Figure figure)
        {
            var annotation = figure.PlayerNumber;
            var text = annotation == null || string.IsNullOrWhiteSpace(annotation.Text)
                ? "0" : annotation.Text.Trim();
            var bounds = figure.Geometry.Path.Path.GetBounds();
            var matrix = figure.Transform.Matrix;
            var elements = matrix.Elements;
            var verticalScale = (float)Math.Sqrt(elements[2] * elements[2] + elements[3] * elements[3]);
            var fontScale = annotation != null && annotation.FontScale > 0 ? annotation.FontScale : 1f;
            // Coach's C is 36 units tall; this font size gives digits a matching cap height.
            var fontSize = Math.Max(1f, 48f * verticalScale * fontScale);
            var center = new[] { new PointF(bounds.Left + bounds.Width / 2f, bounds.Top + bounds.Height / 2f) };
            matrix.TransformPoints(center);

            using (var font = new Font(FontFamily.GenericSansSerif, fontSize, FontStyle.Bold, GraphicsUnit.World))
            using (var brush = new SolidBrush(annotation == null ? Color.Black : annotation.Color))
            using (var format = new StringFormat(StringFormat.GenericTypographic)
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            })
            {
                // Draw digits with a small negative tracking so two-digit numbers
                // read as one compact jersey number rather than two loose glyphs.
                if (text.Length < 2)
                {
                    graphics.DrawString(text, font, brush, center[0], format);
                }
                else
                {
                    var widths = text.Select(character => graphics.MeasureString(character.ToString(), font,
                        PointF.Empty, format).Width).ToArray();
                    var tracking = fontSize * 0.12f;
                    var totalWidth = widths.Sum() - tracking * (text.Length - 1);
                    var x = center[0].X - totalWidth / 2f;
                    for (var i = 0; i < text.Length; i++)
                    {
                        var glyphCenter = new PointF(x + widths[i] / 2f, center[0].Y);
                        graphics.DrawString(text[i].ToString(), font, brush, glyphCenter, format);
                        x += widths[i] - tracking;
                    }
                }
            }
        }

        public override AllowedRendererDecorators AllowedDecorators { get { return AllowedRendererDecorators.None; } }
    }

    [Serializable]
    public sealed class OggettoBasketRenderer : Renderer
    {
        public override void Render(Graphics graphics, Figure figure)
        {
            var geometry = figure.Geometry as OggettoBasketGeometry;
            if (geometry == null) return;

            bool rotationMark = geometry.Kind == OggettoBasketKind.RotazioneAntioraria ||
                                geometry.Kind == OggettoBasketKind.RotazioneOraria;
            if (!rotationMark)
                new DefaultRenderer().Render(graphics, figure);
            if (geometry.GlyphPath.Path.PointCount == 0 && geometry.ArrowPath.Path.PointCount == 0) return;

            using (var glyph = (GraphicsPath)geometry.GlyphPath.Path.Clone())
            {
                glyph.Transform(figure.Transform.Matrix);
                var border = figure.Style.BorderStyle;
                var color = border == null
                    ? Color.Black
                    : Color.FromArgb(border.Opacity, border.Color);
                using (var brush = new SolidBrush(color))
                    graphics.FillPath(brush, glyph);
            }

            if (geometry.ArrowPath.Path.PointCount > 0)
            {
                var border = figure.Style.BorderStyle;
                var color = border == null ? Color.Black : Color.FromArgb(border.Opacity, border.Color);
                using (var arrow = (GraphicsPath)geometry.ArrowPath.Path.Clone())
                using (var arrowHead = (GraphicsPath)geometry.ArrowHeadPath.Path.Clone())
                using (var pen = new Pen(color, border == null ? 2.2f : Math.Max(2.2f, border.Width * 2f)))
                using (var brush = new SolidBrush(color))
                {
                    arrow.Transform(figure.Transform.Matrix);
                    arrowHead.Transform(figure.Transform.Matrix);
                    pen.LineJoin = LineJoin.Round;
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    graphics.DrawPath(pen, arrow);
                    graphics.FillPath(brush, arrowHead);
                }
            }
        }

        public override AllowedRendererDecorators AllowedDecorators { get { return AllowedRendererDecorators.None; } }
    }

    [Serializable]
    public sealed class BallBasketRenderer : Renderer
    {
        public override void Render(Graphics graphics, Figure figure)
        {
            new DefaultRenderer().Render(graphics, figure);
            var geometry = figure.Geometry as BallBasketGeometry;
            if (geometry == null) return;

            using (var seams = (GraphicsPath)geometry.Seams.Path.Clone())
            {
                seams.Transform(figure.Transform.Matrix);
                var border = figure.Style.BorderStyle;
                var seamColor = border == null ? Color.Black : Color.FromArgb(border.Opacity, border.Color);
                using (var pen = new Pen(seamColor, border == null ? 1f : Math.Max(0.75f, border.Width)))
                {
                    pen.LineJoin = LineJoin.Round;
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    graphics.DrawPath(pen, seams);
                }
            }
        }

        public override AllowedRendererDecorators AllowedDecorators { get { return AllowedRendererDecorators.None; } }
    }

      [Serializable]
      public class DefaultRenderer : Renderer
    {
     
        public override void Render(Graphics graphics, Figure figure)
        {
          
            using (var path = figure.GetTransformedPath().Path)
            {
                
                if (figure.Style.FillStyle != null && figure.Style.FillStyle.IsVisible)
                   
                    using (var brush = figure.Style.FillStyle.GetBrush(figure))
                        graphics.FillPath(brush, path);
                
                if (figure.Style.BorderStyle != null && figure.Style.BorderStyle.IsVisible)
                    
                    using (var pen = figure.Style.BorderStyle.GetPen(figure))
                        graphics.DrawPath(pen, path);
            }
        }

      
        public override AllowedRendererDecorators AllowedDecorators
        {
            get { return AllowedRendererDecorators.All; }
        }
    }

}
