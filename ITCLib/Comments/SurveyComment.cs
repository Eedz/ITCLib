using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class SurveyComment : Comment
    {

        public int SurvID { get => _survid; set => SetProperty(ref _survid, value); }
        public string Survey { get => _survey; set => SetProperty(ref _survey, value); }

        public SurveyComment() : base()
        {
            Survey = string.Empty;
        }

        private int _survid;
        private string _survey;
    }
}
