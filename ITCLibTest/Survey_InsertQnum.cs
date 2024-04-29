using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITCLib;
using System.Text.RegularExpressions;
namespace ITCLibTest
{
    /// <summary>
    /// Summary description for Survey_InsertQnum
    /// </summary>
    [TestClass]
    public class Survey_InsertQnum
    {
        Survey s;
        SurveyQuestion s1, s2;
        public Survey_InsertQnum()
        {
            s = new Survey("TS1");

            s1 = new SurveyQuestion("AA000", "001");
            
            s2 = new SurveyQuestion("AA001", "002");
            s2.PrePW = new Wording(0, WordingType.PreP, "Ask if AA000=1.");
            s.AddQuestion(s1);
            s.AddQuestion(s2);

            
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
        [TestCategory("Survey"), TestCategory("InsertQnums")]
        public void InsertQnums()
        {
            

            s.InsertQnums(Enumeration.Qnum);
            Assert.IsTrue(s2.PrePW.WordingText.Equals("Ask if 001/AA000=1."));
        }
    }
}
