using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class CommentType
    {
        public int ID { get; set; }
        public string TypeName { get; set; }
        public string ShortForm { get; set; }

        public CommentType()
        {
            ID = 0;
            TypeName = string.Empty;
            ShortForm = string.Empty;
        }

        public override string ToString()
        {
            return TypeName;
        }

        public override bool Equals(object obj)
        {
            var type = obj as CommentType;
            return type != null &&
                   ID == type.ID &&
                   TypeName == type.TypeName;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TypeName);
            return hashCode;
        }
    }
}
