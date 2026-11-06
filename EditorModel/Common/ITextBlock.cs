using System.Drawing;
using System.Windows.Forms;

namespace EditorModel.Common
{
    public interface ITextBlock
    {
  
        string Text { get; set; }

     
        string FontName { get; set; }

  
   
        float FontSize { get; set; }

      
        FontStyle FontStyle { get; set; }

      
        ContentAlignment Alignment { get; set; }

        Padding Padding { get; set; }

        bool WordWrap { get; set; }

    }
}
