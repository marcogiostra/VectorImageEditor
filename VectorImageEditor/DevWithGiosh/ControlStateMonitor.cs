using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace VectorImageEditor.DevWithGiosh
{
    
    public class ControlStateChangedEventArgs : EventArgs
    {
        public string NomeControl { get; }
        public bool IsEnabled { get; }

        public ControlStateChangedEventArgs(string control, bool isEnabled)
        {
            NomeControl = control;
            IsEnabled = isEnabled;
        }
    }
    

}
