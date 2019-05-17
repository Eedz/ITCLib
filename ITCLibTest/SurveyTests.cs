using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITCLib;

namespace ITCLibTest
{
    /// <summary>
    /// Summary description for SurveyTests
    /// </summary>
    [TestClass]
    public class SurveyTests
    {
        Survey survey;

        public SurveyTests()
        {
            survey = new Survey("TS1");

            SurveyQuestion sq;

            sq = new SurveyQuestion("AA000", "001");

            survey.AddQuestion(sq);

            sq = new SurveyQuestion("AA001", "002");

            survey.AddQuestion(sq);

            sq = new SurveyQuestion("AA002", "003");

            survey.AddQuestion(sq);

            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion

        [TestMethod]
        public void AddQuestionToEnd()
        {
            SurveyQuestion sq = new SurveyQuestion("AA004", "002");
            

            survey.AddQuestion(sq);

            Assert.IsTrue(survey.Questions[3].Qnum == "004");
        }

        [TestMethod]
        public void AddQuestionToBeginning()
        {
            SurveyQuestion sq = new SurveyQuestion("AA004", "002");


            survey.AddQuestion(sq, 0);

            Assert.IsTrue(survey.Questions[0].Qnum == "001");
        }

        [TestMethod]
        public void AddQuestionToMiddle()
        {
            SurveyQuestion sq = new SurveyQuestion("AA004", "002");


            survey.AddQuestion(sq, 1);

            Assert.IsTrue(survey.Questions[2].Qnum == "003");
        }

        [TestMethod]
        public void RemoveQuestionFromMiddle()
        {
            
            survey.RemoveQuestion(survey.Questions[1]);

            Assert.IsTrue(survey.Questions[1].Qnum == "002");
        }

        [TestMethod]
        public void CorrectWordings()
        {

        }

        [TestMethod]
        public void CreateFilterList()
        {
            SurveyQuestion q1 = new SurveyQuestion("AA000", "001");
            q1.PreP = "Ask if FR326=1.";

            SurveyQuestion q2 = new SurveyQuestion("FR326", "002");
            q2.RespOptions = "1  Yes\r\n2  No";
            q2.NRCodes = "8  Refused\r\n9  Don't Know";
            q2.VarLabel = "Varlabel for FR326";

            Survey s = new Survey();
            s.AddQuestion(q1);
            s.AddQuestion(q2);

            s.MakeFilterList();

            Assert.IsTrue(s.Questions[0].Filters.Length > 0);
        }
    }
}
