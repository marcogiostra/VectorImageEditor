using EditorModel.Common;
using EditorModel.Figures;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace EditorModel.Renderers
{
    [Serializable]
    public class TextBlockDecorator : RendererDecorator, ITextBlock, IRendererTransformedPath
    {
        private readonly Renderer _renderer;

       
        public string Text { get; set; }

        
        public string FontName { get; set; }

       
        public float FontSize { get; set; }

       
        public FontStyle FontStyle { get; set; }

        public ContentAlignment Alignment { get; set; }


        public Padding Padding { get; set; }

        public bool WordWrap { get; set; }

        public TextBlockDecorator(Renderer renderer, string text, Padding padding, bool wordWrap = true)
            : base(renderer)
        {
            _renderer = renderer;
            Text = text;
            FontName = "Arial";
            FontSize = 14f;
            Alignment = ContentAlignment.MiddleCenter;
            Padding = padding;
            WordWrap = wordWrap;
        }

        
        public GraphicsPath GetTransformedPath(Graphics graphics, Figure figure)
        {
            return TextRenderer.GetTextBlockTransformedPath(graphics, figure, this, Padding, WordWrap);
        }

        public override void Render(Graphics graphics, Figure figure)
        {
            _renderer.Render(graphics, figure);
            var baseRenderer = GetBaseRenderer(figure.Renderer) as IRendererTransformedPath;
            
            using (var path = baseRenderer != null
                ? baseRenderer.GetTransformedPath(graphics, figure)
                : figure.GetTransformedPath().Path)
            {
                using (var gp = GetTransformedPath(graphics, figure))
                using (var brush = new SolidBrush(Color.Black))
                    graphics.FillPath(brush, gp);
            }
        }

       
        public override AllowedRendererDecorators AllowedDecorators
        {
            get { return AllowedRendererDecorators.All ^ AllowedRendererDecorators.TextBlock; }
        }
        public override string ToString()
        {
            return "TextBlock";
        }
    }
}
