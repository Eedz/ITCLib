using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class VariableName
    {
        public string VarName { get; set; }
        public string refVarName { get; set; }
        public string VarLabel { get; set; }
        public DomainLabel Domain { get; set; }
        public TopicLabel Topic { get; set; }
        public ContentLabel Content { get; set; }
        public ProductLabel Product { get; set; }

        public VariableName(string varname)
        {
            VarName = varname;

            refVarName = Utilities.ChangeCC(varname);
        }
    }

    public class RefVariableName
    {

        public string refVarName { get; set; }
        public string VarLabel { get; set; }
        public DomainLabel Domain { get; set; }
        public TopicLabel Topic { get; set; }
        public ContentLabel Content { get; set; }
        public ProductLabel Product { get; set; }

        public RefVariableName(string refvarname)
        {
            

            refVarName = refvarname;
        }
    }
}
