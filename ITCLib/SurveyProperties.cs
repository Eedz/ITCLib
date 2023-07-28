using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class SurveyUserGroup
    {
        public int ID { get; set; }
        public string UserGroup { get; set; }
        public string Code { get; set; }
        public string WebName { get; set; }

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

    public class SurveyCohort
    {
        public int ID { get; set; }
        public string Cohort { get; set; }
        public string Code { get; set; }
        public string WebName { get; set; }

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

    public class SurveyMode
    {
        public int ID { get; set; }
        public string Mode { get; set; }
        public string ModeAbbrev { get; set; }

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

    public class SurveyUserState
    {
        public int ID { get; set; }
        public int SurvID { get; set; }
        public UserState State { get; set; }

        public SurveyUserState()
        {
            State = null;
        }

        public override string ToString()
        {
            return State.UserStateName;
        }
    }

    public class SurveyScreenedProduct
    {
        public int ID { get; set; }
        public int SurvID { get; set; }
        public ScreenedProduct Product { get; set; }

        public SurveyScreenedProduct()
        {
            Product = new ScreenedProduct();
        }

        public override string ToString()
        {
            return Product.ProductName;
        }

    }

    public class SurveyLanguage
    {
        public int ID { get; set; }
        public int SurvID { get; set; }
        public Language SurvLanguage { get; set; }
        

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
    }
}
