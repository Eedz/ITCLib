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
    public enum HScheme { Sequential = 1, AcrossCountry = 2 }
    public enum HStyle { Classic = 1, TrackedChanges = 2 }
    public enum PaperSize { Letter, Legal, Eleven17, A4 }
    public enum ReportTemplate { Standard, StandardTranslation, Website, WebsiteTranslation, Automatic, Custom }
    public enum Enumeration { Qnum = 1, AltQnum, Both }
    public enum ReadOutOptions { Neither, DontRead, DontReadOut }
    public enum RoutingType { Other, IfResponse, Otherwise, If }
    public enum FileFormats { DOC = 1, PDF }
    public enum TableOfContents { None, Qnums, PageNums }
    public enum PaperSizes { Letter = 1, Legal, Eleven17, A4 }
    public enum ReportTypes { Standard = 1, Label, Order }
    public enum ReportPreset { SurveyList = 1, TopicContent, OrderCompare, Overview, Sections, Syntax, Harmony, VarList, ProductCrosstab }
    public enum RoutingStyle { Normal =1, Grey, None}
    public enum NoteScope { Variable, RefVar, Survey, Wave, Deleted }
}
