using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class DeletedQuestion
    {
        public int ID { get; set; }
        public string SurveyCode { get; set; }
        public string VarName { get; set; }
        public string VarLabel { get; set; }
        public string DomainLabel { get; set; }
        public string TopicLabel { get; set; }
        public string ContentLabel { get; set;  }
        public string ProductLabel { get; set; }
        public DateTime? DeleteDate { get; set; }
        public string DeletedBy { get; set; }

        public List<DeletedComment> DeleteNotes { get; set; }

        public DeletedQuestion()
        {
            DeleteDate = new DateTime();
            DeleteNotes = new List<DeletedComment>();
        }
    }
}
