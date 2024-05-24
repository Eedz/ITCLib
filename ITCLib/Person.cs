using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    public class Person : ObservableObject
    {
        public int ID { get => _id; set => SetProperty(ref _id, value); }
        public string FirstName { get => _firstName; set => SetProperty(ref _firstName, value); }
        public string LastName { get => _lastName; set => SetProperty(ref _lastName, value); }
        public string Name { 
            get => 
                string.Join(" ", new string[] { FirstName, string.IsNullOrEmpty(LastName) ? string.Empty : LastName.Substring(0, 1) }); 
            set
            {
                int space = value.IndexOf(' ');
                if (space >= 0)
                {
                    FirstName = value.Substring(0, space);
                    LastName = value.Substring(space + 1);
                }
                else
                {
                    FirstName = value;
                }
            }
        }
        public string Email { get => _email; set => SetProperty(ref _email, value); }
        public string Username { get => _username; set => SetProperty(ref _username,value); }
        public string OfficeNo { get => _officeno; set => SetProperty(ref _officeno, value); }
        public string WorkPhone { get => _workphone; set => SetProperty(ref _workphone, value); }
        public string HomePhone { get => _homephone; set => SetProperty(ref _homephone, value); }
        public string Institution { get => _insitution; set => SetProperty(ref _insitution, value); }
        public bool Active { get => _active; set => SetProperty(ref _active, value); }
        public string PraccID { get => _praccid; set => SetProperty(ref _praccid, value); }
        public bool Entry { get => _entry; set => SetProperty(ref _entry, value); }
        public bool PraccEntry { get => _praccentry; set => SetProperty(ref _praccentry, value); }
        public bool VarNameChangeNotify { get => _varnamechangenotify; set => SetProperty(ref _varnamechangenotify, value); }

        public List<PersonnelRole> Roles { get; set; }
        public List<PersonnelStudy> AssociatedStudies { get; set; }
        public List<PersonnelComment> PersonnelComments { get; set; }        

        public Person()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            AssociatedStudies = new List<PersonnelStudy>();
            PersonnelComments = new List<PersonnelComment>();
            Roles = new List<PersonnelRole>();
        }

        public Person (int id) : this()
        {
            ID = id;
        }

        public Person(string first, string last) : this()
        {
            FirstName = first;
            LastName = last;
        }

        public Person(string name, int id) : this()
        {
            ID = id;
            FirstName = name;   
        }

        public override string ToString()
        {
            return Name;
        }

        public override bool Equals(object obj)
        {
            var person = obj as Person;
            return person != null &&
                   ID == person.ID;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            return hashCode;
        }

        #region private backing variables
        private int _id;
        private string _firstName;
        private string _lastName;
        private string _email;
        private string _username;
        private string _officeno;
        private string _workphone;
        private string _homephone;
        private string _insitution;
        private bool _active;
        private string _praccid;
        private bool _entry;
        private bool _praccentry;
        private bool _varnamechangenotify;
        #endregion
    }

    public class Role
    {
        public int ID { get; set; }
        public string RoleName { get; set; }
    }

    public class PersonnelRole
    {
        public int ID { get; set; }
        public int PersonnelID { get; set; }
        public Role RoleName { get; set; }

        public PersonnelRole ()
        {
            RoleName = new Role();
        }
    }

    public class PersonnelComment
    {
        public int ID { get; set; }
        public int PersonnelID { get; set; }
        public string CommentType { get; set; }
        public string Comment { get; set; }

        public PersonnelComment()
        {

        }

        public PersonnelComment (int id, string type, string comment)
        {
            ID = id;
            CommentType = type;
            Comment = comment;
        }
    }

    public class PersonnelStudy 
    {
        public int ID { get; set; }
        public int PersonnelID { get; set; }
        public Study StudyName { get; set; }

        public PersonnelStudy ()
        {
            StudyName = new Study();
        }
    }
}
