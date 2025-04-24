using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class PraccingResponse : ObservableObject
    {
        public int ID
        {
            get => _id;
            set => SetProperty(ref _id, value);
        }
        public int IssueID
        {
            get => _issueid;
            set => SetProperty(ref _issueid, value);
        }
        public DateTime? ResponseDate
        {
            get => _responsedate;
            set => SetProperty(ref _responsedate, value);
        }
        public string Response
        {
            get => _response;
            set => SetProperty(ref _response, value);
        }

        public Person ResponseFrom
        {
            get => _responsefrom;
            set => SetProperty(ref _responsefrom, value);
        }
        public Person ResponseTo
        {
            get => _responseto;
            set => SetProperty(ref _responseto, value);
        }

        public string PinNo
        {
            get => _pin;
            set => SetProperty(ref _pin, value);
        }

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
        private Person _responsefrom;
        private Person _responseto;
        private string _pin;
    }
}
