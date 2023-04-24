using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ITCLib
{
    public class Translation: INotifyPropertyChanged
    {
        public int ID { get; set; }
        public int QID { get; set; }
        public string Survey { get; set; }
        public string VarName { get; set; }
        public Language LanguageName { get; set; }
        public string Language
        {
            get { return LanguageName.LanguageName; }
        }
        private string _translationtext;
        public string TranslationText {
            get { return _translationtext; }
            set
            {
                _translationtext = value;
                TranslationRTF = Utilities.GetRtfUnicodeEscapedString(Utilities.FormatText(Utilities.FixElements(value)));
                NotifyPropertyChanged();
                
            }
        }

        public string LitQ { get; set; }
        public string TranslationRTF { get; private set; }
        public bool Bilingual { get; set; }

        public Translation()
        {
            LanguageName = new Language();
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #region Events
        public virtual event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public override bool Equals(object obj)
        {
            var t = obj as Translation;
            return t.ID == ID &&
                t.Survey.Equals(Survey) &&
                t.VarName.Equals(VarName) &&
                t.TranslationText.Equals(TranslationText);
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Survey);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(VarName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TranslationText);
            return hashCode;
        }
    }

    public class Language
    {
        public int ID { get; set; }
        public string LanguageName { get; set; }
        public string Abbrev { get; set; }
        public string ISOAbbrev { get; set; }
        public string PreferredFont { get; set; }
        public bool NonLatin { get; set; }
        public bool RTL { get; set; }
        public Language Self { get { return this; } }

        public Language()
        {
            ID = 0;
            LanguageName = "";
            Abbrev = "";
            ISOAbbrev = "";
            PreferredFont = "";
        }

        public override string ToString()
        {
            return LanguageName;
        }

        public override bool Equals(object obj)
        {
            var label = obj as Language;
            return label != null &&
                   ID == label.ID &&
                   LanguageName == label.LanguageName;
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(LanguageName);
            return hashCode;
        }
    }

    
}
