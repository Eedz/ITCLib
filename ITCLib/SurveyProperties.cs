using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class SurveyUserGroup : ObservableObject
    {
        private int _id;
        private string _usergroup;
        private string _code;
        private string _webname;

        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public string UserGroup { get => _usergroup; set => SetProperty(ref _usergroup, value); }
        public string Code { get => _code; set => SetProperty(ref _code, value); }
        public string WebName { get => _webname; set => SetProperty(ref _webname, value); }

        public SurveyUserGroup()
        {
            UserGroup = string.Empty;
            Code = string.Empty;
            WebName = string.Empty;
        }

        public SurveyUserGroup(int id, string group)
        {
            ID = id;
            UserGroup = group;
            Code = string.Empty;
            WebName = string.Empty;
        }

        public override string ToString()
        {
            return UserGroup;
        }

        public override bool Equals(object obj)
        {
            var group = obj as SurveyUserGroup;
            return group != null &&
                   ID == group.ID;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            return hashCode;
        }
    }

    public class SurveyCohort : ObservableObject
    {
        private int _id;
        private string _cohort;
        private string _code;
        private string _webname;

        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public string Cohort { get => _cohort; set => SetProperty(ref _cohort, value); }
        public string Code { get => _code; set => SetProperty(ref _code, value); }
        public string WebName { get => _webname; set => SetProperty(ref _webname, value); }

        public SurveyCohort()
        {
            Cohort = string.Empty;
            Code = string.Empty;
            WebName = string.Empty;
        }

        public SurveyCohort(int id, string cohort)
        {
            ID = id;
            Cohort = cohort;
        }

        public override string ToString()
        {
            return Cohort;
        }

        public override bool Equals(object obj)
        {
            var cohort = obj as SurveyCohort;
            return cohort != null &&
                   ID == cohort.ID;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            return hashCode;
        }
    }

    public class SurveyMode : ObservableObject
    {
        private int _id;
        private string _mode;
        private string _modeabbrev;

        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public string Mode { get => _mode; set => SetProperty(ref _mode, value); }
        public string ModeAbbrev { get => _modeabbrev; set => SetProperty(ref _modeabbrev, value); }

        public SurveyMode()
        {
            Mode = string.Empty;
            ModeAbbrev = string.Empty;
        }

        public SurveyMode(int id, string mode, string abbrev)
        {
            ID = id;
            Mode = mode;
            ModeAbbrev = abbrev;
        }

        public override string ToString()
        {
            return ModeAbbrev;
        }

        public override bool Equals(object obj)
        {
            var mode = obj as SurveyMode;
            return mode != null &&
                   ID == mode.ID;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            return hashCode;
        }
    }

    public class SurveyUserState : ObservableObject
    {
        private int _id;
        private int _survid;
        private UserState _state;

        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public int SurvID { get => _survid; set => SetProperty(ref _survid, value); }
        public UserState State { get => _state; set => SetProperty(ref _state, value); 
        }

        public SurveyUserState()
        {
            State = null;
        }

        public override string ToString()
        {
            return State.UserStateName;
        }

        public override bool Equals(object obj)
        {
            var state = obj as SurveyUserState;
            return state != null &&
                   SurvID == state.SurvID &&
                   State.Equals(state.State);

           
        }
        public override int GetHashCode()
        {
            var hashCode = 612815053;
            hashCode = hashCode * -1521134295 + SurvID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<UserState>.Default.GetHashCode(State);
            return hashCode;
        }
    }

    public class SurveyScreenedProduct : ObservableObject
    {
        private int _id;
        private int _survid;
        private ScreenedProduct _product;

        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public int SurvID { get => _survid; set => SetProperty(ref _survid, value); }
        public ScreenedProduct Product { get => _product; set => SetProperty(ref _product, value); }

        public SurveyScreenedProduct()
        {
            Product = new ScreenedProduct();
        }

        public override string ToString()
        {
            return Product.ProductName;
        }

        public override bool Equals(object obj)
        {
            var product = obj as SurveyScreenedProduct;
            return product != null &&
                   SurvID == product.SurvID &&
                   Product.Equals(product.Product);


        }
        public override int GetHashCode()
        {
            var hashCode = 612815053;
            hashCode = hashCode * -1521134295 + SurvID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<ScreenedProduct>.Default.GetHashCode(Product);
            return hashCode;
        }

    }

    public class SurveyLanguage : ObservableObject
    {
        private int _id;
        private int _survid;
        private Language _language;

        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public int SurvID { get => _survid; set => SetProperty(ref _survid, value); }
        public Language SurvLanguage { get => _language; set => SetProperty(ref _language, value); }


        public SurveyLanguage()
        {
            ID = 0;
            SurvID = 0;
            SurvLanguage = new Language();
        }

        public override string ToString()
        {
            return SurvLanguage.LanguageName;
        }

        public override bool Equals(object obj)
        {
            var language = obj as SurveyLanguage;
            return language != null &&
                   SurvID == language.SurvID &&
                   SurvLanguage.Equals(language.SurvLanguage);


        }
        public override int GetHashCode()
        {
            var hashCode = 612815053;
            hashCode = hashCode * -1521134295 + SurvID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Language>.Default.GetHashCode(SurvLanguage);
            return hashCode;
        }
    }
}
