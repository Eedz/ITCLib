using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    public class Heading
    {
        string qnum;
        string varname;
        string prep;

        public string Qnum { get => qnum; set => qnum = value; }
        public string Prep { get => prep; set => prep = value; }
    }
}
