using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class GrantLabel
    {
        public int ID { get; set; }
        public string Code { get; set; }
        public string Label { get; set; }

        public GrantLabel()
        {
            Code = string.Empty;
            Label = string.Empty;
        }

        public override bool Equals(object obj)
        {
            var grant = obj as GrantLabel;
            return grant != null &&
                   this.Code == grant.Code;
        }

        public override int GetHashCode()
        {
            var hashCode = -1325402585;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(this.Code);
            return hashCode;
        }
    }
}
