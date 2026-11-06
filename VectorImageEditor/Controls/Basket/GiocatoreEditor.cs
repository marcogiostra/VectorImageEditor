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
    public sealed class GiocatoreEditor : UserControl, IEditor<Selection>
    {
        private readonly ComboBox _pose = new ComboBox { DropDownStyle = ComboBoxStyle.DropDownList, Width = 180 };
        private readonly Button _headColor = new Button { Text = "Colore testa", Width = 150 };
        private readonly Button _bodyColor = new Button { Text = "Colore corpo/maglia", Width = 150 };
        private readonly Button _armsColor = new Button { Text = "Colore braccia", Width = 150 };
        private readonly Button _borderColor = new Button { Text = "Colore bordo", Width = 150 };
        private Selection _selection;
        private bool _updating;

        private sealed class PoseOption
        {
            public GiocatorePose Value { get; private set; }
            public string Label { get; private set; }
            public PoseOption(GiocatorePose value, string label) { Value = value; Label = label; }
            public override string ToString() { return Label; }
        }

        public event EventHandler<ChangingEventArgs> StartChanging = delegate { };
        public event EventHandler<EventArgs> Changed = delegate { };

        public GiocatoreEditor()
        {
            var panel = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false
            };
            panel.Controls.Add(new Label { Text = "Giocatore", AutoSize = true });
            panel.Controls.Add(new Label { Text = "Figura da visualizzare", AutoSize = true });
            panel.Controls.Add(_pose);
            panel.Controls.Add(_headColor);
            panel.Controls.Add(_bodyColor);
            panel.Controls.Add(_armsColor);
            panel.Controls.Add(_borderColor);
            Controls.Add(panel);

            foreach (GiocatorePose pose in Enum.GetValues(typeof(GiocatorePose)))
            {
                var label = pose == GiocatorePose.GiocatoreCheBlocca
                    ? "Difesa"
                    : pose == GiocatorePose.GiocatoreBlocco
                        ? "Blocco"
                    : pose == GiocatorePose.GiocatoreAnticipo
                        ? "Anticipo"
                        : pose == GiocatorePose.GiocatorePalleggio
                            ? "Palleggio"
                        : pose == GiocatorePose.GiocatoreTiro
                                ? "Tiro"
                                : pose == GiocatorePose.GiocatoreRimbalzo
                                    ? "Rimbalzo"
                                    : pose == GiocatorePose.GiocatoreStance
                                        ? "Stance"
                                        : "Attacco";
                _pose.Items.Add(new PoseOption(pose, label));
            }
            _pose.SelectedIndexChanged += (s, e) => ApplyPose();
            _headColor.Click += (s, e) => PickColor(_headColor, ColorTarget.Head);
            _bodyColor.Click += (s, e) => PickColor(_bodyColor, ColorTarget.Body);
            _armsColor.Click += (s, e) => PickColor(_armsColor, ColorTarget.Arms);
            _borderColor.Click += (s, e) => PickColor(_borderColor, ColorTarget.Border);
            Visible = false;
        }

        public void Build(Selection selection)
        {
            _selection = selection;
            var figures = selection.Where(IsGiocatore).ToList();
            Visible = figures.Count > 0 && figures.Count == selection.Count;
            if (!Visible) return;

            _updating = true;
            try
            {
                var geometry = figures[0].Geometry as GiocatoreGeometry;
                if (geometry != null)
                {
                    _headColor.BackColor = geometry.HeadColor;
                    _bodyColor.BackColor = geometry.BodyColor;
                    _armsColor.BackColor = geometry.ArmsColor;
                }
                _borderColor.BackColor = figures[0].Style.BorderStyle.Color;
                var pose = geometry == null ? GiocatorePose.GiocatoreAttacco : geometry.Pose;
                _pose.SelectedItem = _pose.Items.Cast<PoseOption>().FirstOrDefault(item => item.Value == pose);
            }
            finally { _updating = false; }
        }

        private static bool IsGiocatore(Figure figure)
        {
            return figure != null && figure.Geometry != null && figure.Geometry.Name == "Giocatore";
        }

        private void ApplyPose()
        {
            var option = _pose.SelectedItem as PoseOption;
            if (_updating || !Visible || _selection == null || option == null) return;
            StartChanging(this, new ChangingEventArgs("Figura giocatore"));
            foreach (var figure in _selection.Where(IsGiocatore))
            {
                var geometry = figure.Geometry as GiocatoreGeometry;
                if (geometry != null) geometry.Pose = option.Value;
            }
            Changed(this, EventArgs.Empty);
        }

        private enum ColorTarget { Head, Body, Arms, Border }

        private void PickColor(Button button, ColorTarget target)
        {
            using (var dialog = new ColorDialog { Color = button.BackColor, FullOpen = true })
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;
                button.BackColor = dialog.Color;
                if (_updating || !Visible || _selection == null) return;
                StartChanging(this, new ChangingEventArgs("Colore " + target.ToString().ToLowerInvariant() + " giocatore"));
                foreach (var figure in _selection.Where(IsGiocatore))
                {
                    var geometry = figure.Geometry as GiocatoreGeometry;
                    if (target == ColorTarget.Border) figure.Style.BorderStyle.Color = dialog.Color;
                    else if (geometry != null && target == ColorTarget.Head) geometry.HeadColor = dialog.Color;
                    else if (geometry != null && target == ColorTarget.Body) geometry.BodyColor = dialog.Color;
                    else if (geometry != null && target == ColorTarget.Arms) geometry.ArmsColor = dialog.Color;
                }
                Changed(this, EventArgs.Empty);
            }
        }
    }
}
