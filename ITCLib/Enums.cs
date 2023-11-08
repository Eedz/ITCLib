using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ITCLib
{
    public enum AccessLevel { SMG = 1, PMG }
    public enum CommentDetails { Existing = 1, LastUsed, New }
    public enum VarNameFormat { NoCC, WithCC, NonStd }
    public enum QuestionType { Series, Standalone, Heading, InterviewerNote, Subheading }
    public enum Enumeration { Qnum = 1, AltQnum, Both }
    public enum ReadOutOptions { Neither, DontRead, DontReadOut }
    public enum RoutingType { Other, IfResponse, Otherwise, If }
    public enum RoutingStyle { Normal = 1, Grey, None }
    public enum NoteScope { Variable, RefVar, Survey, Wave, Deleted }
}
