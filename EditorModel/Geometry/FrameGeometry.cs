using EditorModel.Common;
using System;
using System.Drawing;

namespace EditorModel.Geometry
{
    /// <summary>
    /// —одержит геометрию рамки, измен€ющей свои размеры
    /// </summary>
    [Serializable]
    public sealed class FrameGeometry : Geometry, IDisposable
    {
        
        private readonly SerializableGraphicsPath _path = new SerializableGraphicsPath();

       
        public override AllowedOperations AllowedOperations { get { return AllowedOperations.None; } }

        public override SerializableGraphicsPath Path
        {
            get
            {
                _path.Path.Reset();
                // 
                var minX = Math.Min(StartPoint.X, EndPoint.X);
                var maxX = Math.Max(StartPoint.X, EndPoint.X);
                var minY = Math.Min(StartPoint.Y, EndPoint.Y);
                var maxY = Math.Max(StartPoint.Y, EndPoint.Y);
                var rect = new Rectangle(minX, minY, maxX - minX + 1, maxY - minY + 1);
                _path.Path.AddRectangle(rect);
                return _path;
            }
        }

        public Point StartPoint { get; set; }
        public Point EndPoint { get; set; }

   
        internal FrameGeometry(Point startPoint)
        {
            EndPoint = StartPoint = startPoint;
        }

        public void Dispose()
        {
            _path?.Dispose();
        }
    }
}