using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class SurveyUserGroup
    {
        public int ID { get; set; }
        public string UserGroup { get; set; }
        public string Code { get; set; }
        public string WebName { get; set; }

        public SurveyUserGroup()
        {

        }
    }

    public class SurveyCohort
    {
        public int ID { get; set; }
        public string Cohort { get; set; }
        public string Code { get; set; }
        public string WebName { get; set; }

        public SurveyCohort()
        {

        }
    }

    public class SurveyMode
    {
        public int ID { get; set; }
        public string Mode { get; set; }
        public string ModeAbbrev { get; set; }
    }

    public class DomainLabel
    {
        public int ID { get; set; }
        public string LabelText { get; set; }
    }

    public class TopicLabel
    {
        public int ID { get; set; }
        public string LabelText { get; set; }
    }

    public class ContentLabel
    {
        public int ID { get; set; }
        public string LabelText { get; set; }
    }

    public class ProductLabel
    {
        public int ID { get; set; }
        public string LabelText { get; set; }
    }

    public class Wording
    {
        public int ID { get; set; }
        public int WordID { get; set; }
        public string FieldName { get; set; }
        private string _wordingText;
        public string WordingText {
            get
            {
                return _wordingText;
            }
            set
            {
                _wordingText = Utilities.FixElements(value);
                WordingTextR = Utilities.FixElements(value);
                WordingTextR = Utilities.FormatText(WordingTextR);
            }
        }
    
        public string WordingTextR { get; set; }
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


}
