using EditorModel.Figures;
using System;
using System.Drawing;

namespace EditorModel.Style
{
    
    [Serializable]
    [Flags]
    public enum AllowedFillDecorators : uint
    {
        None = 0x0,             // 
        LinearGradient = 0x1,   // 
        RadialGradient = 0x2,   // 
        Hatch = 0x4,            // 
        Texture = 0x8,          // 
        // 

        All = 0xffffffff,   //
    }

    [Serializable]
    public abstract class Fill
    {
     
        public virtual int Opacity { get; set; }

      
        public virtual Color Color { get; set; }

      
        public virtual bool IsVisible { get; set; }

        public abstract Brush GetBrush(Figure figure);

     
        public abstract AllowedFillDecorators AllowedDecorators { get; }
    }
}
