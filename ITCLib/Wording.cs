using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ITCLib
{
    public class Wording: INotifyPropertyChanged
    {
        private int _wordid;
        public int WordID
        {
            get
            {
                return _wordid;
            }
            set
            {
                if (value != _wordid)
                {
                    _wordid = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public string FieldName { get; set; }

        private string _wordingText;
        public string WordingText
        {
            get
            {
                return _wordingText;
            }
            set
            {
                _wordingText = Utilities.FixElements(value);
                WordingTextR = Utilities.FixElements(value);
                WordingTextR = Utilities.FormatText(WordingTextR);
                NotifyPropertyChanged();
            }
        }

        public string WordingTextR { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public Wording()
        {
            FieldName = "";
            WordingText = "";
        }

        public Wording (int id, string field, string wording)
        {
            WordID = id;
            FieldName = field;
            WordingText = wording;

        }


        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
