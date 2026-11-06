using System;
using System.Drawing;
using EditorModel.Figures;

namespace EditorModel.Renderers
{
    
    [Serializable]
    [Flags]
    public enum AllowedRendererDecorators : uint
    {
        None = 0x0,         // 
        Shadow = 0x1,       // 
        Glow = 0x2,         // 
        TextBlock = 0x4,    // 
        Warp = 0x8,         // 
        Arrow = 0x10,       // 
        // 

        All = 0xffffffff,   // 
    }

   
    [Serializable]
    public abstract class Renderer
    {
        
        public abstract void Render(Graphics graphics, Figure figure);

        
        public abstract AllowedRendererDecorators AllowedDecorators { get; }
    }
}
