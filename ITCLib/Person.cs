using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
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
    }
}
