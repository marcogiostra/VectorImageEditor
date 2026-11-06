using EditorModel.Figures;
using EditorModel.Selections;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace VectorImageEditor.Controls.Basket
{
    /// <summary>Modifica il numero di maglia dei giocatori selezionati.</summary>
    public sealed partial class PlayerNumberEditor : UserControl, IEditor<Selection>
    {
        private Selection _selection;
        private int _updating;

        public event EventHandler<ChangingEventArgs> StartChanging = delegate { };
        public event EventHandler<EventArgs> Changed = delegate { };

        public PlayerNumberEditor()
        {
            InitializeComponent();
            Visible = false;
            cbPosition.Items.AddRange(Enum.GetValues(typeof(PlayerNumberPosition)).Cast<object>().ToArray());
        }

        public void Build(Selection selection)
        {
            _selection = selection;
            // The type (Geometry.Name) is the discriminator for player SVGs. Older
            // documents may not yet contain the annotation property, so create its
            // defaults when such a player is selected.
            var players = selection.Where(IsPlayer).ToList();
            foreach (var player in players)
            {
                if (player.PlayerNumber == null)
                    player.PlayerNumber = new PlayerNumberAnnotation();
                if (player.Geometry.Name == "GiocatoreNumero")
                {
                    if (string.IsNullOrWhiteSpace(player.PlayerNumber.Text))
                        player.PlayerNumber.Text = "0";
                    player.PlayerNumber.IsVisible = true;
                }
                player.UpdateGiocatoreTitleFromPlayerNumber();
            }

            Visible = selection.Count > 0 && players.Count == selection.Count;
            if (!Visible) return;

            _updating++;
            try
            {
                tbNumber.Text = players[0].PlayerNumber.Text ?? string.Empty;
                cbPosition.SelectedItem = players[0].PlayerNumber.Position;
                btnColor.BackColor = players[0].PlayerNumber.Color;
                var fontPercent = (decimal)(players[0].PlayerNumber.FontScale > 0
                    ? players[0].PlayerNumber.FontScale * 100f : 20f);
                nudFontSize.Value = Math.Max(nudFontSize.Minimum,
                    Math.Min(nudFontSize.Maximum, fontPercent));
                cbShow.Checked = players[0].PlayerNumber.IsVisible;
                cbShow.Enabled = players.All(player => player.Geometry.Name != "GiocatoreNumero");
            }
            finally { _updating--; }
        }

        private static bool IsPlayer(Figure figure)
        {
            return figure.Geometry.Name == "GiocatoreAttacco" || figure.Geometry.Name == "GiocatoreDifesa" ||
                figure.Geometry.Name == "GiocatoreStilizzato" || figure.Geometry.Name == "GiocatoreStilizzatoSemplice" ||
                figure.Geometry.Name == "Giocatore" || figure.Geometry.Name == "GiocatoreX" ||
                figure.Geometry.Name == "GiocatoreO" ||
                figure.Geometry.Name == "GiocatoreNumero";
        }

        private void UpdateObject()
        {
            if (_updating > 0 || _selection == null || !Visible) return;
            if (cbPosition.SelectedItem == null) return;

            StartChanging(this, new ChangingEventArgs("Numero di maglia"));
            foreach (var figure in _selection.Where(IsPlayer))
            {
                figure.PlayerNumber.Text = tbNumber.Text;
                figure.PlayerNumber.Position = (PlayerNumberPosition)cbPosition.SelectedItem;
                figure.PlayerNumber.Color = btnColor.BackColor;
                figure.PlayerNumber.FontScale = (float)nudFontSize.Value / 100f;
                figure.PlayerNumber.IsVisible = figure.Geometry.Name == "GiocatoreNumero" || cbShow.Checked;
                figure.UpdateGiocatoreTitleFromPlayerNumber();
            }
            Changed(this, EventArgs.Empty);
        }

        private void Color_Click(object sender, EventArgs e)
        {
            using (var dialog = new ColorDialog { Color = btnColor.BackColor, FullOpen = true })
            {
                if (dialog.ShowDialog(FindForm()) != DialogResult.OK) return;
                btnColor.BackColor = dialog.Color;
                UpdateObject();
            }
        }

        private void tbNumber_TextChanged(object sender, EventArgs e) { UpdateObject(); }
        private void cbPosition_SelectedIndexChanged(object sender, EventArgs e) { UpdateObject(); }
        private void cbShow_CheckedChanged(object sender, EventArgs e) { UpdateObject(); }
        private void nudFontSize_ValueChanged(object sender, EventArgs e) { UpdateObject(); }
    }
}
