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

        public WordingType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        public string FieldType
        {
            get
            {
                if (!string.IsNullOrEmpty(FieldName))
                    return FieldName;
                else
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
            FieldName = string.Empty;
            WordingText = string.Empty;
        }

        public Wording (int id, string field, string wording)
        {
            WordID = id;
            FieldName = field;
            WordingText = wording;
        }

        public Wording(int id, WordingType type, string wording)
        {
            WordID = id;
            Type = type;
            switch (type)
            {
                case WordingType.PreP:
                    FieldName = "PreP";
                    break;
                case WordingType.PreI:
                    FieldName = "PreI";
                    break;
                case WordingType.PreA:
                    FieldName = "PreA";
                    break;
                case WordingType.LitQ:
                    FieldName = "LitQ";
                    break;
                case WordingType.PstI:
                    FieldName = "PstI";
                    break;
                case WordingType.PstP:
                    FieldName = "PstP";
                    break;

            }
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
        private WordingType _type;
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

        public ResponseType Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
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

        public ResponseSet(string setname, ResponseType type, string responseText)
        {
            RespSetName = setname;
            FieldName = string.Empty;
            Type = type;
            switch (type)
            {
                case ResponseType.RespOptions:
                    FieldName = "RespOptions";
                    break;
                case ResponseType.NRCodes:
                    FieldName = "NRCodes";
                    break;
            }
            RespList = responseText;
        }

        public void SetRandomName()
        {
            this.RespSetName = GenerateRandomString(5);
        }

        string GenerateRandomString(int length)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyz";
            Random random = new Random();
            char[] stringChars = new char[length];

            for (int i = 0; i < length; i++)
            {
                stringChars[i] = chars[random.Next(chars.Length)];
            }

            return new string(stringChars);
        }

        private string _respsetname;
        private string _fieldname;
        private ResponseType _type;
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
