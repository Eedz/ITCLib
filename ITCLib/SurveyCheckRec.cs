using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace ITCLib
{
    
    public class SurveyCheckRec : INotifyPropertyChanged
    {
        
        public int ID {
            get { return _id; }
            set
            {
                _id = value;
                NotifyPropertyChanged();
            }
        }

        public SurveyCheckType CheckType
        {
            get { return _checkType; }
            set
            {
                if (value != _checkType)
                {
                    _checkType = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public DateTime? CheckDate
        {
            get { return _checkDate; }
            set
            {
                _checkDate = value;
                NotifyPropertyChanged();
            }
        }
        public Person Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged();
            }
        }
        
        public Survey SurveyCode
        {
            get { return _survey; }
            set
            {
                _survey = value;
                NotifyPropertyChanged();
            }
        }
        
        public string Comments
        {
            get { return _comments; }
            set
            {
                _comments = value;
                NotifyPropertyChanged();
            }
        }

        public BindingList<SurveyCheckRefSurvey> ReferenceSurveys { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public SurveyCheckRec()
        {
            CheckDate = DateTime.Today;
            CheckType = new SurveyCheckType(0, "");
            Name = new Person(0);

            SurveyCode = new Survey();
            ReferenceSurveys = new BindingList<SurveyCheckRefSurvey>();
          
            Comments = "";

        }

        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
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
