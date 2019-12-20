using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class VariablePrefix
    {
        public int ID { get; set; }
        public string Prefix { get; set; }
        public string PrefixName { get; set; }
        public string ProductType { get; set; }
        public string RelatedPrefixes { get; set; } // possible not needed
        public string Description { get; set; }
        public string Comments { get; set; }
        public bool Inactive { get; set; }

        public List<VariablePrefix> ParallelPrefixes { get; set; }
        public List<VariableRange> Ranges { get; set; }

    }

    public class VariableRange
    {
        public int ID { get; set; }
        public string Lower { get; set; }
        public string Upper { get; set; }
        public string Description { get; set; }
    }
}
