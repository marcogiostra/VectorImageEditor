using System;

namespace EditorModel.Style
{
    /// <summary>
    /// Класс стилей фигуры
    /// </summary>
    [Serializable]
    public class Style
    {
        
        public Border BorderStyle { get; set; }

        
        public Fill FillStyle { get; set; }

       
        public Style()
        {
            BorderStyle = new Border();
            FillStyle = new DefaultFill();
        }
    }
}
