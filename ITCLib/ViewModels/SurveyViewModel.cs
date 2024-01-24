using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib.ViewModels
{
    public class SurveyViewModel : ViewModelBase
    {
        readonly Survey _survey;

        public SurveyViewModel(Survey survey)
        {
            base.DisplayName = "Survey";
            _survey = survey;
        }


    }
}
