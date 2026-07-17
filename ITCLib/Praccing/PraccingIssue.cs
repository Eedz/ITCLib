using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

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
            set => SetProperty(ref _description, value);
        }

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
        
        public DateTime? LastUpdate
        {
            get => new[] { IssueDate, _lastUpdate, Responses.Max(x => x.ResponseDate) }.Max().Value;
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

        /// <summary>
        /// Returns a new object with basic details copied from this instance. DOES NOT COPY RESPONSES OR IMAGES.
        /// </summary>
        /// <returns></returns>
        public PraccingIssue Copy()
        {
            return new PraccingIssue()
            {
                ID = this.ID,
                Survey = new Survey() { SID = this.Survey.SID, SurveyCode = this.Survey.SurveyCode},
                IssueNo = this.IssueNo,
                VarNames = this.VarNames,
                IssueFrom = new Person(this.IssueFrom.Name, this.IssueFrom.ID),
                IssueTo = new Person(this.IssueTo.Name, this.IssueTo.ID),
                Category = new PraccingCategory(this.Category.ID, this.Category.Category),
                IssueDate = this.IssueDate,
                PinNo = this.PinNo,

                Resolved = this.Resolved,
                ResolvedDate = this.ResolvedDate,
                ResolvedBy = this.ResolvedBy,
                Description = this.Description,

                
            };
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
        private DateTime? _lastUpdate;
        private string _language;
        private bool _fixed;
        private PraccingCategory _category;
        private string _description;
        private string _pin;
        private Person _enteredby;
        private DateTime? _enteredon;
     
    }

    

    

}
