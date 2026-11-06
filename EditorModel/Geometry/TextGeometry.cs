using EditorModel.Common;
using System;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace EditorModel.Geometry
{
   
    [Serializable]
    public sealed class TextGeometry : Geometry, IDisposable
    {
       
        public string Text { get; set; }

        public StringAlignment Alignment { get; set; }

       
        public string FontName { get; set; }

        
        public FontStyle FontStyle { get; set; }

        
        private readonly SerializableGraphicsPath _path = new SerializableGraphicsPath();

        
        private readonly AllowedOperations _allowedOperations;

        
        public override SerializableGraphicsPath Path
        {
            get
            {
              
                _path.Path.Reset();
               
                var text = string.IsNullOrWhiteSpace(Text) ? "(empty)" : Text;
                using (var sf = new StringFormat(StringFormat.GenericTypographic))
                {
                    sf.Alignment = Alignment;
                    _path.Path.AddString(text, new FontFamily(FontName), (int)FontStyle, 14f, PointF.Empty, sf);
                }
                var textBounds = _path.Path.GetBounds();
                var pts = _path.Path.PathPoints;
                var eps = 0.0001f;
                var kfx = (textBounds.Width < eps) ? eps : 1 / textBounds.Width;
                var kfy = (textBounds.Height < eps) ? eps : 1 / textBounds.Height;
                for (var i = 0; i < pts.Length; i++)
                {
                    pts[i].X = kfx * (pts[i].X - textBounds.Left) - 0.5f;
                    pts[i].Y = kfy * (pts[i].Y - textBounds.Top) - 0.5f;
                }
                var ptt = _path.Path.PathTypes;
                _path.Path = new GraphicsPath(pts, ptt);
                
                return _path;
            }
        }

  
        public override AllowedOperations AllowedOperations { get { return _allowedOperations; } }

     
        internal TextGeometry()
        {
            _allowedOperations = AllowedOperations.All ^ AllowedOperations.Pathed;
            Text = String.Empty;
            FontName = "Arial";
        }

        public void Dispose()
        {
            _path?.Dispose();
        }

        public override string ToString()
        {
            return "Text Line";
        }
    }
}
