using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITCLib;
using System.Diagnostics;

namespace ITCLibTest
{
    [TestClass]
    public class TestQuestionRouting
    {
       
        [TestMethod]
        public void TestQuestionRouting1Column_1GoTo_ReturnsWithGoTo()
        {
            // Arrange
            string pstp = "If response=1, go to BI901.";
            string respoptions = "1   Yes\r\n2   No\r\n8   Refused\r\n9   Don't know";

            // Act 
            QuestionRouting qr = new QuestionRouting(pstp, respoptions );
            
            // Assert
            Assert.AreEqual(qr.ToString(), "1   Yes <strong>=> go to BI901.</strong>\r\n2   No\r\n8   Refused\r\n9   Don't know");
        }

        [TestMethod]
        public void TestQuestionRouting2Column()
        {
            string pstp = "If response=1, go to BI901.";
            string respoptions = "01   Yes\r\n02   No\r\n88   Refused\r\n99   Don't know";

            QuestionRouting qr = new QuestionRouting(pstp, respoptions);


            Trace.WriteLine(qr.ToString());
            // Assert
            Assert.AreEqual(qr.ToString(), "01   Yes <strong>=> go to BI901.</strong>\r\n02   No\r\n88   Refused\r\n99   Don't know");
        }

        [TestMethod]
        public void TestQuestionRouting1Column_2GoTo_ReturnsWithGoTo()
        {
            string pstp = "If response=1, go to BI901.<br>If response=2, go to BI902.";
            string respoptions = "1   Yes\r\n2   No\r\n8   Refused\r\n9   Don't know";
            QuestionRouting qr = new QuestionRouting(pstp, respoptions);


            Trace.WriteLine(qr.ToString());

            // Assert
            Assert.AreEqual(qr.ToString(), "1   Yes <strong>=> go to BI901.</strong>\r\n2   No <strong>=> go to BI902.</strong>\r\n8   Refused\r\n9   Don't know");
        }

       


    }
}
