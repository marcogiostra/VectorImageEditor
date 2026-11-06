using System;
using System.Drawing;
using EditorModel.Figures;

namespace EditorModel.Style
{
    /// <summary>
    /// Класс хранения данных заливки фигуры
    /// </summary>
    [Serializable]
    public class DefaultFill : Fill
    {
        private AllowedFillDecorators _allowedFillDecorators;

       
        public DefaultFill(AllowedFillDecorators allowedDecorators = AllowedFillDecorators.All)
        {
           
            IsVisible = true;
           
            Color = Color.White;
           
            Opacity = 255;

            _allowedFillDecorators = allowedDecorators;
        }

       
        public override Brush GetBrush(Figure figure)
        {
         
            return new SolidBrush(Color.FromArgb(Opacity, Color));
        }

        
        public override AllowedFillDecorators AllowedDecorators
        {
            get { return _allowedFillDecorators; }
        }
    }

}
