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
    
    public class SurveyCheck : ObservableObject
    {
        
        public int ID { get => _id; set => SetProperty(ref _id, value); }

        public SurveyCheckType CheckType { get => _checkType; set => SetProperty(ref _checkType, value); }

        public DateTime? CheckDate { get => _checkDate; set => SetProperty(ref _checkDate, value); }
        public Person Name { get => _name; set => SetProperty(ref _name, value); }

        public Survey SurveyCode { get => _survey; set => SetProperty(ref _survey, value); }

        public string Comments { get => _comments; set => SetProperty(ref _comments, value); }

        public BindingList<SurveyCheckRefSurvey> ReferenceSurveys { get; set; }

        public SurveyCheck()
        {
            CheckDate = DateTime.Today;
            CheckType = new SurveyCheckType(0, "");
            Name = new Person(0);

            SurveyCode = new Survey();
            ReferenceSurveys = new BindingList<SurveyCheckRefSurvey>();
          
            Comments = string.Empty;
        }

        private int _id;
        private SurveyCheckType _checkType;
        private DateTime? _checkDate;
        private Person _name;
        private Survey _survey;
        private string _comments;
    }

    public class SurveyCheckRefSurvey
    {
        public int ID { get; set; }
        public int CheckID { get; set; }
        public int SID { get; set; }        
        public DateTime? SurveyDate { get; set; }

        public SurveyCheckRefSurvey()
        {
            SurveyDate = null;
        }
    }


    public class SurveyCheckType
    {
        public int ID { get; set; }
        public string CheckName { get; set; }

        public SurveyCheckType()
        {
            ID = 0;
            CheckName = "";
        }

        public SurveyCheckType(int id, string name)
        {
            ID = id;
            CheckName = name;
        }

        public override bool Equals(object obj)
        {
            var type = obj as SurveyCheckType;
            return type != null &&
                   ID == type.ID &&
                   CheckName == type.CheckName;
        }

        public override int GetHashCode()
        {
            var hashCode = 744774948;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(CheckName);
            return hashCode;
        }
    }
    

}
