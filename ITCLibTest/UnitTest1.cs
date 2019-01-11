using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITCSurveyReportLib;
using System.Diagnostics;

namespace ISISLibTest
{
    [TestClass]
    public class DBActionsTest
    {
       
        [TestMethod]
        public void TestVarNameChangesByID()
        {
            VarNameChange vc = DBAction.GetVarNameChangeByID(1);

            Assert.IsNotNull(vc);

            Assert.AreEqual(vc.ID, 1);
            Assert.AreEqual(vc.OldName.VarName, "BQ11404");
            Assert.AreEqual(vc.NewName.VarName, "BQ11160");
            

        }

        [TestMethod]
        public void TestQuestionRouting()
        {
            SurveyQuestion sq = DBAction.GetSurveyQuestion(19);

            Assert.IsNotNull(sq);

            QuestionRouting qr = new QuestionRouting(sq.PstP, sq.RespOptions + "\r\n" + sq.NRCodes);
            
            Trace.WriteLine(qr.ToString());
            

        }
    }
}
