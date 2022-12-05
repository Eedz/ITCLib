using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public enum Comparity { Equals, Contains, EndsWith, StartsWith, GreaterThan, LessThan }
    public enum LogicalOperator { AND, OR }
    public class SearchCriterium
    {
        public string Field { get; set; }
        public List<string> Fields { get; set; }
        public Comparity Compare { get; set; }
        public List<string> Criteria { get; set; }
        public bool Negate { get; set; }
        //public LogicalOperator LogicOperator { get; set; } 

        public SearchCriterium()
        {
            Field = string.Empty;
            Compare = Comparity.Equals;
            Criteria = new List<string>();
            Fields = new List<string>();
        }

        public SearchCriterium(string field, Comparity compare, List<string> criteria)
        {
            Field = field;
            Fields = new List<string>() { field };
            Compare = compare;
            Criteria = criteria;
            
        }

        public SearchCriterium(string field, Comparity compare, List<string> criteria, bool negate)
        {
            Field = field;
            Fields = new List<string>() { field };
            Compare = compare;
            Criteria = criteria;
            Negate = negate;
        }

        public SearchCriterium(List<string> fields, Comparity compare, List<string> criteria)
        {
            Fields = fields;
            Compare = compare;
            Criteria = criteria;
         }

        public SearchCriterium(List<string> fields, Comparity compare, List<string> criteria, bool negate)
        {
            Fields = fields;
            Compare = compare;
            Criteria = criteria;
            Negate = negate;
        }

        public string GetParameterizedCondition(int tagNumber)
        {
            if (Criteria.Count == 0)
                return "";

            if (Fields.Count == 0)
                return "";

            StringBuilder sb = new StringBuilder();
            
            if (Negate) sb.Append(" NOT ");

            sb.Append("(");
            foreach (string f in Fields)
            {
                sb.Append("(");
                foreach (string s in Criteria)
                {

                    sb.Append(f);

                    switch (Compare)
                    {
                        case Comparity.Equals:
                            sb.Append(" = @tag");
                            sb.Append(tagNumber);
                            break;
                        case Comparity.Contains:
                            sb.Append(" LIKE '%' + @tag");
                            sb.Append(tagNumber);
                            sb.Append(" + '%'");
                            break;
                        case Comparity.EndsWith:
                            sb.Append(" LIKE '%' + @tag" + tagNumber);
                            break;
                        case Comparity.StartsWith:
                            sb.Append(" LIKE tag" + tagNumber + " + '%'");
                            break;
                        case Comparity.GreaterThan:
                            sb.Append(" >= tag" + tagNumber);
                            break;
                        case Comparity.LessThan:
                            sb.Append(" <= tag" + tagNumber);
                            break;
                    }
                    sb.Append(" OR ");
                    tagNumber++;
                }
                sb.Remove(sb.Length - 4, 4);
                sb.Append(") OR ");
            }
            
            sb.Remove(sb.Length - 4, 4);
            sb.Append(")");
            return sb.ToString();
        }

    }
}
