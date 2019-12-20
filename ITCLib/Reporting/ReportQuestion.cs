using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib.Reporting
{
    public class ReportQuestion : SurveyQuestion 
    {
        public string ReportVarName { get; set; }
        public string ReportQnum { get; set; }
        public string SortBy { get; set; }

        public ReportQuestion() : base ()
        {
            ReportVarName = "";
            ReportQnum = "";
            SortBy = "";
        }

        public ReportQuestion(string varname) :base(varname)
        {
            ReportVarName = varname;
            ReportQnum = "";
            SortBy = "";
        }

        public ReportQuestion(string varname, string qnum) : base(varname, qnum)
        {
            ReportVarName = varname;
            ReportQnum = qnum;
            SortBy = qnum;
        }
    }
}
