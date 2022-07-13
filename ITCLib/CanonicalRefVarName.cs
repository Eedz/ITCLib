using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class CanonicalRefVarName : RefVariableName
    {
        public bool AnySuffix { get; set; }
        public string Notes { get; set; }
        public bool Active { get; set; }

        public CanonicalRefVarName () : base()
        {
            Notes = "";
        }
    }
}
