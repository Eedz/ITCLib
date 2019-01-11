using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace ITCSurveyReportLib
{
    // a report that is based on a selection of VarNames
    // harmony (has wordings, labels, survey list)
    // var list (has headings that are surveys)
    public class VarNameBasedReport : ITCReport
    {
        public BindingList<VariableName> VarNames { get; set; }
        public List<SurveyQuestion> questions;
        

        public VarNameBasedReport()
        {

            VarNames = new BindingList<VariableName>();
            questions = new List<SurveyQuestion>();
            

        }

        

        
    }
}
