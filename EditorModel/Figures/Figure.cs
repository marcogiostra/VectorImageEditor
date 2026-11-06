using EditorModel.Common;
using System;
using System.Drawing.Drawing2D;
using EditorModel.Renderers;
using System.Drawing;

namespace EditorModel.Figures
{
    public enum PlayerNumberPosition
    {
        N,
        NE,
        E,
        SE,
        S,
        SW,
        W,
        NW
    }

    [Serializable]
    public sealed class PlayerNumberAnnotation
    {
        public string Text { get; set; } = string.Empty;
        public PlayerNumberPosition Position { get; set; } = PlayerNumberPosition.N;
        /// <summary>Font height as a fraction of the player's smaller dimension.</summary>
        public float FontScale { get; set; } = 0.2f;
        public int ColorArgb { get; set; } = Color.Black.ToArgb();
        public bool IsVisible { get; set; }

        public Color Color
        {
            get { return Color.FromArgb(ColorArgb); }
            set { ColorArgb = value.ToArgb(); }
        }
    }

    [Serializable]
    public class Figure
    {
        public SerializableGraphicsMatrix Transform { get; set; }

        public Geometry.Geometry Geometry { get; set; }

        public Style.Style Style { get; private set; }

        public Renderer Renderer { get; set; }

        public PlayerNumberAnnotation PlayerNumber { get; set; }

        /// <summary>Keeps player titles useful to external macros.</summary>
        public void UpdateGiocatoreTitleFromPlayerNumber()
        {
            if (Geometry == null || string.IsNullOrEmpty(Geometry.Name) ||
                !Geometry.Name.StartsWith("Giocatore", StringComparison.Ordinal)) return;

            var number = PlayerNumber != null && PlayerNumber.IsVisible
                ? (PlayerNumber.Text ?? string.Empty).Trim()
                : string.Empty;
            if (Geometry.Name == "GiocatoreNumero" && string.IsNullOrEmpty(number)) number = "0";
            var playerName = Geometry.Name;
            Geometry.Title = string.IsNullOrEmpty(number) ? playerName : playerName + "_" + number;
        }

        public Figure()
        {
            Transform = new SerializableGraphicsMatrix();
            Style = new Style.Style();
            Renderer = new DefaultRenderer();
        }

        public virtual SerializableGraphicsPath GetTransformedPath()
        {
            var path = (GraphicsPath)Geometry.Path.Path.Clone();
            path.Transform(Transform);
            return path;
        }
    
        public virtual void PushTransform(Matrix matrix)
        {
            Transform.Matrix.Multiply(matrix, MatrixOrder.Append);
        }

    }

}
