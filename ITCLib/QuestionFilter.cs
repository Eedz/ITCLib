using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;

namespace ITCSurveyReportLib
{
    /// <summary>
    /// Represents the filter portion of a survey question, also known as Pre-programming instructions.
    /// </summary>
    class QuestionFilter
    {
        string filterText;  // the complete text of the filter
        bool hasVar;        // true if the filter contains a variable name
        List<FilterVar> filterVars;      // list of varnames that appear in this filter

        public QuestionFilter()
        {
            
        }

        /// <summary>
        /// Initializes a new instances of the QuestionFilter class using fields from the provided DataRow object.
        /// </summary>
        /// <param name="question"></param>
        public QuestionFilter(DataRow question)
        {

        }

        public QuestionFilter(string filter)
        {
            string var = Utilities.ExtractVarName(filter);
            filterText = filter;
            // check for vars to set hasVar flag
            if (!var.Equals(""))
                hasVar = true;
            // populate filterVars list
            filterVars = new List<FilterVar>();
            GetFilterVars();
        }

        private void GetFilterVars()
        {
            string filterVar;
          
           
            int filterVarPos;
            int filterVarLen;

            string filterExp; // the filter expression of the variable e.g. [varname]=1, 2, 8 or 9.
            
            string[] filterOptionsList;
            string options;
            

            Regex rx;
            MatchCollection results;
            
            
            while (!filterText.Equals(""))
            {
                FilterVar fv;
                filterVar = Utilities.ExtractVarName(filterText);

                if (filterVar.Equals(""))
                    break;

                rx = new Regex(filterVar + "(=|<|>|<>)" +
                            "(([0-9]+(,\\s[0-9]+)+\\sor\\s[0-9]+)" +
                            "|([0-9]+\\sor\\s[0-9]+)" +
                            "|([0-9]+\\-[0-9]+)" +
                            "|([0-9]+))");

                filterVarPos = filterText.IndexOf(filterVar);
                filterVarLen = filterVar.Length;

                   
                results = rx.Matches(filterText);

                if (results.Count > 0)
                {
                    filterExp = results[0].Value;
                    options = filterExp.Substring(filterVarLen+1);
                    options = Regex.Replace(options, "[^0-9 <->]", "");
                    
                    filterOptionsList = GetOptionList(options).Split(' ');
                    
                    fv = new FilterVar();
                    fv.Varname = filterVar;
                    if (filterOptionsList.Length != 0)
                    {
                        fv.ResponseCodes = filterOptionsList.Select(Int32.Parse).ToList();
                    }
                    // add to the list of filter vars if it is not already there
                    if (!filterVars.Contains(fv))
                        filterVars.Add(fv);

                }
                

                filterText = filterText.Substring(filterVarPos + filterVarLen);
            }
                
            

        }
    

        public string GetOptionList(string options)
        {
            string low, high;
            string list = "";

            if (options.IndexOf('-') > 0)
            {
                low = options.Substring(options.IndexOf('-') - 1, 1);
                high = options.Substring(options.IndexOf('-') + 1, 1);

                for (int i = Int32.Parse(low); i < Int32.Parse(high); i++)
                {
                    list += Convert.ToString(i);
                    if (i != Int32.Parse(high))
                    {
                        list += " ";
                    }

                }
            }
            else if (options.StartsWith("<>"))
            {
                list = options.Substring(2);
            }
            else if (options.StartsWith(">"))
            {
                list = options.Substring(1);
            }
            else if (options.StartsWith("<"))
            {
                list = options.Substring(1);
            }
            else
            {
                list = options;
            }

            while (list.IndexOf("  ")> 0)
            {
                list = Regex.Replace(list, "  ", " ");
            }

            return list;
        }

        public string FilterText { get => filterText; set => filterText = value; }
        public bool HasVar { get => hasVar; }
        internal List<FilterVar> FilterVars { get => filterVars; set => filterVars = value; }
    }
}
