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

        public string WordingText
        {
            get
            {
                return _wordingText;
            }
            set
            {
                _wordingText = Utilities.FixElements(value);
                WordingTextR = _wordingText;
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

        #region Private Backing Variables
        private int _wordid;
        private string _wordingText;
        #endregion
    }

    public class ResponseSet
    {

        public string RespSetName { get; set; }
        public string FieldName { get; set; }
        private string _respList;
        public string RespList
        {
            get
            {
                return _respList;
            }
            set
            {
                _respList = Utilities.FixElements(value);
                RespListR = _respList;
                RespListR = Utilities.FormatText(RespListR);
            }
        }

        public string RespListR { get; private set; }

        public ResponseSet()
        {
            FieldName = string.Empty;
            RespSetName = string.Empty;
            RespList = string.Empty;
        }
    }

    public class WordingUsage
    {
        public string VarName { get; set; }
        public string VarLabel { get; set; }
        public string SurveyCode { get; set; }
        public int WordID { get; set; }
        public string Qnum { get; set; }
        public bool Locked { get; set; }
    }

    public class ResponseUsage
    {
        public string VarName { get; set; }
        public string VarLabel { get; set; }
        public string SurveyCode { get; set; }
        public string RespName { get; set; }
        public string Qnum { get; set; }
        public bool Locked { get; set; }
    }
}
