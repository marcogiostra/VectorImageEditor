using EditorModel.Figures;
using EditorModel.Geometry;
using EditorModel.Geometry.GeometryBasket;
using EditorModel.Selections;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace VectorImageEditor.Controls.Basket
{
    /// <summary>
    /// EDITOR: oggetto basket: consente di modificare il tipo di oggetto (cono, coach, assistente, ecc.) e i colori di riempimento e bordo.
    /// </summary>
    public sealed partial class OggettoBasketEditor : UserControl, IEditor<Selection>
    {
        private Selection _selection;
        private bool _updating;

        private sealed class KindOption
        {
            public OggettoBasketKind Value { get; private set; }
            public string Label { get; private set; }
            public KindOption(OggettoBasketKind value, string label) { Value = value; Label = label; }
            public override string ToString() { return Label; }
        }

        public event EventHandler<ChangingEventArgs> StartChanging;
        public event EventHandler<EventArgs> Changed;

        public OggettoBasketEditor()
        {
            Visible = false;

            InitializeComponent();

            //inizializza la combo KIND
            cmbKind.Items.Add(new KindOption(OggettoBasketKind.Cono, "Cono"));
            cmbKind.Items.Add(new KindOption(OggettoBasketKind.ConoPiccolo, "ConoPiccolo"));
            cmbKind.Items.Add(new KindOption(OggettoBasketKind.Coach, "Coach"));
            cmbKind.Items.Add(new KindOption(OggettoBasketKind.CoachConPalla, "Coach con palla"));
            cmbKind.Items.Add(new KindOption(OggettoBasketKind.Assistente, "Assistente"));
            cmbKind.Items.Add(new KindOption(OggettoBasketKind.AssistenteConPalla, "Assistente con palla"));
            cmbKind.Items.Add(new KindOption(OggettoBasketKind.RotazioneAntioraria, "Rotazione antioraria"));
            cmbKind.Items.Add(new KindOption(OggettoBasketKind.RotazioneOraria, "Rotazione oraria"));

            Visible = false;
        }


  

        private void lblFillColor_Click(object sender, EventArgs e)
        {
            PickColor((Label)sender, false);
        }

        private void lblBorderColor_Click(object sender, EventArgs e)
        {
            PickColor((Label)sender, true);

        }

        private void cmbKind_SelectedIndexChanged(object sender, EventArgs e)
        {
            ApplyKind();
        }

        
        public void Build(Selection selection)
        {
            _selection = selection;
            var figures = selection.Where(IsOggettoBasket).ToList();
            Visible = figures.Count > 0 && figures.Count == selection.Count;
            if (!Visible) return;

            _updating = true;
            try
            {
                var geometry = figures[0].Geometry as OggettoBasketGeometry;
                cmbKind.SelectedItem = cmbKind.Items.Cast<KindOption>().FirstOrDefault(option =>
                    option.Value == (geometry == null ? OggettoBasketKind.Cono : geometry.Kind));
                lblFillColor.BackColor = figures[0].Style.FillStyle.Color;
                lblBorderColor.BackColor = figures[0].Style.BorderStyle.Color;
            }
            finally { _updating = false; }
        }
        

        private static bool IsOggettoBasket(Figure figure)
        {
            return figure != null && figure.Geometry != null && figure.Geometry.Name == "OggettoBasket";
        }

        private void ApplyKind()
        {
            var option = cmbKind.SelectedItem as KindOption;
            if (_updating || !Visible || _selection == null || option == null) return;
            StartChanging(this, new ChangingEventArgs("Tipo oggetto basket"));
            foreach (var figure in _selection.Where(IsOggettoBasket))
            {
                var geometry = figure.Geometry as OggettoBasketGeometry;
                if (geometry != null) geometry.Kind = option.Value;
            }
            Changed(this, EventArgs.Empty);
        }

        private void PickColor(Label label, bool border)
        {
            using (var dialog = new ColorDialog { Color = label.BackColor, FullOpen = true })
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;
                label.BackColor = dialog.Color;
                if (_updating || !Visible || _selection == null) return;
                StartChanging(this, new ChangingEventArgs(border ? "Colore bordo oggetto basket" : "Colore riempimento oggetto basket"));
                foreach (var figure in _selection.Where(IsOggettoBasket))
                {
                    if (border) figure.Style.BorderStyle.Color = dialog.Color;
                    else figure.Style.FillStyle.Color = dialog.Color;
                }
                Changed(this, EventArgs.Empty);
            }
        }
    }
}
