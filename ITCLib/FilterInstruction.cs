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
        public bool AnyOf { get; set; }
        public List<string> ValuesStr { get; set; }
        public string FilterExpression {get;set;}
        

        public FilterInstruction()
        {
            Values = new List<int>();
            ValuesStr = new List<string>();
        }

        public FilterInstruction (string varname, Operation op)
        {
            VarName = varname;
            Oper = op;
            Values = new List<int>();
            ValuesStr = new List<string>();
        }

        public bool ContainsResponse(string response)
        {
            // 
            if ((response.Length == 1 && char.IsLetter(Char.Parse(response))) ||
                (response.Length == 2 && char.IsLetter(char.Parse(response.Substring(0, 1))) && char.IsLetter(char.Parse(response.Substring(1, 1)))))
            {
                // check for letter-type response (C or P usually)
                if (ValuesStr.Contains(response))
                {
                    return true;
                }
                else
                    return false;
            }

            else
            {
                if (Oper == Operation.Equals && ValuesStr.Contains(Int32.Parse(response).ToString()))
                {
                    return true;
                }
                else if (Oper == Operation.NotEquals && !ValuesStr.Contains(Int32.Parse(response).ToString()))
                {
                    return true;
                }
                else if (Oper == Operation.GreaterThan && ValuesStr.Any(y => Int32.Parse(y) < Int32.Parse(response)))
                {
                    return true;
                }
                else if (Oper == Operation.LessThan && ValuesStr.Any(y => Int32.Parse(y) > Int32.Parse(response)))
                {
                    return true;
                }
                else
                {
                    return false;
                }


                // 
                // TODO <>
                //if (list.Any(x => x.VarName.Equals(fi.VarName)))

                //{
                //    if (list.Any(x=>x.ValuesStr.Contains(Int32.Parse(fi.ValuesStr[0]).ToString())))
                //    hasFilter = true;
                //    break;
                //}
            }

            //if (char.IsLetter(response[0]))
            //{
            //    if (ValuesStr.Contains(response))
            //        return true;
            //    else
            //        return false;
            //}
            //else
            //{

            //    if (ValuesStr.Any(x => Int32.Parse(x) == Int32.Parse(response)))
            //        return true;
            //    else
            //        return false;
            //}
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
            //return result;
            
        }

        
    }
}
