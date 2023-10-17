using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class VarNameChangeNotification
    {
        public int ID { get; set; }
        public int ChangeID { get; set; }
        public Person Name { get; set; }
        public string NotifyType { get; set; }

        public VarNameChangeNotification()
        {
            Name = new Person();
        }

        public VarNameChangeNotification (Person p, string type)
        {
            Name = p;
            NotifyType = type;
        }
    }
}
