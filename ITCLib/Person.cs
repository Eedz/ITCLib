using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class Person
    {
        public int ID { get; set; }
        public string Name { get; set; }

        public Person (int id)
        {
            ID = id;
        }

        public Person(string name)
        {
            Name = name;
        }

        public Person(string name, int id)
        {
            ID = id;
            Name = name;
        }

        public override bool Equals(object obj)
        {
            var person = obj as Person;
            return person != null &&
                   ID == person.ID &&
                   Name == person.Name;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Name);
            return hashCode;
        }
    }
}
