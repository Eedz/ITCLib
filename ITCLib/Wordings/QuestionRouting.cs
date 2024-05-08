using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Text.RegularExpressions;

namespace ITCLib
{
    // TODO separate destination suffix (section or heading) from varname
    /// <summary>
    /// Represents the routing/skips portion of a survey question, also known as Post-programming instructions. This object also
    /// includes the response option set for the question.
    /// </summary>
    public class QuestionRouting
    {
        string [] routingText;  // the complete text of the routing
        string [] responseOptions; // the response options for the question
        bool hasVar;        // true if the routing contains a variable name
        List<RoutingVar> routingVars;      // list of varname destinations that appear in this routing instruction

        public QuestionRouting()
        {

        }


        public QuestionRouting(string routing, string respOptions, string linebreak = "<br>")
        {
            string var = Utilities.ExtractVarName(routing);
            routingText = routing.Split(new string[] { linebreak }, StringSplitOptions.RemoveEmptyEntries);
            responseOptions = respOptions.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            routingVars = new List<RoutingVar>();
            // check for vars to set hasVar flag
            if (!var.Equals(""))
            {
                hasVar = true;
            }
            else
            {
                return;
            }
            
            // populate routingVars list
            
            GetRoutingVars();
        }

        /// <summary>
        /// Isolates the variable names in the routing expression and adds them to the list of 
        /// </summary>
        private void GetRoutingVars()
        {
            RoutingVar rv;
           
            string destination;
            string numbers;
            string[] numbersArray;
            int[] numbersArrayInt;

            RoutingType routingType;
           
            Regex rx = new Regex("go to ([A-Z][A-Z][A-Z]/|[0-9][0-9][0-9][a-z]*/)*[a-zA-z][a-zA-z](\\d{5}|\\d{3})");
            MatchCollection results;
            Match m;
 
            for (int i = 0; i < routingText.Length; i++)
            {
                // get routing type, if 1 or 2, this instruction will be removed from the routing field and its routing destination will be
                // appended to the appropriate response option, if 3, this routing may be moved to the response option location
                routingType = GetRoutingType(routingText[i]);

                // start with the destination
                results = rx.Matches(routingText[i]);
                // go to next line of routing if there is no match for our pattern
                if (results.Count == 0)
                    continue;

                m = results[0];

                // the destination varname (or sometimes, section) (anything after the destination variable is formatting with a smaller font)
                destination = routingText[i].Substring(m.Index, m.Length + 1);
                if (!string.IsNullOrEmpty(routingText[i].Substring(m.Index + m.Length +1)) && routingText[i].Substring(m.Index + m.Length + 1).Length>1) destination += "<Font Size=8>" + routingText[i].Substring(m.Index + m.Length + 1) + "</Font>";

                rv = new RoutingVar();
                rv.SectionReference = destination;

                switch (routingType)
                {
                    
                    case RoutingType.IfResponse:
                        /* the most common style (If response)
                         get the numbers referenced by this instruction
                         this list of numbers should be all the numbers that would route to this instruction's destination
                        */

                        numbers = GetRoutingNumbers(routingText[i]);
                        numbersArray = numbers.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        numbersArrayInt = Array.ConvertAll(numbersArray, int.Parse);
                             
                        rv.Varname = destination.Replace("go to ", "");
                        rv.Varname = rv.Varname.Replace(".", "");
                        int sp = rv.Varname.IndexOf(" ");
                        if (rv.Varname.Contains(" "))
                            rv.Varname = rv.Varname.Substring(0, sp);

                        rv.ResponseCodes = numbersArrayInt.ToList<int>();
                        if (!routingVars.Contains(rv))
                            routingVars.Add(rv);

                        routingText[i] = "";
                        break;
                    case RoutingType.Otherwise:
                        rv.Varname = destination.Replace("go to ", "");
                        rv.Varname = rv.Varname.Replace(".", "");
                        sp = rv.Varname.IndexOf(" ");
                        if (rv.Varname.Contains(" "))
                            rv.Varname = rv.Varname.Substring(0, sp);

                        for (int r = 0; r < routingVars.Count; r++)
                        {
                            for (int s = 0; s < responseOptions.Length; s++)
                            {
                                if (!routingVars[r].ResponseCodes.Contains(ResponseNumber(responseOptions[s])))
                                {
                                    rv.ResponseCodes.Add(ResponseNumber(responseOptions[s]));
                                }
                            }
                        }

                        if (!routingVars.Contains(rv))
                            routingVars.Add(rv);
                        break;
                    case RoutingType.If:
                        rv.Varname = destination.Replace("go to ", "");
                        rv.Varname = rv.Varname.Replace(".", "");
                        sp = rv.Varname.IndexOf(" ");
                        if (rv.Varname.Contains(" "))
                            rv.Varname = rv.Varname.Substring(0, sp);

                        rv.ResponseCodes.Add(0);
                        break;
                    case RoutingType.Other:
                        break;

                }

            }

            
        }

        /// <summary>
        /// Returns a list of numbers separated by a space that exist between "If response" and "go to"
        /// </summary>
        /// <param name="routingInstruction"></param>
        /// <returns></returns>
        private string GetRoutingNumbers(string routingInstruction)
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
            routingNumbers = Regex.Replace(routingNumbers, "[^0-9 ]", " ");
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
            string ineq= "";
            double d = 0;
            while (!Double.TryParse(s.Substring(0, 1), out d)) {
                ineq += s.Substring(1,1);
                s = s.Substring(2);
            }
            ineq = Regex.Replace(ineq, ",", "");
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


            for (int i = 0; i < arr.Length; i++) { 
                if (arr[i].Contains('-')) { // for any range, change this member of the array into a sub-list of numbers
                    low = Int32.Parse(arr[i].Substring(0, arr[i].IndexOf("-") - 1)); // low end of range
                    high = Int32.Parse(arr[i].Substring(arr[i].IndexOf("-") + 1)); // high end of range
                    arr[i] = "";
                    for (int x = low; x < high; x++) {
                        arr[i] = arr[i] + x + ",";
                    }
                    arr[i] = arr[i].Trim(new char[] { ',' });
                }
            }
            // join the array back into a string
            return string.Join(",",arr);
        }

        /// <summary>
        /// Get routing type, if 1 or 2, this instruction will be removed from the routing field and its routing destination will be
        /// appended to the appropriate response option, if 3, this routing may be moved to the response option location
        /// </summary>
        /// <param name="routingExpression"></param>
        /// <returns></returns>
        public RoutingType GetRoutingType (string routingExpression)
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
                if (Double.TryParse(respOption.Substring(i, 1), out d))
                {
                    number += respOption.Substring(i, 1);
                }
            }
            return Int32.Parse(number);
        }

        public string GetDestination(string response)
        {
            string result;
            result = response;
            foreach (RoutingVar rv in routingVars)
            {
                if (rv.ResponseCodes.Contains(ResponseNumber(response)))
                {
                    result += " <strong>=> go to " + rv.Varname + "</strong>";
                    break;
                }
            }
            return result;
        }

        public override string ToString()
        {
            string output = "";
            for (int i = 0; i < responseOptions.Length; i++)
            {
                output += GetDestination(responseOptions[i]) + "\r\n";
            }
            output = output.Trim("\r\n".ToCharArray());
            return output;
        }

        public string[] RoutingText { get => routingText; set => routingText = value; }
        public bool HasVar { get => hasVar; }
        public List<RoutingVar> RoutingVars { get => routingVars; set => routingVars = value; }
    }
}

