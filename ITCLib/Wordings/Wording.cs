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

        public WordingType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string FieldType
        {
            get
            {
                
                switch (Type)
                {
                    case WordingType.PreP:
                        return "PreP";

                    case WordingType.PreI:
                        return "PreI";
                    case WordingType.PreA:
                        return "PreA";
                    case WordingType.LitQ:
                        return "LitQ";
                    case WordingType.PstI:
                        return "PstI";
                    case WordingType.PstP:
                        return "PstP";
                    default:
                        return null;
                }
                
            }
        }


        public string WordingText
        {
            get => _wordingText;
            set 
            {
                SetProperty(ref _wordingText, value);               
                WordingTextR = _wordingText;
                WordingTextR = Utilities.GetRtfUnicodeEscapedString(Utilities.FormatText(WordingTextR));
            }
        }

        public string WordingTextR { get; private set; }

        public Wording()
        {
            WordingText = string.Empty;
        }

        public Wording (int id, string field, string wording)
        {
            WordID = id;
            WordingText = wording;

            switch (field)
            {
                case "PreP":
                    Type = WordingType.PreP;
                    break;
                case "PreI":
                    Type = WordingType.PreI;
                    break;
                case "PreA":
                    Type = WordingType.PreA;
                    break;
                case "LitQ":
                    Type = WordingType.LitQ;
                    break;
                case "PstI":
                    Type = WordingType.PstI;
                    break;
                case "PstP":
                    Type = WordingType.PstP;
                    break;
                
            }
        }

        public Wording(int id, WordingType type, string wording)
        {
            WordID = id;
            Type = type;
            WordingText = wording;
        }

        public bool IsBlank()
        {
            return WordingText == string.Empty;
        }

        public override string ToString()
        {
            return this.FieldType + "# " +this.WordID;
        }

        public override bool Equals(object obj)
        {
            var wording = obj as Wording;
            return wording != null &&
                   WordID == wording.WordID &&
                   Type == wording.Type &&
                   WordingText == wording.WordingText;
        }

        public override int GetHashCode()
        {
            var hashCode = 612815053;
            hashCode = hashCode * -1521134295 + WordID.GetHashCode();
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(WordingText);
            return hashCode;
        }

        #region Private Backing Variables
        private int _wordid;
        private string _fieldname;
        private WordingType _type;
        private string _wordingText;
        #endregion
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
