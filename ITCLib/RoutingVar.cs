using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace ITCSurveyReportLib
{
    /// <summary>
    /// Represents a VarName that appears in a routing expression. A RoutingVar can have 0 or more response codes and labels.
    /// TODO better equatable checking
    /// </summary>
    class RoutingVar : IEquatable<RoutingVar>
    {
        string varname; // the destination for this routing instruction
        string sectionReference;
        List<int> responseCodes;
        List<string> responseLabels;

        public RoutingVar()
        {

        }

        // TODO finish this or abandon?
        public RoutingVar(string routingExpression, string respOptions)
        {
            // find first varname
            Regex rx = new Regex("go to ([A-Z][A-Z][A-Z]/|[0-9][0-9][0-9][a-z]*/)*[a-zA-z][a-zA-z](\\d{ 5}|\\d{ 3})");
            MatchCollection results;
            Match m;
            RoutingType rtype = 0;
            // start with the destination
            results = rx.Matches(routingExpression);
            
            // if there is no varname in the routing expression, this object's properties are null
            if (results.Count == 0)
            {
                varname = null;
                responseCodes = null;
                responseLabels = null;
                return;
            }
                
            m = results[0];
            // isolate the varname destination
            varname = routingExpression.Substring(m.Index, m.Length + 1);
            // anything after the varname is stored in the sectionReference member.
            sectionReference = "<Font Size=8>" + routingExpression.Substring(m.Index + m.Length + 1) + "</Font>";

            // get options
            rtype = GetRoutingType(routingExpression);
            switch (rtype)
            {
                case RoutingType.IfResponse:
                    break;
                case RoutingType.Otherwise:
                    break;
                case RoutingType.If:
                    break;
                case RoutingType.Other:
                    break;

            }
            // get labels
        }

        /// <summary>
        /// Returns a list of numbers separated by a space that exist between "If response" and "go to"
        /// </summary>
        /// <param name="routingInstruction"></param>
        /// <returns></returns>
        private string GetRoutingNumbers(string routingInstruction, string [] responseOptions)
        {

            string routingNumbers;
            string[] numberArr;
            string oper;
            string numberList = "";
            int ifResponsePos = routingInstruction.IndexOf("If response");
            int gotoPos = routingInstruction.IndexOf("go to");
            // exit if If response and go to are not present
            if (ifResponsePos == 0 && gotoPos == 0)
                return "";

            routingNumbers = routingInstruction.Substring(ifResponsePos, gotoPos);
            routingNumbers = Regex.Replace(routingNumbers, "[^0-9 =<->,]", "");
            // get the operation before the list of numbers (=, >, or <)
            oper = GetOperation(routingNumbers);
            // check for range
            if (routingNumbers.Contains("-"))
            {
                routingNumbers = FillRange(routingNumbers);
            }
            routingNumbers = Regex.Replace(routingNumbers, ",", " ");
            numberArr = routingNumbers.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            // depending on the operation, get the numbers that will lead to this routing instruction
            switch (oper)
            {
                case "=":
                    numberList = Regex.Replace(routingNumbers, " ", ",");
                    break;
                case ">":
                    for (int i = 0; i < responseOptions.Length; i++)
                    {
                        for (int j = 0; j < numberArr.Length; j++)
                        {
                            if (Int32.Parse(numberArr[j]) < ResponseNumber(responseOptions[i]))
                                numberList += "," + numberArr[j];
                        }
                    }
                    break;
                case "<":
                    for (int i = 0; i < responseOptions.Length; i++)
                    {
                        for (int j = 0; j < numberArr.Length; j++)
                        {
                            if (Int32.Parse(numberArr[j]) < ResponseNumber(responseOptions[i]))
                                numberList += "," + numberArr[j];
                        }
                    }
                    break;
                case "<>":
                    for (int i = 0; i < responseOptions.Length; i++)
                    {
                        for (int j = 0; j < numberArr.Length; j++)
                        {
                            if (Int32.Parse(numberArr[j]) != ResponseNumber(responseOptions[i]))
                                numberList += "," + numberArr[j];
                        }
                    }
                    break;

                default:
                    break;
            }

            return numberList;
        }

        /// <summary>
        /// Returns the non-numeric characters from beginning of the string.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        private string GetOperation(string s)
        {
            string ineq = "";
            double d = 0;
            while (!Double.TryParse(s.Substring(1, 1), out d))
            {
                ineq += s.Substring(1, 1);
                s = s.Substring(2);
            }
            return ineq;
        }

        /// <summary>
        /// Given an expression containing a dash surrounded by numbers, this method fills in the intermediate numbers. 
        /// </summary>
        /// <param name="numbers"></param>
        /// <returns></returns>
        public string FillRange(string numbers)
        {
            int low;
            int high;
            string[] arr;

            arr = numbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);  // split the list into its members


            for (int i = 0; i < arr.Length; i++)
            {
                if (arr[i].Contains('-'))
                { // for any range, change this member of the array into a sub-list of numbers
                    low = Int32.Parse(arr[i].Substring(0, arr[i].IndexOf("-") - 1)); // low end of range
                    high = Int32.Parse(arr[i].Substring(arr[i].IndexOf("-") + 1)); // high end of range
                    arr[i] = "";
                    for (int x = low; x < high; x++)
                    {
                        arr[i] = arr[i] + x + ",";
                    }
                    arr[i] = Utilities.TrimString(arr[i], ",");
                }
            }
            // join the array back into a string
            return string.Join(",", arr);
        }

        /// <summary>
        /// Returns the response number for a given response option.
        /// </summary>
        /// <param name="respOption"></param>
        /// <returns>int</returns>
        public int ResponseNumber(string respOption)
        {
            Double d = 0;
            string number = "";
            for (int i = 0; i < respOption.Length; i++)
            {
                if (!Double.TryParse(respOption.Substring(i, 1), out d))
                {
                    number += respOption.Substring(i, 1);
                }
            }
            return Int32.Parse(number);
        }

        public bool Equals(RoutingVar obj)
        {
            RoutingVar fv = obj as RoutingVar;
            return (fv != null)
                && (varname == fv.varname);
            //&& (responseCodes.SequenceEqual(fv.responseCodes)); 
            //&& (responseLabels.Equals(fv.responseLabels));
        }

        /// <summary>
        /// Get routing type, if 1 or 2, this instruction will be removed from the routing field and its routing destination will be
        /// appended to the appropriate response option, if 3, this routing may be moved to the response option location
        /// </summary>
        /// <param name="routingExpression"></param>
        /// <returns></returns>
        public RoutingType GetRoutingType(string routingExpression)
        {
            RoutingType routingType;
            if (routingExpression.StartsWith("If response"))
            {
                routingType = RoutingType.IfResponse;
            }
            else if (routingExpression.StartsWith("Otherwise"))
            {
                routingType = RoutingType.Otherwise;
            }
            else if (routingExpression.StartsWith("If"))
            {
                routingType = RoutingType.If;
            }
            else
            {
                routingType = RoutingType.Other;
            }
            return routingType;
        }

        public List<int> ResponseCodes { get => responseCodes; set => responseCodes = value; }
        public string Varname { get => varname; set => varname = value; }

    }
}

