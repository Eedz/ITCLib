using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class Translation
    {
        public int ID { get; set; }
        public int QID { get; set; }
        public string Survey { get; set; }
        public string VarName { get; set; }
        public string Language { get; set; }
        private string _translationtext;
        public string TranslationText { get { return _translationtext; } set { _translationtext = value; TranslationRTF = Utilities.FormatText(value); } }
        public string LitQ { get; set; }
        public string TranslationRTF { get; private set; }
        public bool Bilingual { get; set; }

        public Translation()
        {

        }
    }
}
