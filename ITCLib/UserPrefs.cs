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
        public int userid { get; set;}
        public string Username { get; set; }
        public AccessLevel accessLevel { get; set; }
        // reporting preferences
        public string ReportPath { get; set; }
        public bool reportPrompt { get; set; }
        // other preferences
        public bool wordingNumbers { get; set; }
        public int commentDetails { get; set; }

        public Comment LastUsedComment { get; set; }
        public List<Comment> SavedComments { get; set; }
        public List<string> SavedSources { get; set; }

        // form filters
        public List<FormState> FormStates { get; set; }

        public UserPrefs()
        {
            FormStates = new List<FormState>();
        
            ReportPath = "\\\\psychfile\\psych$\\psych-lab-gfong\\SMG\\SDI\\Reports\\ISR\\";

            LastUsedComment = new Comment();
            SavedComments = new List<Comment>();
            SavedSources = new List<string>();
        }

        public FormState GetFormState(string formname, int formnum)
        {
            return FormStates.Where(x => x.FormName.Equals(formname) && x.FormNum == formnum).FirstOrDefault();
        }

        public int GetFilterID(string formname, int formnum)
        {
            var state = FormStates.Where(x => x.FormName.Equals(formname) && x.FormNum == formnum).FirstOrDefault();
            if (state == null)
                return 0;
            else
                return state.FilterID;
        }

        public string GetFilter(string formname, int formnum)
        {
            var state = FormStates.Where(x => x.FormName.Equals(formname) && x.FormNum == formnum).FirstOrDefault();
            if (state == null)
                return string.Empty;
            else
                return state.Filter;
        }
    }

    public class FormState
    {
        public int ID { get; set; }
        public int PersonnelID { get; set; }
        public string FormName { get; set; }
        public int FormNum { get; set; }
        public int RecordPosition { get; set; }
        public string Filter { get; set; }
        public int FilterID { get; set; }

        public FormState()
        {
            FormName = string.Empty;
            Filter = string.Empty;
        }
    }
}
