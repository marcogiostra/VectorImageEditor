using EditorModel.Figures;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EditorModel.Style
{
    [Serializable]
    public class RadialGradientFill : FillDecorator, IGradientFill
    {
        private readonly Fill _fill;

        private PointF[] _points;

        public RadialGradientFill(Fill fill)
            : base(fill)
        {
            _fill = fill;
            _points = new[] { new PointF(0.25f, -0.25f) };
            GradientColor = Color.Gray;
        }

     
        public PointF[] Points
        {
            get { return _points; }
            set { _points = value; }
        }

       
        public PointF[] GetGradientPoints(Figure owner)
        {
            var points = (PointF[])Points.Clone();
            owner.Transform.Matrix.TransformPoints(points);
            return points;
        }


        public void SetGradientPoints(Figure owner, PointF[] points)
        {
            points = (PointF[])points.Clone();
            var m = owner.Transform.Matrix.Clone();
            m.Invert();
            m.TransformPoints(points);
            Points = points;
        }

       
        public Color GradientColor { get; set; }

        public override Brush GetBrush(Figure figure)
        {
          
            var pts = (PointF[])_points.Clone();
            figure.Transform.Matrix.TransformPoints(pts);
            var brush = new PathGradientBrush(figure.GetTransformedPath().Path);
            brush.CenterPoint = pts[0];
            brush.CenterColor = Color;
            brush.SurroundColors = new Color[] { GradientColor };
            return brush;
        }

       
        public override AllowedFillDecorators AllowedDecorators
        {
            get { return AllowedFillDecorators.None; }
        }
      

        public override int Opacity
        {
            get { return _fill.Opacity; }
            set { _fill.Opacity = value; }
        }

   
        public override Color Color
        {
            get { return _fill.Color; }
            set { _fill.Color = value; }
        }

       
        public override bool IsVisible
        {
            get { return _fill.IsVisible; }
            set { _fill.IsVisible = value; }
        }
        public override string ToString()
        {
            return "RadialGradient";
        }
    }
}
