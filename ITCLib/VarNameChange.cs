using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace ITCLib
{
    public class VarNameChange : INotifyPropertyChanged
    {
        public int ID { get; set; }
        public string OldName
        {
            get { return _oldname; }
            set
            {
                if (_oldname != value)
                {
                    _oldname = value;
                    OldRefName = Utilities.ChangeCC(_oldname);
                    NotifyPropertyChanged();
                }
            }
        }
        public string OldRefName { get; private set; }
        public string NewName { get { return _newname; }
            set
            {
                if (_newname != value)
                {
                    _newname = value;
                    NewRefName = Utilities.ChangeCC(_newname);
                    NotifyPropertyChanged();
                }
            }
        }
        public string NewRefName { get; private set; }

        public DateTime ChangeDate {
            get { return _changedate; }
            set
            {
                if (_changedate != value)
                {
                    _changedate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public DateTime? ApproxChangeDate {
            get { return _approxchangedate; }
            set
            {
                if (_approxchangedate != value)
                {
                    _approxchangedate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public Person ChangedBy {
            get { return _changedby; }
            set
            {
                if (_changedby ==null || !_changedby.Equals(value))
                {
                    _changedby = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Authorization {
            get { return _authorization; }
            set
            {
                if (_authorization != value)
                {
                    _authorization = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Rationale {
            get { return _rationale; }
            set
            {
                if (_rationale != value)
                {
                    _rationale = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public string Source {
            get { return _source; }
            set
            {
                if (_source != value)
                {
                    _source = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool HiddenChange {
            get { return _hiddenchange; }
            set
            {
                if (_hiddenchange != value)
                {
                    _hiddenchange = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public bool PreFWChange {
            get { return _prefwchange; }
            set
            {
                if (_prefwchange != value)
                {
                    _prefwchange = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public List<VarNameChangeNotification> Notifications {get;set;}
        public List<VarNameChangeSurvey> SurveysAffected { get; set; }
        public string SurveyList { get
            {
                return GetSurveys();
            }
        }

        #region Events
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion

        public VarNameChange()
        {
            ChangeDate = DateTime.Today;
            ChangedBy = new Person();
            SurveysAffected = new List<VarNameChangeSurvey>();
            Notifications = new List<VarNameChangeNotification>();
            Rationale = string.Empty;
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

        public string GetSurveys()
        {
            return string.Join(", ", SurveysAffected.Select(x => x.SurveyCode.SurveyCode));
        }

        #region Backing variables
        private string _oldname;
        private string _newname;
        private DateTime _changedate;
        private DateTime? _approxchangedate;
        private Person _changedby;
        private string _authorization;
        private string _rationale;
        private string _source;
        private bool _hiddenchange;
        private bool _prefwchange;
        #endregion
    }

    public class VarNameChangeSurvey
    {
        public int ID { get; set; }
        public int ChangeID { get; set; }
        public Survey SurveyCode { get; set; }
    }
}
