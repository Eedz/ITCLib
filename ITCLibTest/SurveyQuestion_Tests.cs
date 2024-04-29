using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using ITCLib;

namespace ITCLibTest
{
    /// <summary>
    /// Summary description for SurveyQuestion_Tests
    /// </summary>
    [TestClass]
    public class SurveyQuestion_Tests
    {
        public SurveyQuestion_Tests()
        {
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
        public void ConstructorTest_VarName()
        {
            SurveyQuestion question = new SurveyQuestion("AA000");

            Assert.IsNotNull(question.PrePW);
            Assert.IsNotNull(question.PreIW);
            Assert.IsNotNull(question.PreAW);
            Assert.IsNotNull(question.LitQW);
            Assert.IsNotNull(question.PstIW);
            Assert.IsNotNull(question.PstPW);
            Assert.IsNotNull(question.RespOptionsS);
            Assert.IsNotNull(question.NRCodesS);

            Assert.IsNotNull(question.Comments);
            Assert.IsNotNull(question.Translations);
            Assert.IsNotNull(question.PreviousNameList);
            Assert.IsNotNull(question.TimeFrames);
            Assert.IsNotNull(question.Images);
        }

        [TestMethod]
        public void ConstructorTest_VarNameQnum()
        {
            SurveyQuestion question = new SurveyQuestion("AA000","001");

            Assert.IsNotNull(question.PrePW);
            Assert.IsNotNull(question.PreIW);
            Assert.IsNotNull(question.PreAW);
            Assert.IsNotNull(question.LitQW);
            Assert.IsNotNull(question.PstIW);
            Assert.IsNotNull(question.PstPW);
            Assert.IsNotNull(question.RespOptionsS);
            Assert.IsNotNull(question.NRCodesS);

            Assert.IsNotNull(question.Comments);
            Assert.IsNotNull(question.Translations);
            Assert.IsNotNull(question.PreviousNameList);
            Assert.IsNotNull(question.TimeFrames);
            Assert.IsNotNull(question.Images);
        }

        [TestMethod]
        public void ConstructorTest_VarNameQnumProduct()
        {
            ProductLabel product = new ProductLabel(0, "N/A");
            SurveyQuestion question = new SurveyQuestion("AA000", "001", product);

            Assert.IsNotNull(question.PrePW);
            Assert.IsNotNull(question.PreIW);
            Assert.IsNotNull(question.PreAW);
            Assert.IsNotNull(question.LitQW);
            Assert.IsNotNull(question.PstIW);
            Assert.IsNotNull(question.PstPW);
            Assert.IsNotNull(question.RespOptionsS);
            Assert.IsNotNull(question.NRCodesS);

            Assert.IsNotNull(question.Comments);
            Assert.IsNotNull(question.Translations);
            Assert.IsNotNull(question.PreviousNameList);
            Assert.IsNotNull(question.TimeFrames);
            Assert.IsNotNull(question.Images);
        }

        [TestMethod]
        public void FullVarNameTest_NoTimeFrame_NoBrackets()
        {
            SurveyQuestion question = new SurveyQuestion("AA000", "001");
            question.VarName.VarLabel = "VarLabel";

            string fullVarLabel = question.GetFullVarLabel();

            Assert.IsTrue(fullVarLabel.Equals("VarLabel"));
        }

        #region GetFullVarLabel
        [TestMethod]
        [TestCategory("GetFullVarLabel")]
        public void FullVarNameTest_NoTimeFrame_Brackets()
        {
            SurveyQuestion question = new SurveyQuestion("AA000", "001");
            question.VarName.VarLabel = "VarLabel {}";

            string fullVarLabel = question.GetFullVarLabel();

            Assert.IsTrue(fullVarLabel.Equals("VarLabel {}"));
        }

        [TestMethod]
        [TestCategory("GetFullVarLabel")]
        public void FullVarNameTest_TimeFrame_NoBrackets()
        {
            SurveyQuestion question = new SurveyQuestion("AA000", "001");
            question.VarName.VarLabel = "VarLabel";
            question.TimeFrames.Add(new QuestionTimeFrame() { TimeFrame = "LSD" });

            string fullVarLabel = question.GetFullVarLabel();

            Assert.IsTrue(fullVarLabel.Equals("VarLabel - {LSD}"));
        }

        [TestMethod]
        [TestCategory("GetFullVarLabel")]
        public void FullVarNameTest_TimeFrame_Brackets()
        {
            SurveyQuestion question = new SurveyQuestion("AA000", "001");
            question.VarName.VarLabel = "VarLabel {}";
            question.TimeFrames.Add(new QuestionTimeFrame() { TimeFrame = "LSD" });

            string fullVarLabel = question.GetFullVarLabel();

            Assert.IsTrue(fullVarLabel.Equals("VarLabel {LSD}"));
        }
        #endregion


    }
}
