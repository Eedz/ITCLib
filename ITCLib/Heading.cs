﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public class Heading : SurveyQuestion
    {
        public string StartQnum { get; set; }
        public string EndQnum { get; set; }
        public string FirstVarName { get; set; }
        public string LastVarName { get; set; }

        public Heading()
        {

        }

       
        
        public Heading (string qnum, Wording prep)
        {
            Qnum = qnum;
            PrePW = prep;
        }

        public override bool Equals(object obj)
        {
            var heading = obj as Heading;
            return heading != null &&
                   VarName.VarName == heading.VarName.VarName &&
                   PrePW == heading.PrePW;
        }

        public override int GetHashCode()
        {
            var hashCode = -1325402585;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(VarName.VarName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(PrePW.WordingText);
            return hashCode;
        }

        public override string ToString()
        {
            return VarName.VarName + " -- " + PrePW.WordingText;
        }
    }
}
