using System;
using ITCLib;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data;

namespace ITCLibTest
{
    [TestClass]
    public class SurveyReportTests
    {
        [TestMethod]
        public void SurveyReportAddCurrentSurvey()
        {
            SurveyReport SR = new SurveyReport();
            ReportSurvey RS = new ReportSurvey("6E2");

            SR.AddSurvey(RS);

            Assert.IsTrue(SR.Surveys[0].ID == 1);
            Assert.IsTrue(SR.Surveys[0].Primary);
            Assert.IsTrue(SR.Surveys[0].Qnum);

        }

        [TestMethod]
        public void SurveyReportAdd2CurrentSurveys()
        {
            SurveyReport SR = new SurveyReport();
            ReportSurvey RS = new ReportSurvey("6E2");

            SR.AddSurvey(RS);

            RS = new ReportSurvey("6E1");

            SR.AddSurvey(RS);

            Assert.IsTrue(SR.Surveys[0].ID == 1);
            Assert.IsTrue(SR.Surveys[1].ID == 2);
            Assert.IsTrue(SR.Surveys[1].Primary);
            Assert.IsFalse(SR.Surveys[0].Primary);
            Assert.IsTrue(SR.Surveys[0].Qnum);

        }

        [TestMethod]
        public void SurveyReportAdd3CurrentSurveys()
        {
            SurveyReport SR = new SurveyReport();
            ReportSurvey RS = new ReportSurvey("6E2");

            SR.AddSurvey(RS);

            RS = new ReportSurvey("6E1");

            SR.AddSurvey(RS);

            RS = new ReportSurvey("6E2.5-ES");

            SR.AddSurvey(RS);

            Assert.IsTrue(SR.Surveys[0].ID == 1);
            Assert.IsTrue(SR.Surveys[1].ID == 2);
            Assert.IsTrue(SR.Surveys[2].ID == 3);

            Assert.IsTrue(SR.Surveys[0].Primary);
            Assert.IsFalse(SR.Surveys[1].Primary);
            Assert.IsFalse(SR.Surveys[2].Primary);
            Assert.IsTrue(SR.Surveys[0].Qnum);
        }

        [TestMethod]
        public void SurveyReportAdd2CurrentSurveysRemove1()
        {
            SurveyReport SR = new SurveyReport();
            ReportSurvey RS = new ReportSurvey("6E2");
            ReportSurvey RS2 = new ReportSurvey("6E1");

            SR.AddSurvey(RS);

            SR.AddSurvey(RS2);

            SR.RemoveSurvey(RS);

            Assert.IsTrue(SR.Surveys[0].ID == 1);
            Assert.IsTrue(SR.Surveys[0].Primary);
            Assert.IsTrue(SR.Surveys[0].Qnum);

        }

        [TestMethod]
        // add 3 surveys, then remove the first added survey. The 2nd of the remaining surveys should be primary
        public void SurveyReportAdd3CurrentSurveysRemove2()
        {
            SurveyReport SR = new SurveyReport();
            ReportSurvey RS = new ReportSurvey("6E2");
            ReportSurvey RS2 = new ReportSurvey("6E1");
            ReportSurvey RS3 = new ReportSurvey("6E2.5-ES");

            SR.AddSurvey(RS);
            SR.AddSurvey(RS2);
            SR.AddSurvey(RS3);

            SR.RemoveSurvey(RS);

            Assert.IsTrue(SR.Surveys[0].ID == 1);
            Assert.IsFalse(SR.Surveys[0].Primary);

            Assert.IsTrue(SR.Surveys[1].Primary);
            Assert.IsTrue(SR.Surveys[0].Qnum);

        }

        [TestMethod]
        public void SingleSurveySingleQuestionReportTable()
        {
            SurveyReport SR = new SurveyReport();
            ReportSurvey s = new ReportSurvey("Test");

            SR.AddSurvey(s);

            SurveyQuestion sq = new SurveyQuestion();
            sq.VarName.VarName = "AA000";
            sq.Qnum = "000";
            sq.PreP = "Test PreP";
            sq.LitQ = "Test LitQ";
            sq.RespOptions = "1   Yes";

            s.AddQuestion(sq);

            int result = SR.GenerateReport();

            Assert.IsTrue(result == 0);

            Assert.IsTrue(SR.ReportTable.Rows.Count == 1);
        }

        [TestMethod]
        public void SingleSurveyMultiQuestionReportTable()
        {
            SurveyReport SR = new SurveyReport();
            ReportSurvey s = new ReportSurvey("Test");

            SR.AddSurvey(s);

            for (int i = 0; i < 10; i++)
            {
                SurveyQuestion sq = new SurveyQuestion();
                sq.VarName.VarName = "AA" + i.ToString("000");
                sq.Qnum = i.ToString("000");
                sq.PreP = "Test PreP" + i;
                sq.LitQ = "Test LitQ" + i;
                for (int j = 1; j <= i; j++)
                    sq.RespOptions += j + "   Response Option " + j;

                s.AddQuestion(sq);
            }

            int result = SR.GenerateReport();

            Assert.IsTrue(result == 0);

            Assert.IsTrue(SR.ReportTable.Rows.Count == 10);
        }



        [TestMethod]
        public void DefaultSingleSurveyWithComments()
        {
          
        }
    }
}
