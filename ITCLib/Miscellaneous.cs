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

        public DomainLabel(int id, string label)
        {
            ID = id;
            LabelText = label;
        }

        public DomainLabel(DomainLabel domain)
        {
            ID = domain.ID;
            LabelText = domain.LabelText;
        }
    }

    public class TopicLabel
    {
        public int ID { get; set; }
        public string LabelText { get; set; }

        public TopicLabel (int id, string label)
        {
            ID = id;
            LabelText = label;
        }

        public TopicLabel(TopicLabel topic)
        {
            ID = topic.ID;
            LabelText = topic.LabelText;
        }
    }

    public class ContentLabel
    {
        public int ID { get; set; }
        public string LabelText { get; set; }

        public ContentLabel (int id, string label)
        {
            ID = id;
            LabelText = label;
        }

        public ContentLabel(ContentLabel content)
        {
            ID = content.ID;
            LabelText = content.LabelText;
        }
    }

    public class ProductLabel
    {
        public int ID { get; set; }
        public string LabelText { get; set; }

        public ProductLabel(int id, string label)
        {
            ID = id;
            LabelText = label;
        }

        public ProductLabel(ProductLabel product)
        {
            ID = product.ID;
            LabelText = product.LabelText;
        }
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
                RespListR = Utilities.FixElements(value);
                RespListR = Utilities.FormatText(RespListR);
            }
        }

        public string RespListR { get; set; }
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
