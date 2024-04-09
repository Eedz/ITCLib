﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System.Text.RegularExpressions;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ITCLib
{
    // TODO: use Wording objects
    // TODO: remove formatting methods but keep GetQuestionText variants
    // TODO: create ReportQuestion derived object that has SortQnum property and remove check for ^ in GetQnum

    public class SurveyQuestion : ObservableObject
    {
        #region Properties

        public int ID; // question ID
        public string SurveyCode { get; set; }

        public VariableName VarName { get; set; }
        
        public string Qnum
        {
            get => _qnum;
            set => SetProperty(ref _qnum, value);
        }
        public string AltQnum
        {
            get => _altqnum;
            set => SetProperty(ref _altqnum, value);
        }   
        public string AltQnum2 
        {
            get => _altqnum2;
            set => SetProperty(ref _altqnum2, value);
        }
        public string AltQnum3 
        {
            get =>  _altqnum3;
            set => SetProperty(ref _altqnum3, value);
        }

        // wordings
        
        public Wording PrePW { get => _prepw; 
            set => SetProperty(ref _prepw, value); }
        public Wording PreIW { get => _preiw; 
            set => SetProperty(ref _preiw, value); }        
        public Wording PreAW { get => _preaw; 
            set => SetProperty(ref _preaw, value); }
        public Wording LitQW { get => _litqw; 
            set => SetProperty(ref _litqw, value); }        
        public Wording PstIW { get => _pstiw; 
            set => SetProperty(ref _pstiw, value); }
        public Wording PstPW { get => _pstpw; 
            set => SetProperty(ref _pstpw, value); }        
        public ResponseSet RespOptionsS { get => _respoptionss; 
            set => SetProperty(ref _respoptionss, value); }
        public ResponseSet NRCodesS { get => _nrcodess; 
            set => SetProperty(ref _nrcodess, value); }

        // field info

        public bool ProgrammerOnly { get; set; }
        public bool Internal { get; set; } // dervied variables, programmer notes, routing screens, headings?
        public bool TableFormat { get; set; }
        public bool ScriptOnly { get; set; }
        public string NumFmt { get; set; }

        private bool _correctedflag;
        public bool CorrectedFlag {
            get => _correctedflag;
            set => SetProperty(ref _correctedflag, value);
        }

        public List<Translation> Translations { get; set; }
        public List<QuestionComment> Comments { get; set; }

        private string _filters;
        public string Filters {
            get => _filters;
            set => SetProperty(ref _filters, value);
        }

        public List<VariableName> PreviousNameList { get; set; }

        public List<QuestionTimeFrame> TimeFrames { get; set; }

        public string FilterDescription
        {
            get { return _plainfilter; }
            set
            {
                SetProperty(ref _plainfilter, value);
                FilterDescriptionRTF = Utilities.GetRtfUnicodeEscapedString(FormatText(value));
            }
        }

        public string FilterDescriptionRTF { get; private set; }

        public List<SurveyImage> Images { get; set; }

        public string QuestionText
        {
            get { return GetQuestionText(); }
        }
        #endregion

        

        #region Constructors
        public SurveyQuestion()
        {
            VarName = new VariableName();
            Qnum = string.Empty;

            InitializeWordings();

            //PreP = string.Empty;
            //PreI = string.Empty;
            //PreA = string.Empty;
            //LitQ = string.Empty;
            //PstI = string.Empty;
            //PstP = string.Empty;
            //RespName = "0";
            //RespOptions = string.Empty;
            //NRName = "0";
            //NRCodes = string.Empty;
            FilterDescription = string.Empty;
            Filters = string.Empty;

            Translations = new List<Translation>();
            Comments = new List<QuestionComment>();

            PreviousNameList = new List<VariableName>();
            
            TimeFrames = new List<QuestionTimeFrame>();
            Images = new List<SurveyImage>();
        }

        public SurveyQuestion(string var) : this()
        {
            VarName = new VariableName(var);
        }

        public SurveyQuestion(string var, string qnum) : this()
        {
            VarName = new VariableName(var);
            Qnum = qnum;
        }

        public SurveyQuestion(string surveyCode, string var, ProductLabel product) : this()
        {
            SurveyCode = surveyCode;
            VarName = new VariableName(var);
            VarName.Product = product;
        }

        private void InitializeWordings()
        {
            PrePW = new Wording();
            PreIW = new Wording();
            PreAW = new Wording();
            LitQW = new Wording();
            PstIW = new Wording();
            PstPW = new Wording();
            RespOptionsS = new ResponseSet();
            NRCodesS = new ResponseSet();
        }

        private void InitializeLists()
        {
            Translations = new List<Translation>();
            Comments = new List<QuestionComment>();
            PreviousNameList = new List<VariableName>();
            TimeFrames = new List<QuestionTimeFrame>();
            Images = new List<SurveyImage>();
        }
        #endregion

        public SurveyQuestion DeepCopyWordings()
        {
            SurveyQuestion copy = new SurveyQuestion();
            copy.Qnum = string.Copy(Qnum);
            copy.VarName = new VariableName(VarName.VarName);
            //copy.PrePNum = PrePNum; copy.PreINum = PreINum; 
            //copy.PreANum = PreANum; copy.LitQNum = LitQNum;
            //copy.RespName = string.Copy(RespName);
            //copy.NRName = string.Copy(NRName);

            copy.PrePW = new Wording(PrePW.WordID, WordingType.PreP, PrePW.WordingText);
            copy.PreIW = new Wording(PreIW.WordID, WordingType.PreI, PreIW.WordingText);
            copy.PreAW = new Wording(PreAW.WordID, WordingType.PreA, PreAW.WordingText);
            copy.LitQW = new Wording(LitQW.WordID, WordingType.LitQ, LitQW.WordingText);
            copy.PstIW = new Wording(PstIW.WordID, WordingType.PstI, PstIW.WordingText);
            copy.PstPW = new Wording(PstPW.WordID, WordingType.PstP, PstPW.WordingText);
            copy.RespOptionsS = new ResponseSet(RespOptionsS.RespSetName, ResponseType.RespOptions, RespOptionsS.RespList);
            copy.NRCodesS = new ResponseSet(NRCodesS.RespSetName, ResponseType.NRCodes, NRCodesS.RespList);
            //copy.PreP = string.Copy(PreP);
            //copy.PreI = string.Copy(PreI);
            //copy.PreA = string.Copy(PreA);
            //copy.LitQ = string.Copy(LitQ);
            //copy.PstI = string.Copy(PstI);
            //copy.PstP = string.Copy(PstP);
            //copy.RespOptions = string.Copy(RespOptions);
            //copy.NRCodes = string.Copy(NRCodes);
            copy.Filters = string.Copy(Filters);
            copy.FilterDescription = string.Copy(FilterDescription);
            
            foreach (Translation t in Translations)
            {
                copy.Translations.Add(new Translation
                {
                    ID = t.ID,
                    QID = t.QID,
                    LitQ = string.Copy(t.LitQ ?? string.Empty),
                    LanguageName = t.LanguageName,
                    TranslationText = string.Copy(t.TranslationText ?? string.Empty),
                    Bilingual = t.Bilingual
                });
            }

            copy.Images = Images;
            return copy;
        }

        public SurveyQuestion Copy()
        {
            SurveyQuestion sq;

            sq = new SurveyQuestion
            {

                VarName = VarName,

                Qnum = Qnum,
                AltQnum = AltQnum,
                AltQnum2 = AltQnum2,
                AltQnum3 = AltQnum3,
                PreviousNameList = PreviousNameList,

                PrePW = PrePW,
                PreIW = PreIW,
                PreAW = PreAW,
                LitQW = LitQW,
                PstIW = PstIW,
                PstPW = PstPW,
                RespOptionsS = RespOptionsS,
                NRCodesS = NRCodesS,
               // PrePNum = PrePNum,
                //PreP = PreP,
                //PreINum = PreINum,
                //PreI = PreI,
                //PreANum = PreANum,
                //PreA = PreA,
                //LitQNum = LitQNum,
                //LitQ = LitQ,
                //PstINum = PstINum,
                //PstI = PstI,
                //PstPNum = PstPNum,
                //PstP = PstP,
                //RespName = RespName,
                //RespOptions = RespOptions,
                //NRName = NRName,
                //NRCodes = NRCodes,

                CorrectedFlag = CorrectedFlag,

                TimeFrames = TimeFrames,
                FilterDescription = FilterDescription,
                Filters = Filters,
                Images = Images
                
            };

            return sq;
        }

        private string FixElements(string input)
        {
            if (string.IsNullOrEmpty(input))
                return "";

            return input.Replace("&gt;", ">").Replace("&lt;", "<").Replace("&nbsp;", " ");
        }

        private string FormatText(string wordingText)
        {
            string wording = FixElements(wordingText);

            wording = wording.Replace("<strong>", @"\b ");
            wording = wording.Replace("</strong>", @"\b0 ");
            wording = wording.Replace("<em>", @"\i ");
            wording = wording.Replace("</em>", @"\i0 ");
            wording = wording.Replace("<br>", @"\line ");
            wording = wording.Replace("\r\n", @"\line ");
            wording = wording.Replace("<u>", @"\ul ");
            wording = wording.Replace("</u>", @"\ul0 ");
            wording = wording.Replace("[bullet]", @"\bullet ");
            wording = @"{\rtf1\ansi " + wording + "}";

            return wording;
        }

        public string GetFullVarLabel()
        {
            if (TimeFrames.Count == 0)
                return VarName.VarLabel;

            // concatenate the list of time frames
            string timeframeList = string.Join(", ", TimeFrames.Select(x => x.TimeFrame));

            // insert in between {} if present
            if (VarName.VarLabel.Contains("{") && VarName.VarLabel.Contains("}"))
            {
                return VarName.VarLabel.Replace("{}", "{" +  timeframeList + "}");
            }
            // append if {} is not present
            return VarName.VarLabel + " - {" + timeframeList + "}";

        }

        public string GetQuestionText(List<string> stdFieldsChosen, bool colorLitQ = false, string newline = "\r\n")
        {
            
            string questionText = "";

            if (stdFieldsChosen.Contains("PreP") && !string.IsNullOrEmpty(PrePW.WordingText)) { questionText += "<strong>" + PrePW.WordingText + "</strong>" + newline; }
            if (stdFieldsChosen.Contains("PreI") && !string.IsNullOrEmpty(PreIW.WordingText)) { questionText += "<em>" + PreIW.WordingText + "</em>" + newline; }
            if (stdFieldsChosen.Contains("PreA") && !string.IsNullOrEmpty(PreAW.WordingText)) { questionText += PreAW.WordingText + newline; }

            if (stdFieldsChosen.Contains("LitQ") && !string.IsNullOrEmpty(LitQW.WordingText)) {
                if (colorLitQ)
                    questionText += "[indent][lblue]" + LitQW.WordingText + "[/lblue][/indent]" + newline;
                else
                    questionText += "[indent]" + LitQW.WordingText + "[/indent]" + newline;
            }

            if (stdFieldsChosen.Contains("RespOptions") && !string.IsNullOrEmpty(RespOptionsS.RespList)) { questionText += "[indent3]" + RespOptionsS.RespList + "[/indent3]" + newline; }
            if (stdFieldsChosen.Contains("NRCodes") && !string.IsNullOrEmpty(NRCodesS.RespList)) { questionText += "[indent3]" + NRCodesS.RespList + "[/indent3]" + newline; }
            if (stdFieldsChosen.Contains("PstI") && !string.IsNullOrEmpty(PstIW.WordingText)) { questionText += "<em>" + PstIW.WordingText + "</em>" + newline; }
            if (stdFieldsChosen.Contains("PstP") && !string.IsNullOrEmpty(PstPW.WordingText)) { questionText += "<strong>" + PstPW.WordingText + "</strong>"; }

            
            questionText += string.Join("\r\n", Images.Select(x=>x.ImagePath));
            

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);
            

            return questionText;
        }

        public string GetQuestionTextHTML(List<string> stdFieldsChosen, bool colorLitQ = false)
        {
            StringBuilder questionText = new StringBuilder();
            
            if (stdFieldsChosen.Contains("PreP") && !string.IsNullOrEmpty(PrePW.WordingText)) { questionText.Append("<p><strong>" + PrePW.WordingText + "</strong></p>"); }
            if (stdFieldsChosen.Contains("PreI") && !string.IsNullOrEmpty(PreIW.WordingText)) { questionText.Append("<p><em>" + PreIW.WordingText + "</em></p>"); }
            if (stdFieldsChosen.Contains("PreA") && !string.IsNullOrEmpty(PreAW.WordingText)) { questionText.Append("<p>" + PreAW.WordingText + "</p>"); }

            if (stdFieldsChosen.Contains("LitQ") && !string.IsNullOrEmpty(LitQW.WordingText))
            {
                if (colorLitQ)
                    questionText.Append("<indent><font color=\"blue\">" + LitQW.WordingText + "</font></indent>");
                else
                    questionText.Append("<p style=\"margin-left: 16px\">" + LitQW.WordingText + "</p>");
            }

            if (stdFieldsChosen.Contains("RespOptions") && !string.IsNullOrEmpty(RespOptionsS.RespList))
            {
                string[] lines = RespOptionsS.RespList.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                questionText.Append("<p style=\"margin-left: 48px\">" + string.Join("</p><p style=\"margin-left: 48px\">", lines) + "</p>");
            }
            if (stdFieldsChosen.Contains("NRCodes") && !string.IsNullOrEmpty(NRCodesS.RespList))
            {
                string[] lines = NRCodesS.RespList.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                questionText.Append("<p style=\"margin-left: 48px\">" + string.Join("</p><p style=\"margin-left: 48px\">", lines) + "</indent3></p>");
            }
            if (stdFieldsChosen.Contains("PstI") && !string.IsNullOrEmpty(PstIW.WordingText)) { questionText.Append("<p><em>" + PstIW.WordingText + "</em></p>"); }
            if (stdFieldsChosen.Contains("PstP") && !string.IsNullOrEmpty(PstPW.WordingText)) { questionText.Append("<p><strong>" + PstPW.WordingText + "</strong></p>"); }

            if (Images.Count > 0)
                questionText.Append("<p>Image filename: " + string.Join("</p><p>Image filename: ", Images.Select(x => x.ImageName)) + "</p>");

            return questionText.ToString();
        }

        public string GetQuestionTextHTML(bool colorLitQ = false)
        {
            StringBuilder questionText = new StringBuilder();

            if (!string.IsNullOrEmpty(PrePW.WordingText)) { questionText.Append("<p><strong>" + PrePW.WordingText + "</strong></p>"); }
            //if (!string.IsNullOrEmpty(PreI)) { questionText.Append("<p><em>" + PreI + "</em></p>"); }
            if (!string.IsNullOrEmpty(PreIW.WordingText)) { questionText.Append("<p><em>" + PreIW.WordingText + "</em></p>"); }
            if (!string.IsNullOrEmpty(PreAW.WordingText)) { questionText.Append("<p>" + PreAW.WordingText + "</p>"); }

            if (!string.IsNullOrEmpty(LitQW.WordingText))
            {
                if (colorLitQ)
                    questionText.Append("<p style=\\\"margin-left: 16px\\\">\"<font color=\"blue\">" + LitQW.WordingText + "</font></p>");
                else
                    questionText.Append("<p style=\"margin-left: 16px\">" + LitQW.WordingText + "</p>");
            }

            if (!string.IsNullOrEmpty(RespOptionsS.RespList))
            {
                string[] lines = RespOptionsS.RespList.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                questionText.Append("<p style=\"margin-left: 48px\">" + string.Join("</p><p style=\"margin-left: 48px\">", lines) + "</p>");
            }
            if (!string.IsNullOrEmpty(NRCodesS.RespList))
            {
                string[] lines = NRCodesS.RespList.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                questionText.Append("<p style=\"margin-left: 48px\">" + string.Join("</p><p style=\"margin-left: 48px\">", lines) + "</p>");
            }
            if (!string.IsNullOrEmpty(PstIW.WordingText)) { questionText.Append("<p><em>" + PstIW.WordingText + "</em></p>"); }
            if (!string.IsNullOrEmpty(PstPW.WordingText)) { questionText.Append("<p><strong>" + PstPW.WordingText + "</strong></p>"); }

            if (Images.Count > 0)
                questionText.Append("<p>Image filename: " + string.Join("</p><p>Image filename: ", Images.Select(x => x.ImageName)) + "</p>");

            return questionText.ToString();
        }

        public string GetQuestionMainTextHTML()
        {
            StringBuilder questionText = new StringBuilder();


            //if (!string.IsNullOrEmpty(PreI)) { questionText.Append("<p><em>" + PreI + "</em></p>"); }
            if (!string.IsNullOrEmpty(PreIW.WordingText)) { questionText.Append("<p><em>" + PreIW.WordingText + "</em></p>"); }
            if (!string.IsNullOrEmpty(PreAW.WordingText)) { questionText.Append("<p>" + PreAW.WordingText + "</p>"); }

            if (!string.IsNullOrEmpty(LitQW.WordingText)) questionText.Append("<p style=\"margin-left: 16px\">" + LitQW.WordingText + "</p>");
            
            if (!string.IsNullOrEmpty(RespOptionsS.RespList))
            {
                string[] lines = RespOptionsS.RespList.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);

                questionText.Append("<p style=\"margin-left: 48px\">" + string.Join("</p><p style=\"margin-left: 48px\">", lines) + "</p>");
            }
            if (!string.IsNullOrEmpty(NRCodesS.RespList))
            {
                string[] lines = NRCodesS.RespList.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                questionText.Append("<p style=\"margin-left: 48px\">" + string.Join("</p><p style=\"margin-left: 48px\">", lines) + "</p>");
            }
            if (!string.IsNullOrEmpty(PstIW.WordingText)) { questionText.Append("<p><em>" + PstIW.WordingText + "</em></p>"); }           

            return questionText.ToString();
        }

        public string GetQuestionTextHTML2(bool colorLitQ = false)
        {
            StringBuilder questionText = new StringBuilder();

            if (!string.IsNullOrEmpty(PrePW.WordingText)) { questionText.Append("<p><strong>" + PrePW.WordingText + "</strong></p>"); }
            if (!string.IsNullOrEmpty(PreIW.WordingText)) { questionText.Append("<p><em>" + PreIW.WordingText + "</em></p>"); }
            if (!string.IsNullOrEmpty(PreAW.WordingText)) { questionText.Append("<p>" + PreAW.WordingText + "</p>"); }

            if (!string.IsNullOrEmpty(LitQW.WordingText))
            {
                if (colorLitQ)
                    questionText.Append("<p style=\"margin-left: 16px\"><font color=\"blue\">" + LitQW.WordingText + "</font></p>");
                else
                    questionText.Append("<p style=\"margin-left: 16px\">" + LitQW.WordingText + "</p>");
            }

            if (!string.IsNullOrEmpty(RespOptionsS.RespList))
            {
                string[] lines = RespOptionsS.RespList.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);

                questionText.Append("<p style=\"margin-left: 48px\">" + string.Join("</p><p style=\"margin-left: 48px\">", lines) + "</p>");
            }
            if (!string.IsNullOrEmpty(NRCodesS.RespList))
            {
                string[] lines = NRCodesS.RespList.Split(new string[] { "<br>" }, StringSplitOptions.RemoveEmptyEntries);
                questionText.Append("<p style=\"margin-left: 48px\">" + string.Join("</p><p style=\"margin-left: 48px\">", lines) + "</p>");
            }
            if (!string.IsNullOrEmpty(PstIW.WordingText)) { questionText.Append("<p><em>" + PstIW.WordingText + "</em></p>"); }
            if (!string.IsNullOrEmpty(PstPW.WordingText)) { questionText.Append("<p><strong>" + PstPW.WordingText + "</strong></p>"); }

            if (Images.Count > 0)
                questionText.Append("<p>Image filename: " + string.Join("</p><p>Image filename: ", Images.Select(x => x.ImageName)) + "</p>");

            return questionText.ToString();
        }

        public string GetQuestionText(string newline = "\r\n")
        {
            string questionText = "";

            if (!string.IsNullOrEmpty(PrePW.WordingText)) { questionText += "<strong>" + PrePW.WordingText + "</strong>" + newline; }
            //if (!string.IsNullOrEmpty(PreI)) { questionText += "<em>" + PreI + "</em>" + newline; }
            if (!string.IsNullOrEmpty(PreIW.WordingText)) { questionText += "<em>" + PreIW.WordingText + "</em>" + newline; }
            if (!string.IsNullOrEmpty(PreAW.WordingText)) { questionText += PreAW.WordingText + newline; }

            if (!string.IsNullOrEmpty(LitQW.WordingText)) { questionText += "[indent]" + LitQW.WordingText + "[/indent]" + newline; }

            if (!string.IsNullOrEmpty(RespOptionsS.RespList)) { questionText += "[indent3]" + RespOptionsS.RespList + "[/indent3]" + newline; }
            if (!string.IsNullOrEmpty(NRCodesS.RespList)) { questionText += "[indent3]" + NRCodesS.RespList + "[/indent3]" + newline; }
            if (!string.IsNullOrEmpty(PstIW.WordingText)) { questionText += "<em>" + PstIW.WordingText + "</em>" + newline; }
            if (!string.IsNullOrEmpty(PstPW.WordingText)) { questionText += "<strong>" + PstPW.WordingText + "</strong>"; }

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);

            return questionText;
        }

        public string GetQuestionTextPlain(string newline = "\r\n")
        {
            string questionText = "";

            if (!string.IsNullOrEmpty(PrePW.WordingText)) { questionText += PrePW.WordingText + newline; }
            if (!string.IsNullOrEmpty(PreIW.WordingText)) { questionText += PreIW.WordingText + newline; }
            if (!string.IsNullOrEmpty(PreAW.WordingText)) { questionText += PreAW.WordingText + newline; }

            if (!string.IsNullOrEmpty(LitQW.WordingText)) { questionText += LitQW.WordingText + newline; }

            if (!string.IsNullOrEmpty(RespOptionsS.RespList)) { questionText += RespOptionsS.RespList + newline; }
            if (!string.IsNullOrEmpty(NRCodesS.RespList)) { questionText += NRCodesS.RespList + newline; }
            if (!string.IsNullOrEmpty(PstIW.WordingText)) { questionText += PstIW.WordingText + newline; }
            if (!string.IsNullOrEmpty(PstPW.WordingText)) { questionText += PstPW.WordingText; }

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);

            return questionText;
        }

        public string GetQuestionText(bool colorLitQ)
        {
            string questionText = "";
            string newline = "\r\n";

            if (!string.IsNullOrEmpty(PrePW.WordingText)) { questionText += "<strong>" + PrePW.WordingText + "</strong>" + newline; }
            if (!string.IsNullOrEmpty(PreIW.WordingText)) { questionText += "<em>" + PreIW.WordingText + "</em>" + newline; }
            if (!string.IsNullOrEmpty(PreAW.WordingText)) { questionText += PreAW.WordingText + newline; }

            if (!string.IsNullOrEmpty(LitQW.WordingText))
            {
                if (colorLitQ)
                    questionText += "[indent][lblue]" + LitQW.WordingText + "[/lblue][/indent]" + newline;
                else
                    questionText += "[indent]" + LitQW.WordingText + "[/indent]" + newline;
            }

            if (!string.IsNullOrEmpty(RespOptionsS.RespList)) { questionText += "[indent3]" + RespOptionsS.RespList + "[/indent3]" + newline; }
            if (!string.IsNullOrEmpty(NRCodesS.RespList)) { questionText += "[indent3]" + NRCodesS.RespList + "[/indent3]" + newline; }
            if (!string.IsNullOrEmpty(PstIW.WordingText)) { questionText += "<em>" + PstIW.WordingText + "</em>" + newline; }
            if (!string.IsNullOrEmpty(PstPW.WordingText)) { questionText += "<strong>" + PstPW.WordingText + "</strong>"; }

            // replace all "<br>" tags with newline characters
            questionText = questionText.Replace("<br>", newline);
            questionText = Utilities.TrimString(questionText, newline);

            return questionText;
        }      

        public string GetEnglishRoutingTranslation(string lang, RoutingStyle routingStyle = RoutingStyle.Normal)
        {
            if (Translations == null || Translations.Count == 0)
                return string.Empty;

            StringBuilder sb = new StringBuilder();

            foreach (Translation t in Translations)
            {
                if (t.Language.Equals(lang))
                {
                    sb.Append(t.TranslationText);
                    // if heading and there is a translation do not proceed to English Routing
                    if (VarName.VarName.StartsWith("Z") && !string.IsNullOrEmpty(sb.ToString()))
                        continue;
                    // if not a heading and there is no translation, do not add English Routing
                    if (!VarName.VarName.StartsWith("Z") && string.IsNullOrEmpty(t.TranslationText))
                        continue;


                    if (!string.IsNullOrEmpty(PrePW.WordingText))
                    {
                        // insert PreP in the desired style
                        switch (routingStyle)
                        {
                            case RoutingStyle.Normal:
                                sb.Insert(0, "<strong>" + PrePW.WordingText + "</strong><br>");
                                break;
                            case RoutingStyle.Grey:
                                sb.Insert(0, "<strong><font color=\"#a6a6a6\">" + PrePW.WordingText + "</font></strong><br>");
                                break;
                            case RoutingStyle.None:
                                break;
                        }
                    }

                    if (!string.IsNullOrEmpty(PstPW.WordingText))
                    {
                        switch (routingStyle)
                        {
                            case RoutingStyle.Normal:
                                sb.Append("<br><strong>" + PstPW.WordingText + "</strong>");
                                break;
                            case RoutingStyle.Grey:
                                sb.Append("<br><strong><font color=\"#a6a6a6\">" + PstPW.WordingText + "</font></strong>");
                                break;
                            case RoutingStyle.None:
                                break;
                        }
                    }

                    break;
                }
            }
            return sb.ToString();
        }

        public string GetTranslationText(string lang)
        {
            if (Translations == null || Translations.Count == 0)
                return string.Empty;

            foreach (Translation t in Translations)
            {
                if (t.LanguageName.LanguageName.Equals(lang))
                    return t.TranslationText;
            }

            return string.Empty;
        }


        public Translation GetTranslation(string lang)
        {
            if (Translations == null || Translations.Count == 0)
                return null;

            foreach (Translation t in Translations)
            {
                if (t.LanguageName.LanguageName.Equals(lang))
                    return t;
            }

            return null;
        }

        public string GetComments()
        {
            string comments = "";
            foreach (QuestionComment qc in Comments)
            {
                comments += qc.GetComments() + "\r\n";
            }

            return Utilities.TrimString(comments, "\r\n");
        }

        /// <summary>
        /// Returns the Qnum formatted as a normal Qnum. If the Qnum contains 2 qnums, with a ^, this returns the 2nd qnum.
        /// </summary>
        /// <returns></returns>
        public string GetQnum()
        {
            // if contains ^ (re-inserted mark) get the 2nd qnum
            // else return the whole qnum
            if (Qnum.Contains ("^"))
                return Qnum.Substring(Qnum.LastIndexOf("^") + 1);
            else
                return Qnum;
        }

        /// <summary>
        /// Returns the integer value of the Qnum
        /// </summary>
        /// <returns></returns>
        public int GetQnumValue()
        {
            return Int32.Parse(Qnum.Substring(0, 3));
        }

        public List<string> GetFilterVars()
        {
            List<string> filterVars = new List<string>();

            if (string.IsNullOrEmpty(PrePW.WordingText))
                return filterVars;

            Regex rx1 = new Regex("[A-Z][A-Z][0-9][0-9][0-9][a-z]*");

            // find all VarNames in the prep
            MatchCollection matches = rx1.Matches(PrePW.WordingText);

            if (matches.Count == 0)
                return filterVars;

            foreach (Match m in matches)
            {
                filterVars.Add(m.Value);
            }

            return filterVars;
        }

        /// <summary>
        /// Return a list of FilterInstruction objects found in this question's PreP. 
        /// </summary>
        /// <returns></returns>
        public List<FilterInstruction> GetFilterInstructions()
        {
            List<FilterInstruction> filterVars = new List<FilterInstruction>();

            if (string.IsNullOrEmpty(PrePW.WordingText))
                return filterVars;

            if (!PrePW.WordingText.Contains("Ask if"))
                return filterVars;

            string decodedPreP = System.Web.HttpUtility.HtmlDecode(PrePW.WordingText);

            // check if "any of" list exists

            if (decodedPreP.StartsWith("Ask if any of ("))
            {
                filterVars.AddRange(GetAnyOfList());
            }
                

            Regex rx1 = new Regex("([A-Z][A-Z][0-9][0-9][0-9][a-z]*)" +
                                "(=|<|>|<>)" +
                                "(([0-9]+(,\\s[0-9]+)*\\sor\\s[0-9]+)" +
                                "|([0-9]+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\-[0-9]+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\-[0-9]+)" +
                                "|([0-9]+\\sand\\s<[0-9]+)" +
                                "|([0-9]+\\sand\\s>[0-9]+)" +
                                "|([0-9]+))");

            // find all VarNames in the prep
            MatchCollection matches = rx1.Matches(decodedPreP);

            if (matches.Count == 0)
                return filterVars;
            
            // get the varname, operation and numbers from the groups in the match

            foreach (Match m in matches)
            {
                string match = m.Groups[0].Value;
                string var = m.Groups[1].Value;
                string condition = m.Groups[2].Value;
                Operation op = Operation.Equals;
                switch(condition)
                {
                    case "=":
                        op = Operation.Equals;
                        break;
                    case "<>":
                        op = Operation.NotEquals;
                        break;
                    case "<":
                        op = Operation.LessThan;
                        break;
                    case ">":
                        op = Operation.GreaterThan;
                        break;
                }
                string number = m.Groups[3].Value;

                if (string.IsNullOrEmpty(number))
                    continue;

                // determine number range if present
                List<int> numbers = GetNumberRange(number);
                List<string> numbersStr;
                if (numbers.Count() < 100)
                {
                    numbersStr = GetNumberRangeStr(number);
                }
                else
                {
                    numbersStr = new List<string> { numbers[0].ToString(), numbers[numbers.Count-1].ToString() };
                }

                bool range = number.Contains("-");

                FilterInstruction fi = new FilterInstruction();
                fi.VarName = var;
                fi.Oper = op;
                fi.Values = numbers;
                fi.ValuesStr = numbersStr;
                fi.Range = range;
                fi.FilterExpression = match;

                filterVars.Add(fi);
            }
            return filterVars;
        }

        private List<FilterInstruction> GetAnyOfList()
        {
            List<FilterInstruction> list = new List<FilterInstruction>();

            
            Regex rx1 = new Regex("([A-Z][A-Z][0-9][0-9][0-9][a-z]*(,\\s*[A-Z][A-Z][0-9][0-9][0-9][a-z]*)*)\\)" +
                                "(=|<|>|<>)" +
                                "(([0-9]+(,\\s[0-9]+)*\\sor\\s[0-9]+)" +
                                "|([0-9]+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\-[0-9]+\\sor\\s[0-9]+)" +
                                "|([0-9]+\\-[0-9]+)" +
                                "|([0-9]+))");

            // find all VarNames in the prep
            MatchCollection matches = rx1.Matches(PrePW.WordingText);

            if (matches.Count == 0)
                return list;


           
            // get the varname, operation and numbers from the groups in the match

            foreach (Match m in matches)
            {
                string match = m.Groups[0].Value;
                string vars = m.Groups[1].Value;
                string condition = m.Groups[3].Value;
                Operation op = Operation.Equals;
                switch (condition)
                {
                    case "=":
                        op = Operation.Equals;
                        break;
                    case "<>":
                        op = Operation.NotEquals;
                        break;
                    case "<":
                        op = Operation.LessThan;
                        break;
                    case ">":
                        op = Operation.GreaterThan;
                        break;
                }
                string number = m.Groups[4].Value;

                if (string.IsNullOrEmpty(number))
                    continue;

                // determine number range if present
                List<int> numbers = GetNumberRange(number);
                List<string> numbersStr = GetNumberRangeStr(number);

                bool range = number.Contains("-");
                
                foreach (string v in vars.Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries))
                {

                    FilterInstruction fi = new FilterInstruction();
                    fi.VarName = v;
                    fi.Oper = op;
                    fi.Values = numbers;
                    fi.ValuesStr = numbersStr;
                    fi.Range = range;
                    fi.FilterExpression = match;
                    fi.AnyOf = true;

                    list.Add(fi);
                }
            }

            return list;
        }

        

        private List<int> GetNumberRange(string nums)
        {
            List<int> numbers = new List<int>();
            string[] words = nums.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].Replace(",", "");
                bool s = Int32.TryParse(words[i], out int n);
                if (words[i].Contains("-"))
                {
                    int lower = Int32.Parse(words[i].Substring(0, words[i].IndexOf("-")));
                    int upper = Int32.Parse(words[i].Substring(words[i].IndexOf("-") + 1, words[i].Length - words[i].IndexOf("-") - 1));
                    for (int j = lower; j <= upper; j++)
                        numbers.Add(j);
                }
                else
                {

                }
                if (s) numbers.Add(n);
            }
            return numbers;
        }

        private List<string> GetNumberRangeStr(string nums)
        {
            List<string> numbers = new List<string>();
            string[] words = nums.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < words.Length; i++)
            {
                words[i] = words[i].Replace(",", "");
                int n = 0;
                bool s = Int32.TryParse(words[i], out n) || words[i].Equals("P") || words[i].Equals("C") ;
                if (words[i].Contains("-"))
                {
                    
                    int lower = Int32.Parse(words[i].Substring(0, words[i].IndexOf("-")));
                    int upper = Int32.Parse(words[i].Substring(words[i].IndexOf("-") + 1, words[i].Length - words[i].IndexOf("-") - 1));
                    for (int j = lower; j <= upper; j++)
                        numbers.Add(j.ToString());

                }
                else
                {
                    if (s) numbers.Add(words[i]);
                    
                }
            }
            return numbers;
        }


        public List<FilterInstruction> GetFilterInstructions(List<string> NonStdVars)
        {
            List<FilterInstruction> filterVars = new List<FilterInstruction>();

            if (string.IsNullOrEmpty(PrePW.WordingText))
                return filterVars;

            if (!PrePW.WordingText.Contains("Ask if"))
                return filterVars;

            foreach (string v in NonStdVars)
            {
                Regex rx1 = new Regex("(" + v + ")" +
                                    "(=|<|>|<>)" +
                                    "(([0-9]+(,\\s[0-9]+)*\\sor\\s[0-9]+)" +
                                    "|([0-9]+\\sor\\s[0-9]+)" +
                                    "|([0-9]+\\-[0-9]+)" +
                                    "|([0-9]+)" +
                                    "|([A-Z]))", RegexOptions.IgnoreCase);
                
                // find all VarNames in the prep
                MatchCollection matches = rx1.Matches(PrePW.WordingText);

                if (matches.Count == 0)
                    continue;


                // get the varname, operation and numbers from the groups in the match

                foreach (Match m in matches)
                {
                    string match = m.Groups[0].Value;
                    string var = m.Groups[1].Value;
                    string condition = m.Groups[2].Value;
                    Operation op = Operation.Equals;
                    switch (condition)
                    {
                        case "=":
                            op = Operation.Equals;
                            break;
                        case "<>":
                            op = Operation.NotEquals;
                            break;
                        case "<":
                            op = Operation.LessThan;
                            break;
                        case ">":
                            op = Operation.GreaterThan;
                            break;
                    }
                    string number = m.Groups[3].Value;

                    if (string.IsNullOrEmpty(number))
                        continue;

                    // determine number range if present
                    List<int> numbers = GetNumberRange(number);
                    List<string> numbersStr = GetNumberRangeStr(number);

                    bool range = number.Contains("-");

                    FilterInstruction fi = new FilterInstruction();
                    fi.VarName = var;
                    fi.Oper = op;
                    fi.Values = numbers;
                    fi.ValuesStr = numbersStr;
                    fi.Range = range;
                    fi.FilterExpression = match;

                    filterVars.Add(fi);
                }
            }
            return filterVars;
        }

        public List<string> GetRoutingVars()
        {
            List<string> routingVars = new List<string>();

            if (string.IsNullOrEmpty(PstPW.WordingText))
                return routingVars;

            Regex rx1 = new Regex("[A-Z][A-Z][0-9][0-9][0-9][a-z]*");

            // find all VarNames in the pstp
            MatchCollection matches = rx1.Matches(PstPW.WordingText);

            if (matches.Count == 0)
                return routingVars;

            foreach (Match m in matches)
            {
                routingVars.Add(m.Value);
            }

            return routingVars;
        }

        // TODO finish
        public Dictionary<int, string> GetRoutingInstructions()
        {
            Dictionary<int, string> routingVars = new Dictionary<int, string>();

            if (string.IsNullOrEmpty(PstPW.WordingText))
                return routingVars;

            Regex rx1 = new Regex("[A-Z][A-Z][0-9][0-9][0-9][a-z]*");

            // find all VarNames in the pstp
            MatchCollection matches = rx1.Matches(PstPW.WordingText);

            if (matches.Count == 0)
                return routingVars;

            foreach (Match m in matches)
            {
                //routingVars.Add(m.Value);
            }

            return routingVars;
        }

        public List<string> GetRespNumbers(bool both = false)
        {

            List<string> responseList;
            int space;
            responseList = RespOptionsS.RespList.Split(new string [] { "\r\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (both)
                responseList.AddRange(NRCodesS.RespList.Split(new string[] { "\r\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList());

            for (int s = 0; s<responseList.Count; s ++)
            {
                for (int i = 0; i < responseList[s].Length; i++)
                {
                    if (char.IsWhiteSpace(responseList[s][i]))
                    {
                        space = i;
                        responseList[s] = responseList[s].Substring(0, i);
                        break;

                    }
                }
                
            }

            return responseList;
        }

        public List<string> GetRespLabels(bool both = false)
        {

            List<string> responseList;
            int space;
            responseList = RespOptionsS.RespList.Split(new string[] { "\r\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            if (both)
                responseList.AddRange(NRCodesS.RespList.Split(new string[] { "\r\n", "<br>" }, StringSplitOptions.RemoveEmptyEntries).ToList());

            for (int s = 0; s < responseList.Count; s++)
            {
                for (int i = 0; i < responseList[s].Length; i++)
                {
                    if (char.IsWhiteSpace(responseList[s][i]))
                    {
                        space = i;
                        responseList[s] = responseList[s].Substring(i);
                        responseList[s] = responseList[s].TrimStart(new char[] { ' ' });
                        break;

                    }
                }

            }

            return responseList;
        }

        public List<string> GetNonRespNumbers()
        {

            List<string> responseList;
            int space;
            
            responseList = NRCodesS.RespList.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();

            for (int s = 0; s < responseList.Count; s++)
            {
                for (int i = 0; i < responseList[s].Length; i++)
                {
                    if (!char.IsNumber(responseList[s][i]))
                    {
                        space = i;
                        responseList[s] = responseList[s].Substring(0, i);
                        break;

                    }
                }

            }

            return responseList;
        }

        /// <summary>
        /// Returns altered versions of the PreI, LitQ and Response Options to conform to Semi-tel specifications.
        /// </summary>
        /// <param name="ChangedPreI"></param>
        /// <param name="ChangedLitQ"></param>
        /// <param name="ChangedResponseOptions"></param>
        public void FormatSemiTel(out string ChangedPreI, out string ChangedLitQ, out string ChangedResponseOptions)
        {
            // replace "flash card" with "read out response options" in PreI
            // add ", or" to second last line of response options
            if (!PreIW.WordingText.Contains("flash card") && !PreIW.WordingText.Contains("response options"))
            {
                ChangedPreI = this.PreIW.WordingText;
                ChangedLitQ = this.LitQW.WordingText;
                ChangedResponseOptions = this.RespOptionsS.RespList;
                return;
            }
            else if (PreIW.WordingText.ToLower().Contains("don't read out response options") || PreIW.WordingText.ToLower().Contains("do not read out response options"))
            {
                ChangedPreI = this.PreIW.WordingText;
                ChangedLitQ = this.LitQW.WordingText;
                ChangedResponseOptions = this.RespOptionsS.RespList;
                return;
            }

            ChangedPreI = PreIW.WordingText.Replace("flash card", "Read out response options");
            ChangedLitQ = this.LitQW.WordingText;
            ChangedResponseOptions = this.RespOptionsS.RespList;

            if (IsSeries())
            {
                if (!Qnum.EndsWith("a"))
                {
                    ChangedResponseOptions = RespOptionsWithOr();
                    if (!LitQW.WordingText.Contains("Would you say"))
                        ChangedLitQ = this.LitQW.WordingText + " Would you say...?";
                }
            }
            else
            {
                ChangedResponseOptions = RespOptionsWithOr();
                if (!LitQW.WordingText.Contains("Would you say"))
                    ChangedLitQ = this.LitQW.WordingText + " Would you say...?";
            }
        }

        public string RespOptionsWithOr()
        {
            string[] options = RespOptionsS.RespList.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
            options[options.Length - 2] = options[options.Length - 2] + ", or";
            return string.Join("\r\n", options );
        }

        public bool IsSeries()
        {
            return char.IsLetter(Qnum[Qnum.Length-1]);
        }

        public bool IsDerived()
        {
            return this.VarName.RefVarName.EndsWith("v") ||
                this.PrePW.WordingText.ToLower().StartsWith("programmer:") || 
                this.PrePW.WordingText.ToLower().StartsWith("programming instructions:") ||
                this.PrePW.WordingText.ToLower().Contains("derived variable") ||            
                this.PreIW.WordingText.ToLower().Contains("derived variable") ||
                this.PreAW.WordingText.ToLower().Contains("derived variable") ||
                this.LitQW.WordingText.ToLower().Contains("derived variable") ||
                this.PstIW.WordingText.ToLower().Contains("derived variable") ||
                this.PstPW.WordingText.ToLower().Contains("derived variable");
        }

        public bool IsProgramming()
        {
            return this.PrePW.WordingText.ToLower().StartsWith("programmer:") || this.PrePW.WordingText.StartsWith("programming instructions:") || 
                this.PreAW.WordingText.Replace("<strong>", "").Replace("<u>", "").ToLower().StartsWith("programmer:") || this.PreAW.WordingText.Replace("<strong>","").Replace("<u>", "").StartsWith("programming instructions:") ||
                this.LitQW.WordingText.Replace("<strong>", "").Replace("<u>", "").ToLower().StartsWith("programmer:") || this.LitQW.WordingText.Replace("<strong>", "").Replace("<u>", "").StartsWith("programming instructions:");
        }

        public bool IsTermination()
        {
            return this.VarName.RefVarName.StartsWith("BI9");
        }

        public bool IsHeading()
        {
            return this.VarName.RefVarName.StartsWith("Z") && !this.VarName.RefVarName.EndsWith("s");
        }

        public bool IsSubHeading()
        {
            return this.VarName.RefVarName.StartsWith("Z") && this.VarName.RefVarName.EndsWith("s");
        }

        public bool IsBlank()
        {
            return PrePW.WordID == 0 && PreIW.WordID == 0 && PreAW.WordID == 0 && LitQW.WordID == 0 && PstIW.WordID == 0 && PstPW.WordID == 0 && 
                RespOptionsS.RespSetName.Equals("0") && NRCodesS.RespSetName.Equals("0");
        }


        public int GetNumCols()
        {
            if (RespOptionsS.RespSetName.Equals("0") && NRCodesS.RespSetName.Equals("0"))
                return 0;

            return GetRespNumbers(true)[0].Length;
            
        }

        public string GetVarType()
        {
            string type = string.Empty;

            if (VarName.RefVarName.EndsWith("hdg") || VarName.RefVarName.StartsWith("Z") || VarName.RefVarName.StartsWith("HG"))
                type = string.Empty;
            else if (VarName.RefVarName.EndsWith("o"))
                type = "string";
            else
            {
                if (string.IsNullOrEmpty(RespOptionsS.RespList) && string.IsNullOrEmpty(NRCodesS.RespList))
                    type = string.Empty; // derived or other instruction
                else
                    type = "numeric";
            }

            return type;
        }
        

        public void ChangeRefVarName(string newRefVarName)
        {
            VarName.RefVarName = newRefVarName;
        }

        public string GetRefVarName()
        {
            return VarName.RefVarName;
        }

        public bool ContainsString(string searchFor, int startAt = 0)
        {
            string fullText = GetQuestionTextPlain();

            if (startAt > fullText.Length)
                return false;

            return fullText.IndexOf(searchFor,startAt)>-1;
        }

        public bool ContainsString(string field, string searchFor, int startAt = 0)
        {
            switch (field)
            {
                case "PreP":
                    if (startAt < PrePW.WordingText.Length)
                        return PrePW.WordingText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;

                case "PreI":
                    if (startAt < PreIW.WordingText.Length)
                        return PreIW.WordingText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "PreA":
                    if (startAt < PreAW.WordingText.Length)
                        return PreAW.WordingText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "LitQ":
                    if (startAt < LitQW.WordingText.Length)
                        return LitQW.WordingText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "PstI":
                    if (startAt < PstIW.WordingText.Length)
                        return PstIW.WordingText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "PstP":
                    if (startAt < PstPW.WordingText.Length)
                        return PstPW.WordingText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "RespOptions":
                    if (startAt < RespOptionsS.RespList.Length)
                        return RespOptionsS.RespList.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "NRCodes":
                    if (startAt < NRCodesS.RespList.Length)
                        return NRCodesS.RespList.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "VarLabel":
                    if (startAt < VarName.VarLabel.Length)
                        return VarName.VarLabel.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "Content":
                    if (startAt < VarName.Content.LabelText.Length)
                        return VarName.Content.LabelText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "Topic":
                    if (startAt < VarName.Topic.LabelText.Length)
                        return VarName.Topic.LabelText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "Domain":
                    if (startAt < VarName.Domain.LabelText.Length)
                        return VarName.Domain.LabelText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
                case "Product":
                    if (startAt < VarName.Product.LabelText.Length)
                        return VarName.Product.LabelText.IndexOf(searchFor, startAt) > -1;
                    else
                        return false;
            }

            return false;
        }

        public override string ToString()
        {
            return VarName.RefVarName;
        }

        #region Private backing variables
        private string _qnum;
        public string _altqnum;
        public string _altqnum2;
        public string _altqnum3;
        private Wording _prepw;
        private Wording _preiw;
        private Wording _preaw;
        private Wording _litqw;
        private Wording _pstiw;
        private Wording _pstpw;
        private ResponseSet _respoptionss;
        private ResponseSet _nrcodess;
        private string _prep;
        private int _prepnum;
        private string _prei;
        private int _preinum;
        private string _prea;
        private int _preanum;
        private string _litq;
        private int _litqnum;
        private string _psti;
        private int _pstinum;
        private string _pstp;
        private int _pstpnum;
        private string _respoptions;
        private string _respname;
        private string _nrcodes;
        private string _nrname;
        private string _plainfilter;
        #endregion
    }

    public class QuestionUsage : SurveyQuestion
    {
        public string SurveyList { get; set; }

        public QuestionUsage() : base()
        {
            SurveyList = string.Empty;
        }
    }

    public class QuestionTimeFrame
    {
        public int ID { get; set; }
        public int QID { get; set; }
        public string TimeFrame { get; set; }

        public QuestionTimeFrame()
        {
            TimeFrame = string.Empty;
        }
    }
}
