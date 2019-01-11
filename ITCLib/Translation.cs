using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCSurveyReportLib
{
    public class Translation
    {
        public int ID { get; set; }
        public int QID { get; set; }
        public string Language { get; set; }
        public string TranslationText { get; set; }
        public bool Bilingual { get; set; }

        public Translation()
        {

        }



        // methods for splitting, return each field?
    }
}
