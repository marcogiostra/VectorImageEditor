using EditorModel.Figures;
using EditorModel.Renderers;
using EditorModel.Selections;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace VectorImageEditor.Controls.Basket 
{
    public sealed class GiocatoreStilizzatoEditor : UserControl, IEditor<Selection>
    {
        private readonly Button _jerseyColor = new Button { Text = "Colore maglia", Width = 150 };
        private readonly Button _headAndBorderColor = new Button { Text = "Colore testa e bordi", Width = 150 };
        private readonly CheckBox _headFilled = new CheckBox { Text = "Testa piena", AutoSize = true };
        private readonly Label _title = new Label { Text = "Giocatore stilizzato", AutoSize = true };
        private Selection _selection;
        private bool _updating;

        public event EventHandler<ChangingEventArgs> StartChanging = delegate { };
        public event EventHandler<EventArgs> Changed = delegate { };

        public GiocatoreStilizzatoEditor()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            panel.Controls.Add(_title);
            panel.Controls.Add(_jerseyColor);
            panel.Controls.Add(_headAndBorderColor);
            panel.Controls.Add(_headFilled);
            Controls.Add(panel);
            _jerseyColor.Click += (s, e) => PickColor(_jerseyColor, true);
            _headAndBorderColor.Click += (s, e) => PickColor(_headAndBorderColor, false);
            _headFilled.CheckedChanged += (s, e) => ApplyHeadFill();
            Visible = false;
        }

        public void Build(Selection selection)
        {
            _selection = selection;
            var figures = selection.Where(IsStylizedPlayer).ToList();
            Visible = figures.Count > 0 && figures.Count == selection.Count;
            if (!Visible) return;
            _updating = true;
            try
            {
                _jerseyColor.BackColor = figures[0].Style.FillStyle.Color;
                _headAndBorderColor.BackColor = figures[0].Style.BorderStyle.Color;
                _title.Text = IsSimplePlayer(figures[0]) ? "Giocatore stilizzato semplice" : "Giocatore stilizzato";
                _headFilled.Visible = figures.All(IsSimplePlayer);
                if (_headFilled.Visible)
                {
                    var renderer = figures[0].Renderer as StylizedPlayerSimpleRenderer;
                    _headFilled.Checked = renderer != null && renderer.HeadFilled;
                }
            }
            finally { _updating = false; }
        }

        private static bool IsStylizedPlayer(Figure figure)
        {
            return figure != null && figure.Geometry != null &&
                (figure.Geometry.Name == "GiocatoreStilizzato" || IsSimplePlayer(figure));
        }

        private static bool IsSimplePlayer(Figure figure)
        {
            return figure != null && figure.Geometry != null && figure.Geometry.Name == "GiocatoreStilizzatoSemplice";
        }

        private void ApplyHeadFill()
        {
            if (_updating || !Visible || !_headFilled.Visible || _selection == null) return;
            StartChanging(this, new ChangingEventArgs("Testa piena"));
            foreach (var figure in _selection.Where(IsSimplePlayer))
            {
                var renderer = figure.Renderer as StylizedPlayerSimpleRenderer;
                if (renderer != null) renderer.HeadFilled = _headFilled.Checked;
            }
            Changed(this, EventArgs.Empty);
        }

        private void PickColor(Button button, bool jersey)
        {
            using (var dialog = new ColorDialog { Color = button.BackColor, FullOpen = true })
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;
                button.BackColor = dialog.Color;
                if (_updating || !Visible || _selection == null) return;
                StartChanging(this, new ChangingEventArgs(jersey ? "Colore maglia" : "Colore testa e bordi"));
                foreach (var figure in _selection.Where(IsStylizedPlayer))
                {
                    if (jersey) figure.Style.FillStyle.Color = dialog.Color;
                    else figure.Style.BorderStyle.Color = dialog.Color;
                }
                Changed(this, EventArgs.Empty);
            }
        }
    }
}
