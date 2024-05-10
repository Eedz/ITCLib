using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Threading.Tasks;

namespace ITCLib
{
    public class PraccingIssue : ObservableObject 
    {
        public int ID 
        {
            get => _id;
            set => SetProperty(ref _id, value);             
        }
        public int IssueNo 
        {
            get => _issueno;
            set => SetProperty(ref _issueno, value);
        }
        public Survey Survey 
        {
            get => _survey;
            set => SetProperty(ref _survey, value);
        }
        public string VarNames
        {
            get => _varnames;
            set => SetProperty(ref _varnames, value);
        }
        public string Description
        {
            get => _description;
            set 
            {
                SetProperty(ref _description, value);
                DescriptionRTF = Utilities.GetRtfUnicodeEscapedString(Utilities.FormatText(_description));
            }
        }
        public string DescriptionRTF { get; private set; }

        public DateTime IssueDate
        {
            get => _issuedate;
            set => SetProperty(ref _issuedate, value);
        }
        
        public Person IssueFrom
        {
            get => _issueFrom;
            set => SetProperty(ref _issueFrom, value);
        }
        
        public Person IssueTo
        {
            get => _issueTo;
            set => SetProperty(ref _issueTo, value);
        }
        
        public bool Resolved
        {
            get => _resolved;
            set => SetProperty(ref _resolved, value);
        }
        
        public DateTime? ResolvedDate
        {
            get => _resolvedDate;
            set => SetProperty(ref _resolvedDate, value);
        }
        
        public Person ResolvedBy
        {
            get => _resolvedBy;
            set => SetProperty(ref _resolvedBy, value);
        }
        
        public DateTime LastUpdate
        {
            get => _lastUpdate;
            set => SetProperty(ref _lastUpdate, value);
        }
        
        public string Language
        {
            get => _language;
            set => SetProperty(ref _language, value);
        }
        
        public bool Fixed
        {
            get => _fixed;
            set => SetProperty(ref _fixed, value);
        }

        public PraccingCategory Category
        {
            get => _category;
            set => SetProperty(ref _category, value);
        }

        public Person EnteredBy { get => _enteredby; set => SetProperty(ref _enteredby, value); }
        public DateTime? EnteredOn { get => _enteredon; set => SetProperty(ref _enteredon, value); }

        public string PinNo { get => _pin; set => SetProperty(ref _pin, value); }

        public List<PraccingImage> Images { get; set; }
        public List<PraccingResponse> Responses { get; set; }

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

        private int _id;
        private int _issueno;
        private Survey _survey;
        private string _varnames;
        private DateTime _issuedate;
        private Person _issueFrom;
        private Person _issueTo;
        private bool _resolved;
        private DateTime? _resolvedDate;
        private Person _resolvedBy;
        private DateTime _lastUpdate;
        private string _language;
        private bool _fixed;
        private PraccingCategory _category;
        private string _description;
        private string _pin;
        private Person _enteredby;
        private DateTime? _enteredon;
     
    }

    public class PraccingResponse : ObservableObject
    {
        public int ID {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public int IssueID {
            get => _issueid;
            set => SetProperty(ref _issueid, value);
        }
        public DateTime? ResponseDate {
            get => _responsedate;
            set => SetProperty(ref _responsedate, value);
        }
        public string Response {
            get => _response;
            set
            {
                SetProperty(ref _response, value);
                _responseRTF =  Utilities.GetRtfUnicodeEscapedString(Utilities.FormatText(_response));
            }
        }
        public string ResponseRTF {
            get => _responseRTF;
            set
            {
                SetProperty(ref _responseRTF, value);
                _response = Utilities.FormatRTF(_responseRTF);
            }
        }

        public Person ResponseFrom {
            get => _responsefrom;
            set => SetProperty(ref _responsefrom, value);
        }
        public Person ResponseTo {
            get => _responseto;
            set => SetProperty(ref _responseto, value);
        }

        public string PinNo { get => _pin; 
            set => SetProperty(ref _pin, value); }

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

        private int _id;
        private int _issueid;
        private DateTime? _responsedate;
        private string _response;
        private string _responseRTF;
        private Person _responsefrom;
        private Person _responseto;
        private string _pin;
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

    public class PraccingImage : ITCImage
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
