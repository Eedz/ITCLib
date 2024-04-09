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

            Assert.IsNotNull(question.Comments);
            Assert.IsNotNull(question.Translations);
            Assert.IsNotNull(question.PreviousNameList);
            Assert.IsNotNull(question.TimeFrames);
            Assert.IsNotNull(question.Images);
        }
    }
}
