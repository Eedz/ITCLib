using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public enum Operation { Equals = 1, NotEquals, LessThan, GreaterThan }
    public class FilterInstruction
    {
        public string VarName { get; set; }
        public List<int> Values { get; set; }
        public Operation Oper { get; set; }
        public bool Range { get; set; }
        public List<string> ValuesStr { get; set; }
        public string FilterExpression {get;set;}
        

        public FilterInstruction()
        {
            Values = new List<int>();
            ValuesStr = new List<string>();
        }

        public override string ToString()
        {
            string result = "";
            switch (Oper)
            {
                case Operation.Equals:
                    result= VarName + "=";
                    break;
                case Operation.NotEquals:
                    result = VarName + "<>";
                    break;
                case Operation.GreaterThan:
                    result = VarName + ">";
                    break;
                case Operation.LessThan:
                    result = VarName + "<";
                    break;
            }

            if (Range)
            {

               
                result += ValuesStr[0] + "-" + ValuesStr.Last();
            }
            else
            {
                if (ValuesStr.Count == 1)
                    result += ValuesStr[0];
                else
                {
                    for (int i = 0; i < ValuesStr.Count; i++)
                    {
                        if (i == ValuesStr.Count - 1)
                            result += "or " + ValuesStr[i];
                        else if (i == ValuesStr.Count - 2)
                            result += ValuesStr[i] + " ";
                        else
                            result += ValuesStr[i] + ", ";
                    }
                   
                    
                    
                }

                
            }

            return FilterExpression;
            return result;
            
        }
    }
}
