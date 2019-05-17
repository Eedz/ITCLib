using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Data;

namespace ITCLib
{
    /// <summary>
    /// Represents the filter portion of a survey question, also known as Pre-programming instructions.
    /// </summary>
    class QuestionFilter
    {
        string FilterText { get; set; }  // the complete text of the filter
        public List<FilterVar> FilterVars { get; set; }      // list of varnames that appear in this filter

        public QuestionFilter()
        {
            
        }


        /// <summary>
        /// Builds a QuestionFilter object using the provided filter and looks for any VarNames contained in the list.
        /// </summary>
        /// <param name="filter"></param>
        /// <param name="questions"></param>
        public QuestionFilter(string filter, List<SurveyQuestion> questions)
        {
            FilterText = filter;
            // populate filterVars list
            FilterVars = new List<FilterVar>();
            GetFilterVars(questions);
        }

        public QuestionFilter(string filter)
        {
          
            FilterText = filter;
      
            // populate filterVars list
            FilterVars = new List<FilterVar>();
            GetFilterVars();
        }

        /// <summary>
        /// Populates the list of filter vars with those that match the VarNames found in the provided list of questions.
        /// </summary>
        /// <param name="questions"></param>
        private void GetFilterVars(List<SurveyQuestion> questions)
        {
            string filterVar;

            int filterVarPos=0;
            int filterVarLen;

            string filterExp; // the filter expression of the variable e.g. [varname]=1, 2, 8 or 9.

            string[] filterOptionsList;
            string options;

            Regex rx;
            MatchCollection results;

            while (!FilterText.Equals(""))
            {
                FilterVar fv;
                foreach (SurveyQuestion q in questions)
                {
                    filterVar = q.RefVarName;

                    if (!FilterText.Contains(filterVar))
                        continue;

                    rx = new Regex(filterVar + "(=|<|>|<>)" +
                                "(([0-9]+(,\\s[0-9]+)+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\-[0-9]+)" +
                                "|([0-9]+))");

                    filterVarPos = FilterText.IndexOf(filterVar);
                    filterVarLen = filterVar.Length;

                    results = rx.Matches(FilterText);

                    if (results.Count > 0)
                    {
                        filterExp = results[0].Value;
                        options = filterExp.Substring(filterVarLen + 1);
                        options = Regex.Replace(options, "[^0-9 <->]", "");

                        filterOptionsList = GetOptionList(options).Split(' ');

                        fv = new FilterVar();
                        fv.Varname = filterVar;
                        if (filterOptionsList.Length != 0)
                        {
                            fv.ResponseCodes = filterOptionsList.Select(Int32.Parse).ToList();
                        }
                        // add to the list of filter vars if it is not already there
                        if (!FilterVars.Contains(fv))
                            FilterVars.Add(fv);


                    }
                    else
                    {
                        filterVarPos = 0;
                        continue;
                    }
                    filterVarPos = 0;
                    // trim the filterText to the point after this VarName
                    FilterText = FilterText.Substring(filterVarPos + filterVarLen);
                }
                if (filterVarPos == 0)
                    break;
                
            }
        }

        /// <summary>
        /// Populates the list of filter vars with standard VarNames found in the filter.
        /// </summary>
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
            
            
            while (!FilterText.Equals(""))
            {
                FilterVar fv;
                filterVar = Utilities.ExtractVarName(FilterText);

                if (filterVar.Equals(""))
                    break;

                rx = new Regex(filterVar + "(=|<|>|<>)" +
                            "(([0-9]+(,\\s[0-9]+)+\\sor\\s[0-9]+)" +
                            "|([0-9]+\\sor\\s[0-9]+)" +
                            "|([0-9]+\\-[0-9]+)" +
                            "|([0-9]+))");

                filterVarPos = FilterText.IndexOf(filterVar);
                filterVarLen = filterVar.Length;

                   
                results = rx.Matches(FilterText);

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
                    if (!FilterVars.Contains(fv))
                        FilterVars.Add(fv);

                }
                

                FilterText = FilterText.Substring(filterVarPos + filterVarLen);
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

    }
}
