using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    public class Wording : ObservableObject
    {
        public int WordID
        {
            get => _wordid;
            set => SetProperty(ref _wordid, value);
        }

        public string FieldName 
        { 
            get => _fieldname;
            set => SetProperty(ref _fieldname, value);
        }

        public string WordingText
        {
            get => _wordingText;
            set 
            {
                SetProperty(ref _wordingText, value);               
                WordingTextR = _wordingText;
                WordingTextR = Utilities.FormatText(WordingTextR);
            }
        }

        public string WordingTextR { get; private set; }

        public Wording()
        {
            FieldName = string.Empty;
            WordingText = string.Empty;
        }

        public Wording (int id, string field, string wording)
        {
            WordID = id;
            FieldName = field;
            WordingText = wording;
        }

        public bool IsBlank()
        {
            return WordingText == string.Empty;
        }

        public override string ToString()
        {
            return this.FieldName + "# " +this.WordID;
        }

        #region Private Backing Variables
        private int _wordid;
        private string _fieldname;
        private string _wordingText;
        #endregion
    }

    public class ResponseSet : ObservableObject
    {
        public string RespSetName { 
            get => _respsetname; 
            set => SetProperty(ref _respsetname, value); 
        }

        public string FieldName {
            get => _fieldname;
            set => SetProperty(ref _fieldname, value);
        }
        
        public string RespList
        {
            get => _respList;
            set
            {
                SetProperty (ref _respList, value.Replace("&nbsp;", " "));
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

        private string _respsetname;
        private string _fieldname;
        private string _respList;
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
