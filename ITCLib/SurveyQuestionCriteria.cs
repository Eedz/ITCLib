using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class SurveyQuestionCriteria
    {
        public List<int> DomainNums { get; set; }
        public List<int> TopicNums { get; set; }
        public List<int> ContentNums { get; set; }
        public List<int> ProductNums { get; set; }
        

        public SurveyQuestionCriteria()
        {
            DomainNums = new List<int>();
            TopicNums = new List<int>();
            ContentNums = new List<int>();
            ProductNums = new List<int>();
            
        }

        public string GetCriteria()
        {
            StringBuilder sb = new StringBuilder();

            if (DomainNums.Count > 0)
                sb.Append(" AND DomainNum IN (" + string.Join(",", DomainNums) + ")");
            if (TopicNums.Count>0)
                sb.Append(" AND TopicNum IN (" + string.Join(",", TopicNums) + ")");
            if (ContentNums.Count > 0)
                sb.Append(" AND ContentNum IN (" + string.Join(",", ContentNums) + ")");
            if (ProductNums.Count > 0)
                sb.Append(" AND ProductNum IN (" + string.Join(",", ProductNums) + ")");

            sb.Remove(0, 4);

            return sb.ToString();
        }
    }
}
