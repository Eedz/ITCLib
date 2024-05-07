using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    public class Translation : ObservableObject
    {
        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public int QID { get => _qid; set => SetProperty(ref _qid, value); }
        public string Survey { get => _survey; set => SetProperty(ref _survey, value); }
        public string VarName { get => _varname; set => SetProperty(ref _varname, value); }
        public Language LanguageName { get => _languagename; set => SetProperty (ref _languagename, value); }
        public string Language { get => LanguageName.LanguageName; }
        public string TranslationText { get => _translationtext; set => SetProperty(ref _translationtext, value); }
        public string LitQ { get => _litq; set => SetProperty(ref _litq, value); }
        public bool Bilingual { get => _bilingual; set => SetProperty(ref _bilingual, value); }

        public Translation()
        {
            this.Survey = string.Empty;
            this.VarName = string.Empty;
            LanguageName = new Language();
            TranslationText = string.Empty;
            LitQ = string.Empty;
        }

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

        #region private backing variables
        private int _id;
        private int _qid;
        private string _survey;
        private string _varname;
        private Language _languagename;
        private string _translationtext;
        private string _litq;
        private bool _bilingual;
        #endregion
    }

    public class Language : ObservableObject
    {
        

        public int ID { get => _id; set => SetProperty (ref _id, value); }
        public string LanguageName { get => _languagename; set => SetProperty(ref _languagename, value); }
        public string Abbrev { get => _abbrev; set => SetProperty(ref _abbrev, value); }
        public string ISOAbbrev { get => _isoabbrev; set => SetProperty(ref _isoabbrev, value); }
        public string PreferredFont { get => _preferredfont; set => SetProperty(ref _preferredfont, value); }
        public bool NonLatin { get => _nonlatin; set => SetProperty(ref _nonlatin, value); }
        public bool RTL { get => _rtl; set => SetProperty (ref _rtl, value); }

        public Language()
        {
            ID = 0;
            LanguageName = string.Empty;
            Abbrev = string.Empty;
            ISOAbbrev = string.Empty;
            PreferredFont = string.Empty;
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

        #region private backing variables
        private int _id;
        private string _languagename;
        private string _abbrev;
        private string _isoabbrev;
        private string _preferredfont;
        private bool _nonlatin;
        private bool _rtl;
        #endregion
    }

    
}
