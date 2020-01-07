using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib.Reporting
{
    public class ReportQuestion : SurveyQuestion 
    {
        public string ReportVarName { get; set; }   // varname that will be displayed in report
        public string ReportQnum { get; set; }      // qnum that will be displayed in report
        public string SortBy { get; set; }          // sorting field

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
