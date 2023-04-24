using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ITCLib
{
    public class PraccingIssue : INotifyPropertyChanged
    {
        private int _id;
        public int ID { get { return _id; } set { _id = value; NotifyPropertyChanged(); } }
        private int _issueno;
        public int IssueNo { get { return _issueno; } set { _issueno = value; NotifyPropertyChanged(); } }
        private Survey _survey;
        public Survey Survey { get { return _survey; } set { _survey = value; NotifyPropertyChanged(); } }
        private string _varnames;
        public string VarNames
        {
            get
                { return _varnames; }
            set
                {
                    
                    _varnames = value.Trim();
                    NotifyPropertyChanged();
                }
        }
        public string Description
        {
            get { return _description; }
            set
            {
                if (value != _description)
                {
                    _description = value;
                    NotifyPropertyChanged();
                    DescriptionRTF = Utilities.GetRtfUnicodeEscapedString(Utilities.FormatText(_description));
                }
            }
        }

        public string DescriptionRTF { get; private set; }

        private DateTime _issuedate;
        public DateTime IssueDate { get { return _issuedate; } set { _issuedate = value; NotifyPropertyChanged(); } }
        private Person _issueFrom;
        public Person IssueFrom { get { return _issueFrom; } set { _issueFrom = value; NotifyPropertyChanged(); } }
        private Person _issueTo;
        public Person IssueTo { get { return _issueTo; } set { _issueTo = value; NotifyPropertyChanged(); } }
        private bool _resolved;
        public bool Resolved { get { return _resolved; } set { _resolved = value; NotifyPropertyChanged(); } }
        private DateTime? _resolvedDate;
        public DateTime? ResolvedDate { get { return _resolvedDate; } set { _resolvedDate = value; NotifyPropertyChanged(); } }
        private Person _resolvedBy;
        public Person ResolvedBy { get { return _resolvedBy; } set { _resolvedBy = value; NotifyPropertyChanged(); } }
        private DateTime _lastUpdate;
        public DateTime LastUpdate { get { return _lastUpdate; } set { _lastUpdate = value; NotifyPropertyChanged(); } }
        private string _language;
        public string Language { get { return _language; } set { _language = value; NotifyPropertyChanged(); } }
        private bool _fixed;
        public bool Fixed { get { return _fixed; } set { _fixed = value; NotifyPropertyChanged(); } }
        private PraccingCategory _category;
        public PraccingCategory Category { get { return _category; } set { _category = value; NotifyPropertyChanged(); } }

        public Person EnteredBy { get; set; }
        public DateTime? EnteredOn { get; set; }

        public List<PraccingImage> Images { get; set; }
        public List<PraccingResponse> Responses { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public PraccingIssue()
        {
            Images = new List<PraccingImage>();
            Responses = new List<PraccingResponse>();
            Category = new PraccingCategory();
            LastUpdate = new DateTime();
            ResolvedBy = new Person();
            ResolvedDate = null;
            IssueTo = new Person();
            IssueFrom = new Person();
            IssueDate = new DateTime();
            Survey = new Survey();
            EnteredBy = new Person();
            EnteredOn = null;
            Description = string.Empty;
            Language = "English";
            VarNames = string.Empty;
        }

        // This method is called by the Set accessor of each property.
        // The CallerMemberName attribute that is applied to the optional propertyName
        // parameter causes the property name of the caller to be substituted as an argument.
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                //PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private string _description;
     
    }

    public class PraccingResponse
    {
        public int ID { get; set; }
        public int IssueID { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string Response {
            get { return _response; }
            set
            {
                if (value != _response)
                {
                    _response = value;
                    _responseRTF =  Utilities.GetRtfUnicodeEscapedString(Utilities.FormatText(_response));


                }
            }
        }
        public string ResponseRTF {
            get { return _responseRTF; }
            set
            {
                _responseRTF = value;
                _response = Utilities.FormatRTF(_responseRTF);
            }
        }

        public Person ResponseFrom { get; set; }
        public Person ResponseTo { get; set; }
        public List<PraccingImage> Images { get; set; }

        public PraccingResponse()
        {
            Images = new List<PraccingImage>();
            ResponseDate = DateTime.Now;
            ResponseFrom = new Person();
            ResponseTo = new Person();
            Response = string.Empty;
        }

        public override bool Equals(object obj)
        {
            var response = obj as PraccingResponse;
            return response != null &&
                   ID == response.ID &&
                   Response == response.Response;
                   
        }

        public override int GetHashCode()
        {
            var hashCode = -244446586;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Response);
            return hashCode;
        }

        private string _response;
        private string _responseRTF;
    }


    public class PraccingCategory
    {
        public int ID { get; set; }
        public string Category { get; set; }

        public PraccingCategory()
        {
            Category = string.Empty;
        }

        public PraccingCategory(int id, string category)
        {
            ID = id;
            Category = category;
        }

        public override bool Equals(object obj)
        {
            var category = obj as PraccingCategory;
            return category != null &&
                   ID == category.ID;
        }

        public override int GetHashCode()
        {
            var hashCode = 1479869798;
            hashCode = hashCode * -1521134295 + ID.GetHashCode();
            return hashCode;
        }

        public override string ToString()
        {
            return Category;
        }
    }

    public class PraccingImage
    {
        public int ID { get; set; }
        public int PraccID { get; set; }
        public string Path { get; set; }

        public PraccingImage()
        {
            ID = 0;
            Path = string.Empty;
        }

        public PraccingImage(int id, string path)
        {
            ID = id;
            Path = path;
        }
    }

}
