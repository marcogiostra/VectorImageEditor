using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using EditorModel.Figures;

namespace EditorModel.Style
{
  
    [Serializable]
    public class Border
    {
        
        public Border()
        {
        
            IsVisible = true;
        
            Opacity = 255;
         
            Color = Color.Black;
          
            Width = 1f;
         
            DashStyle = DashStyle.Solid;
        }

 
        public int Opacity { get; set; }

    
        public float Width { get; set; }

     
        public Color Color { get; set; }

  
      
        public bool IsVisible { get; set; }

        public DashStyle DashStyle { get; set; }

        public float[] DashPattern { get; set; }

 
        public Pen GetPen(Figure figure)
        {
          
            var pen = new Pen(Color.FromArgb(Opacity, Color), Width) { DashStyle = DashStyle };
            if (DashStyle == System.Drawing.Drawing2D.DashStyle.Custom && DashPattern != null && DashPattern.Length >= 2)
                pen.DashPattern = (float[])DashPattern.Clone();
            return pen;
        }
    }

}
