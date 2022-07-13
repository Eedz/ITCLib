using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITCLib;

namespace ITCLibTest
{
    /// <summary>
    /// Summary description for ComparisonTests
    /// </summary>
    [TestClass]
    public class ComparisonTests
    {
        Comparison comparer;
        ReportSurvey primary;
        ReportSurvey other;

        public ComparisonTests()
        {
            primary = new ReportSurvey("TT1");

            SurveyQuestion sq1 = new SurveyQuestion("BI100", "001");
            SurveyQuestion sq2 = new SurveyQuestion("BI101", "002");

            primary.AddQuestion(sq1);
            primary.AddQuestion(sq2);

            other = new ReportSurvey("TT2");

            other.AddQuestion(sq1);
            other.AddQuestion(sq2);

            comparer = new Comparison(primary, other);


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
        [TestCategory("Comparison")]
        public void Compare_ReInsert_ExtraOtherQuestion_OtherQnum()
        {
            // default comparison settings: show and reinsert deletions
            // other survey has an extra question
            // other survey is Qnum survey

            other.Qnum = true;

            SurveyQuestion extra = new SurveyQuestion("EX100", "003");

            other.AddQuestion(extra);

            comparer.CompareByVarName();

            Assert.IsTrue(extra.VarName.VarName.StartsWith("[yellow]"));
        }

        [TestMethod]
        [TestCategory("Comparison")]
        public void Compare_ReInsert_ExtraPrimaryQuestion_OtherQnum()
        {
            // default comparison settings: show and reinsert deletions
            // primary survey has an extra question
            // extra question should be added to other survey and original question unchanged

            other.Qnum = true;

            SurveyQuestion extra = new SurveyQuestion("EX100", "003");

            primary.AddQuestion(extra);

            comparer.CompareByVarName();

            SurveyQuestion extra1 = other.Questions[other.Questions.Count - 1];

            Assert.IsFalse(extra.VarName.VarName.StartsWith("[s][t]"));
            Assert.IsTrue(extra1.VarName.VarName.StartsWith("[s][t]"));
        }
    }
}
