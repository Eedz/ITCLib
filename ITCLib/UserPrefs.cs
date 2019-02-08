using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class UserPrefs
    {
        // user info
        public int userid;
        public string Username { get; set; }
        public AccessLevel accessLevel { get; set; }
        // reporting preferences
        public string ReportPath { get; set; }
        public bool reportPrompt { get; set; }
        // other preferences
        public bool wordingNumbers { get; set; }
        public int commentDetails { get; set; }

        public UserPrefs()
        {
           
            ReportPath = "\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\Access\\Reports\\ISR\\";
        }

       
    }
}
