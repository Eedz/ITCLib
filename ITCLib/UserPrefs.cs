using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    public class UserPrefs
    {
        string reportPath;
        public UserPrefs()
        {
            reportPath = "\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\Reports\\ISR\\";
        }
        public string ReportPath { get => reportPath; set => reportPath = value; }
    }
}
