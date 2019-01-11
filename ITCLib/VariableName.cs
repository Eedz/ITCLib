using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    public class VariableName
    {
        public string VarName { get; set; }
        public string refVarName { get; set; }
        public string VarLabel { get; set; }
        public string DomainLabel { get; set; }
        public string TopicLabel { get; set; }
        public string ContentLabel { get; set; }
        public string ProductLabel { get; set; }

        public VariableName()
        {

        }

        public VariableName(string varname)
        {
            VarName = varname;

            refVarName = Utilities.ChangeCC(varname);
        }
    }
}
