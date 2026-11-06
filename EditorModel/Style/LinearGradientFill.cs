using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using EditorModel.Figures;

namespace EditorModel.Style
{
    [Serializable]
    public class LinearGradientFill : FillDecorator, IGradientFill
    {
        private readonly Fill _fill;

        private PointF[] _points;

        public LinearGradientFill(Fill fill)
            : base(fill)
        {
            _fill = fill;
            _points = new[] { new PointF(-0.5f, 0.25f), new PointF(0.5f, 0.25f) };
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
            return new LinearGradientBrush(pts[0], pts[1], Color.FromArgb(_fill.Opacity, _fill.Color), GradientColor);
        }

       
        public override AllowedFillDecorators AllowedDecorators
        {
            get { return AllowedFillDecorators.None; }
        }
   

        public override int Opacity
        {
            get { return _fill.Opacity; }
            set { _fill.Opacity = value;  }
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
            return "LinearGradient";
        }
    }

}
