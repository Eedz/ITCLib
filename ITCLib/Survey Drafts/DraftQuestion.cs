using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class DraftQuestion
    {
        public string Qnum { get; set; }
        public float SortBy { get; set; }
        public string AltQnum { get; set; }
        public string VarName { get; set; }
        public string RefVarName { get; set; }
        private string _questionText;
        public string QuestionText
        {
            get { return _questionText; } set
            {
                _questionText = value;
                QuestionTextRTF = Utilities.FormatText(value);
            }
        }
        public string QuestionTextRTF { get; private set; }

        private string _comments;
        public string Comments {
            get { return _comments; }
            set
            {
                _comments = value;
                CommentsRTF = Utilities.FormatText(value);
            }
        }
        public string CommentsRTF { get; private set; }

        private string _extra1;
        public string Extra1 {
            get { return _extra1; }
            set
            {
                _extra1 = value;
                Extra1RTF = Utilities.FormatText(value);
            }
        }
        public string Extra1RTF { get; private set; }
        

        private string _extra2;
        public string Extra2 {
            get { return _extra2; }
            set
            {
                _extra2 = value;
                Extra2RTF = Utilities.FormatText(value);
            }
        }
        public string Extra2RTF { get; private set; }

        private string _extra3;
        public string Extra3
        {
            get { return _extra3; }
            set
            {
                _extra3 = value;
                Extra3RTF = Utilities.FormatText(value);
            }
        }
        public string Extra3RTF { get; private set; }

        private string _extra4;
        public string Extra4
        {
            get { return _extra4; }
            set
            {
                _extra4 = value;
                Extra4RTF = Utilities.FormatText(value);
            }
        }
        public string Extra4RTF { get; private set; }

        private string _extra5;
        public string Extra5
        {
            get { return _extra5; }
            set
            {
                _extra5 = value;
                Extra5RTF = Utilities.FormatText(value);
            }
        }
        public string Extra5RTF { get; private set; }

        public bool Deleted { get; set; }
        public bool Inserted { get; set; }

        public DraftQuestion()
        {
            Qnum = "";
            
            AltQnum = "";
            VarName = "";
            RefVarName = "";
            QuestionText = "";
            Comments = "";
            Extra1 = "";
            Extra2 = "";
            Extra3 = "";
            Extra4 = "";
            Extra5 = "";

        }

        public string GetExtraFieldData(int fieldNumber)
        {
            switch (fieldNumber)
            {
                case 1:
                    return Extra1;
                case 2:
                    return Extra2;
                case 3:
                    return Extra3;
                case 4:
                    return Extra4;
                case 5:
                    return Extra5;
                default:
                    return string.Empty;

            }
        }

        public string GetExtraFieldDataRTF(int fieldNumber)
        {
            switch (fieldNumber)
            {
                case 1:
                    return Extra1RTF;
                case 2:
                    return Extra2RTF;
                case 3:
                    return Extra3RTF;
                case 4:
                    return Extra4RTF;
                case 5:
                    return Extra5RTF;
                default:
                    return string.Empty;

            }
        }
    }
}
