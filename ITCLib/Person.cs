using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class Person
    {
        public int ID { get; set; }
        // private string _firstName;
        //private string _lastName;

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string OfficeNo { get; set; }
        public string WorkPhone { get; set; }
        public string HomePhone { get; set; }
        public string Institution { get; set; }

        public bool Active { get; set; }
        public bool SMG { get; set; }
        public bool Analyst { get; set; }
        public bool Praccer { get; set; }
        public string PraccID { get; set; }
        public bool Programmer { get; set; }
        public bool Firm { get; set; }
        public bool CountryTeam { get; set; }
        public bool Admin { get; set; }
        public bool ResearchAssistant { get; set; }
        public bool Dissemination { get; set; }
        public bool Investigator { get; set; }
        public bool ProjectManager { get; set; }
        public bool Statistician { get; set; }
        public bool Entry { get; set; }
        public bool PraccEntry { get; set; }

        public List<Study> AssociatedStudies { get; set; }
        public List<PersonnelComment> PersonnelComments { get; set; }

        public bool VarNameChangeNotify { get; set; }

        public Person()
        {
            FirstName = string.Empty;
            LastName = string.Empty;
            Name = string.Empty;
            AssociatedStudies = new List<Study>();
            PersonnelComments = new List<PersonnelComment>();
        }

        public Person (int id) :base()
        {
            ID = id;
            Name = string.Empty;
        }

        public Person(string first, string last) : base()
        {
            FirstName = first;
            LastName = last;
            Name = first + " " + last.Substring(0, 1);
        }

        public Person(string name, int id) : base()
        {
            ID = id;
            Name = name;
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
    }

   

    public class PersonnelComment
    {
        public int ID { get; set; }

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

    //public class PersonnelStudy
    //{
    //    public int ID { get; set; }
    //    public Study AssociatedStudy { get; set; }

    //    public PersonnelStudy()
    //    {
    //        AssociatedStudy = new Study();
    //    }

    //    public PersonnelStudy(int id, Study study)
    //    {
    //        ID = id;
    //        AssociatedStudy = study;
    //    }

    //    public override string ToString()
    //    {
    //        return AssociatedStudy.StudyName;
    //    }

    //    public override bool Equals(object obj)
    //    {
    //        var study = obj as Study;
    //        return study != null &&
    //               ID == study.ID;
    //    }

    //    public override int GetHashCode()
    //    {
    //        var hashCode = 1479869798;
    //        hashCode = hashCode * -1521134295 + ID.GetHashCode();
    //        return hashCode;
    //    }


    //}
}
