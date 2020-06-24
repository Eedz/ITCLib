using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class Heading
    {
        public string Qnum { get; set; }
        public string EndQnum { get; set; }
        public string Varname { get; set; }
        public string PreP { get; set; }

       
        
        public Heading (string qnum, string prep)
        {
            Qnum = qnum;
            PreP = prep;
        }

        public override bool Equals(object obj)
        {
            var heading = obj as Heading;
            return heading != null &&
                   Varname == heading.Varname &&
                   PreP == heading.PreP;
        }

        public override int GetHashCode()
        {
            var hashCode = -1325402585;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Varname);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PreP);
            return hashCode;
        }
    }
}
