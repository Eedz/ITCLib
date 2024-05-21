using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    /// <summary>
    /// This class holds lists of words that are to be considered equal in terms of comparison reports.
    /// </summary>
    public class SimilarWords
    {
        public int ID { get; set; }
        public List<string> Words { get; set; }
        public string WordList { get => string.Join(", ", Words); }

        public SimilarWords ()
        {
            Words = new List<string>();
        }

        
    }
}
