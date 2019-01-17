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
        public void TestQuestionRouting1Column ()
        {
            string pstp = "If response=1, go to BI901.";
            string respoptions = "1   Yes\r\n2   No\r\n8   Refused\r\n9   Don't know";
            QuestionRouting qr = new QuestionRouting(pstp, respoptions );
               
            
            Trace.WriteLine(qr.ToString());
            

        }

        [TestMethod]
        public void TestQuestionRouting2Column()
        {
            string pstp = "If response=1, go to BI901.";
            string respoptions = "01   Yes\r\n02   No\r\n88   Refused\r\n99   Don't know";
            QuestionRouting qr = new QuestionRouting(pstp, respoptions);


            Trace.WriteLine(qr.ToString());


        }

        [TestMethod]
        public void TestQuestionRouting1ColumnMulti()
        {
            string pstp = "If response=1, go to BI901.<br>If response = 2, go to BI902.";
            string respoptions = "1   Yes\r\n2   No\r\n8   Refused\r\n9   Don't know";
            QuestionRouting qr = new QuestionRouting(pstp, respoptions);


            Trace.WriteLine(qr.ToString());


        }


    }
}
