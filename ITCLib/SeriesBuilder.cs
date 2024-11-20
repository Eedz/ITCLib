using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    // add items to series
    // set question wordings
    // set all members wordings at once (not LitQ)
    // set all member labels at once (not content)
    // change cc to appropriate value
    

    public class SeriesBuilder
    {
        private readonly List<SurveyQuestion> _seriesMembers;
        public string StartingQnum { get; set; }
        public List<SurveyQuestion> SeriesMembers {  get { return _seriesMembers; } }

        public SeriesBuilder() 
        {
            _seriesMembers = new List<SurveyQuestion>();
        }
        public SeriesBuilder(List<SurveyQuestion> seriesMembers) 
        {
                _seriesMembers = seriesMembers;
        }

        public void AddMember(SurveyQuestion question)
        {
            if (!_seriesMembers.Contains(question))
            {
                question.Qnum = NextQnum();
                _seriesMembers.Add(question);
            }
        }

        public void RemoveMember(SurveyQuestion question)
        {
           
            _seriesMembers.Remove(question);
        }

        public void SetPreP(Wording wording)
        {
            foreach(var s in _seriesMembers)
            {
                s.PrePW = wording;
            }
        }

        public void SetPreI(Wording wording)
        {
            foreach (var s in _seriesMembers)
            {
                s.PreIW = wording;
            }
        }

        public void SetPreA(Wording wording)
        {
            foreach (var s in _seriesMembers)
            {
                s.PreAW = wording;
            }
        }

        public void SetPstI(Wording wording)
        {
            foreach (var s in _seriesMembers)
            {
                s.PstIW = wording;
            }
        }

        public void SetPstP(Wording wording)
        {
            foreach (var s in _seriesMembers)
            {
                s.PstPW = wording;
            }
        }

        public void SetRespOptions(ResponseSet set)
        {
            foreach (var s in _seriesMembers)
            {
                s.RespOptionsS = set;
            }
        }

        public void SetNRCodes(ResponseSet set)
        {
            foreach (var s in _seriesMembers)
            {
                s.NRCodesS = set;
            }
        }

        public void SetTopic(TopicLabel topic)
        {
            foreach (var s in _seriesMembers)
                s.VarName.Topic = topic;
        }

        public void SetDomain(DomainLabel domain)
        {
            foreach (var s in _seriesMembers)
                s.VarName.Domain = domain;
        }

        public void SetProduct(ProductLabel product)
        {
            foreach (var s in _seriesMembers)
                s.VarName.Product = product;
        }

        /// <summary>
        /// Returns a Qnum that would follow the last member of the series.
        /// </summary>
        /// <param name="qnum"></param>
        /// <returns></returns>
        private string NextQnum()
        {
            if (_seriesMembers.Count == 0)
                return StartingQnum ?? "000a";

            string qnum = _seriesMembers.Last().Qnum;
            char tail = 'a';
            if (qnum.Length > 3)
                tail = qnum[qnum.Length - 1];

            // increment the last character
            // if last is 'z' then replace 'z' with 'aa'
            if (tail == 'z')
                qnum = qnum.Substring(0, qnum.Length - 1) + "aa";
            else
            {
                tail++;
                qnum = qnum.Substring(0, qnum.Length - 1) + tail;
            }

            return qnum;
        }


        struct QuestionNumber
        {
            public int Value;
            public string Tail;
        }
    }
}
