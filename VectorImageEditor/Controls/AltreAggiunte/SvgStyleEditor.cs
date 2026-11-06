using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Windows.Forms;
using EditorModel.Renderers;
using EditorModel.Selections;
using Svg;

namespace VectorImageEditor.Controls.AltreAggiunte
{

    public partial class SvgStyleEditor : UserControl, IEditor<Selection>
    {
        private Selection _selection;
        private int _updating;
        private SvgDocument _svg;   // documento corrente dell'editor

        public event EventHandler<ChangingEventArgs> StartChanging = delegate { };
        public event EventHandler<EventArgs> Changed = delegate { };

        public SvgStyleEditor()
        {
            InitializeComponent();
        }

        public void Build(Selection selection)
        {
            // check visibility
            Visible = selection.ForAll(f => RendererDecorator.GetBaseRenderer(f.Renderer) is MioSvgRenderer);
            if (!Visible) return; // do not build anything

            // remember editing object
            _selection = selection;

            // get list of objects
            var svgRenderers = _selection
                .Select(f => (MioSvgRenderer)RendererDecorator.GetBaseRenderer(f.Renderer))
                .ToList();

            // copy properties of object to GUI
            _updating++;

            _svg = svgRenderers.GetProperty(r => r.Document);

            _updating--;
        }

        
        private void UpdateObjectOLD()
        {
            if (_updating > 0) return; // we are in updating mode

            // fire event
            StartChanging(this, new ChangingEventArgs("Svg Fill Style"));

            // get list of objects
            foreach (var fig in _selection.Where(f =>  RendererDecorator.GetBaseRenderer(f.Renderer) is MioSvgRenderer))
            {
                var renderer = (MioSvgRenderer)RendererDecorator.GetBaseRenderer(fig.Renderer);
                var firstIsEmpty = renderer.Document == null;

                renderer.Document = _svg;   // assegna il nuovo SVG

                if (firstIsEmpty && _svg != null)
                {
                    var bounds = fig.GetTransformedPath().Path.GetBounds();
                    //var bounds = renderer.Document.Bounds;
                    var x = bounds.X;
                    var y = bounds.Y;
                    var width = _svg.Width.Value;
                    var height = _svg.Height.Value;

                    var m = new Matrix();
                    m.Translate(x + width / 2f, y + height / 2f);
                    m.Scale(width, height);
                    m.Scale(_svg.Bounds.Width, _svg.Bounds.Height);

                    fig.Transform.Matrix = m;
                }
            }

            // fire event
            Changed(this, EventArgs.Empty);

 
        }

  

        private void UpdateObject()
        {
            if (_updating > 0)
                return;

            StartChanging(this, new ChangingEventArgs("Image Fill Style"));

            foreach (var fig in _selection.Where(f =>
                RendererDecorator.GetBaseRenderer(f.Renderer) is MioSvgRenderer))
            {
                var renderer =
                    (MioSvgRenderer)RendererDecorator.GetBaseRenderer(fig.Renderer);

                var firstIsEmpty = renderer.Document == null;

                renderer.Document = _svg;

                if (firstIsEmpty)
                {
                    var bounds = fig.GetTransformedPath().Path.GetBounds();

                    var x = bounds.X;
                    var y = bounds.Y;

                    // NON usare _svg.Width / _svg.Height:
                    // nel file sono "100%".
                    //
                    // Usa il ViewBox: 0 0 61 42

                    float width = (float)_svg.ViewBox.Width;
                    float height = (float)_svg.ViewBox.Height;

                    if (width <= 0 || height <= 0)
                        continue;

                    var m = new Matrix();

                    m.Translate(
                        x + width / 2f,
                        y + height / 2f);

                    m.Scale(
                        width,
                        height);

                    fig.Transform.Matrix = m;
                }
            }

            Changed(this, EventArgs.Empty);
        }



       

        private void btnLoadSvg_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = @"File SVG (*.svg)|*.svg"
            };
            if (dlg.ShowDialog() != DialogResult.OK) return;

            try
            {
                _svg = SvgDocument.Open(dlg.FileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show("SVG non valido: " + ex.Message);
                return;
            }

            UpdateObject();

  
        }
    }
}


