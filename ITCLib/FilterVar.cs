using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    /// <summary>
    /// Represents a VarName that appears in a filter expression. A FilterVar can have 0 or more response codes and labels.
    /// TODO better equatable checking
    /// </summary>
    class FilterVar : IEquatable<FilterVar>
    {
        public string Varname { get; set; }
        public List<int> ResponseCodes { get; set; }
        List<string> responseLabels;

        public FilterVar()
        {
            ResponseCodes = new List<int>();
            responseLabels = new List<string>();
        }

        public FilterVar(string filterExpression)
        {
            // find first varname
            // get options
            // get labels
        }

        public bool Equals(FilterVar obj)
        {
            FilterVar fv = obj as FilterVar;
            return (fv != null)
                && (Varname == fv.Varname);
                //&& (responseCodes.SequenceEqual(fv.responseCodes)); 
                //&& (responseLabels.Equals(fv.responseLabels));
        }

        
        

    }
}
