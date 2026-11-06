using EditorModel.Figures;
using EditorModel.Selections;
using System;
using System.Linq;
using System.Windows.Forms;

namespace VectorImageEditor.Controls.Basket
{
    /// <summary>
    /// EDITOR:palla di basket: consente di modificare il colore della palla e delle cuciture.
    /// </summary>
    public sealed partial class BallBasketEditor : UserControl, IEditor<Selection>
    {
        private Selection _selection;
        private bool _updating;

        public event EventHandler<ChangingEventArgs> StartChanging = delegate { };
        public event EventHandler<EventArgs> Changed = delegate { };

        public BallBasketEditor()
        {
            InitializeComponent();

            // Il controllo è nascosto di default: verrà mostrato da Build()
            // solo se la selezione contiene esclusivamente figure "BallBasket".
            Visible = false;

           
        }

        public void Build(Selection selection)
        {
            _selection = selection;
            var figures = selection.Where(IsBall).ToList();

            Visible = figures.Count > 0 && figures.Count == selection.Count;
            if (!Visible) return;

            _updating = true;
            try
            {
                lblFillColor.BackColor = figures[0].Style.FillStyle.Color;
                lblSeamColor.BackColor = figures[0].Style.BorderStyle.Color;
            }
            finally { _updating = false; }
        }

        private static bool IsBall(Figure figure)
        {
            return figure != null
                && figure.Geometry != null
                && figure.Geometry.Name == "BallBasket";
        }

        private void PickColor(Label label, bool seams)
        {
            using (var dialog = new ColorDialog { Color = label.BackColor, FullOpen = true })
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;

                label.BackColor = dialog.Color;
                if (_updating || !Visible || _selection == null) return;

                StartChanging(this, new ChangingEventArgs(
                    seams ? "Colore bordo e cuciture palla" : "Colore palla"));

                foreach (var figure in _selection.Where(IsBall))
                {
                    if (seams)
                        figure.Style.BorderStyle.Color = dialog.Color;
                    else
                        figure.Style.FillStyle.Color = dialog.Color;
                }

                Changed(this, EventArgs.Empty);
            }
        }

        private void lbFillColor_Click(object sender, EventArgs e)
        {
            PickColor((Label)sender, false);
        }

        private void lblSeamColor_Click(object sender, EventArgs e)
        {
            PickColor((Label)sender, true);
        }
    }
}