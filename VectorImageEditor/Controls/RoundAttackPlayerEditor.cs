using EditorModel.Figures;
using EditorModel.Selections;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace VectorImageEditor.Controls
{
    public sealed class RoundAttackPlayerEditor : UserControl, IEditor<Selection>
    {
        private readonly TextBox _number = new TextBox();
        private readonly CheckBox _showNumber = new CheckBox { Text = "Mostra numero al centro", AutoSize = true };
        private readonly CheckBox _fill = new CheckBox { Text = "Cerchio pieno", AutoSize = true };
        private readonly CheckBox _sameColor = new CheckBox { Text = "Stesso colore per bordo e riempimento", AutoSize = true };
        private readonly Button _border = new Button { Text = "Colore bordo", Width = 110 };
        private readonly Button _fillColor = new Button { Text = "Colore riempimento", Width = 130 };
        private readonly Button _numberColor = new Button { Text = "Colore numero", Width = 110 };
        private Selection _selection;
        private bool _updating;
        public event EventHandler<ChangingEventArgs> StartChanging = delegate { };
        public event EventHandler<EventArgs> Changed = delegate { };

        public RoundAttackPlayerEditor()
        {
            var panel = new FlowLayoutPanel { Dock = DockStyle.Fill, AutoSize = true, FlowDirection = FlowDirection.TopDown, WrapContents = false };
            panel.Controls.Add(new Label { Text = "Opzioni giocatore", AutoSize = true });
            panel.Controls.Add(_showNumber);
            panel.Controls.Add(_number);
            panel.Controls.Add(_numberColor);
            panel.Controls.Add(_fill);
            panel.Controls.Add(_sameColor);
            panel.Controls.Add(_fillColor);
            panel.Controls.Add(_border);
            Controls.Add(panel);
            _number.TextChanged += (s, e) => Apply();
            _showNumber.CheckedChanged += (s, e) => Apply();
            _fill.CheckedChanged += (s, e) => Apply();
            _sameColor.CheckedChanged += (s, e) =>
            {
                if (_sameColor.Checked) _border.BackColor = _fillColor.BackColor;
                Apply();
            };
            _border.Click += (s, e) => PickColor(_border);
            _fillColor.Click += (s, e) => PickColor(_fillColor);
            _numberColor.Click += (s, e) => PickColor(_numberColor);
            Visible = false;
        }

        public void Build(Selection selection)
        {
            _selection = selection;
            var figures = selection.Where(IsRoundPlayer).ToList();
            Visible = figures.Count > 0 && figures.Count == selection.Count;
            if (!Visible) return;
            _updating = true;
            try
            {
                var figure = figures[0];
                figure.PlayerNumber = figure.PlayerNumber ?? new PlayerNumberAnnotation();
                _number.Text = figure.PlayerNumber.Text ?? string.Empty;
                _showNumber.Checked = figure.PlayerNumber.IsVisible;
                _numberColor.BackColor = figure.PlayerNumber.Color;
                _border.BackColor = figure.Style.BorderStyle.Color;
                _fillColor.BackColor = figure.Style.FillStyle.Color;
                _fill.Checked = figure.Style.FillStyle.IsVisible;
                _sameColor.Checked = false;
                _fillColor.Enabled = _fill.Checked;
                _border.Enabled = true;
            }
            finally { _updating = false; }
        }

        private static bool IsRoundPlayer(Figure f)
        {
            return f != null && f.Geometry != null &&
                (f.Geometry.Name == "GiocatoreAttaccoRotondo" || f.Geometry.Name == "GiocatoreDifesaTriangolare");
        }
        private void Apply()
        {
            if (_updating || !Visible || _selection == null) return;
            StartChanging(this, new ChangingEventArgs("Opzioni giocatore"));
            foreach (var f in _selection.Where(IsRoundPlayer))
            {
                f.PlayerNumber = f.PlayerNumber ?? new PlayerNumberAnnotation();
                f.PlayerNumber.Text = _number.Text;
                f.PlayerNumber.IsVisible = _showNumber.Checked;
                f.UpdateGiocatoreTitleFromPlayerNumber();
                f.PlayerNumber.FontScale = 0.45f;
                f.PlayerNumber.Color = _numberColor.BackColor;
                f.Style.BorderStyle.Color = _border.BackColor;
                f.Style.FillStyle.Color = _fillColor.BackColor;
                f.Style.FillStyle.IsVisible = _fill.Checked;
            }
            _fillColor.Enabled = _fill.Checked;
            Changed(this, EventArgs.Empty);
        }
        private void PickColor(Button button)
        {
            using (var dialog = new ColorDialog { Color = button.BackColor, FullOpen = true })
                if (dialog.ShowDialog(FindForm()) == DialogResult.OK)
                {
                    button.BackColor = dialog.Color;
                    if (_sameColor.Checked)
                    {
                        if (button == _border) _fillColor.BackColor = dialog.Color;
                        else if (button == _fillColor) _border.BackColor = dialog.Color;
                    }
                    Apply();
                }
        }
    }
}
