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
        string varname;
        List<int> responseCodes;
        List<string> responseLabels;

        public FilterVar()
        {

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
                && (varname == fv.varname);
                //&& (responseCodes.SequenceEqual(fv.responseCodes)); 
                //&& (responseLabels.Equals(fv.responseLabels));
        }

        public List<int> ResponseCodes { get => responseCodes; set => responseCodes = value; }
        public string Varname { get => varname; set => varname = value; }

    }
}
