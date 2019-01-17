using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    public class UserPrefs
    {
        // user info
        public int userid;
        public string username;
        public int accessLevel;
        // reporting preferences
        public string reportPath;
        public bool reportPrompt;
        // other preferences
        public bool wordingNumbers;

        public UserPrefs()
        {
           
            reportPath = "\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\Reports\\ISR\\";
        }

       
    }
}
