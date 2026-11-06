using EditorModel.Common;
using System;

namespace EditorModel.Geometry
{
    
    [Serializable]
    [Flags]
    public enum AllowedOperations : uint
    {
        None = 0x0,         // 
        Scale = 0x1,        // 
        Size = 0x2,         // 
        Rotate = 0x4,       // 
        Select = 0x8,       //
        Skew = 0x10,        // 
        Vertex = 0x20,      // 
        Pathed = 0x40,      // 
        Warp = 0x80,        // 
        // 

        All = 0xffffffff,   // 
    }
    

    [Serializable]
    public abstract class Geometry
    {
        public string Name { get; set; }

        public bool Visible { get; set; }
        public string Title { get; set; }
  
        public abstract SerializableGraphicsPath Path { get; }
          
        public abstract AllowedOperations AllowedOperations { get; }

    }
}
