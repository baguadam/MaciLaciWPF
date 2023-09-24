using MaciLaci.Persistence;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks; 

namespace MaciLaci.WPF.ViewModel
{
    public class MaciLaciField : ViewModelBase
    {
        private FieldOptions _fieldOptions;
        private String _text = String.Empty;

        public FieldOptions FieldOptions
        {
            get { return _fieldOptions; }
            set
            {
                _fieldOptions = value;
                OnPropertyChanged(nameof(FieldOptions));
            }
        }

        public Int32 X { get; set; }

        public Int32 Y { get; set; }

        public Int32 Number { get; set; }

        public String Text { 
            get { return _text; }
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
