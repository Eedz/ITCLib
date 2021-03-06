﻿using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITCLib;

namespace ITCLibTest
{
    /// <summary>
    /// Summary description for ReportSurveyTests
    /// </summary>
    [TestClass]
    public class ReportSurveyTests_RemoveRepeats
    {
        ReportSurvey survey;
        string wording;
        string wording2;

        public ReportSurveyTests_RemoveRepeats()
        {
            survey = new ReportSurvey("TS1");
            wording = "test";
            wording2 = "testing";
    
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
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PrePCleared()
        {
            // Arrange
            survey.AddQuestion(
                new SurveyQuestion ("AA000", "000a")
                    {
                        ID = 1,
                        PreP = wording
                    }
                );

            survey.AddQuestion(new SurveyQuestion ("AA001", "000b") { ID = 2, PreP = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsTrue(survey.QuestionByID(2).PreP == "");
           
        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PrePNotCleared()
        {
            // Arrange
            survey.AddQuestion(
                new SurveyQuestion("AA000", "000a")
                {
                    ID = 1,
                    PreP = wording
                }
                );

            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PreP = wording2 });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PreP == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PreICleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PreI = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PreI = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsTrue(survey.QuestionByID(2).PreI == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PreINotCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PreI = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PreI = wording2 });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PreI == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PreACleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PreA = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PreA = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsTrue(survey.QuestionByID(2).PreA == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PreANotCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PreA = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PreA = wording2 });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PreA == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_LitQCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, LitQ = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, LitQ = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsTrue(survey.QuestionByID(2).LitQ == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_LitQNotCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, LitQ = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, LitQ = wording2 });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).LitQ == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PstICleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PstI = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PstI = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsTrue(survey.QuestionByID(2).PstI == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PstINotCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PstI = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PstI = wording2 });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PstI == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PstPCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PstP = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PstP = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsTrue(survey.QuestionByID(2).PstP == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PstPNotCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PstP = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PstP = wording2 });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PstP == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_RespOptionsCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, RespOptions = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, RespOptions = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsTrue(survey.QuestionByID(2).RespOptions == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_RespOptionsNotCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, RespOptions = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, RespOptions = wording2 });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).RespOptions == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_NRCodesCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, NRCodes = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, NRCodes = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsTrue(survey.QuestionByID(2).NRCodes == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_NRCodesNotCleared()
        {
            // Arrange
            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, NRCodes = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, NRCodes = wording2 });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).NRCodes == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PrePNotCleared_RepeatedField()
        {
            // Arrange
            List<string> repeatedFields = new List<string> { "PreP" };

            survey.RepeatedFields = repeatedFields;

            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PreP = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PreP = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PreP == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PreINotCleared_RepeatedField()
        {
            // Arrange
            List<string> repeatedFields = new List<string> { "PreI" };

            survey.RepeatedFields = repeatedFields;

            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PreI = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PreI = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PreI == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PreANotCleared_RepeatedField()
        {
            // Arrange
            List<string> repeatedFields = new List<string> { "PreA" };

            survey.RepeatedFields = repeatedFields;

            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PreA = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PreA = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PreA == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_LitQNotCleared_RepeatedField()
        {
            // Arrange
            List<string> repeatedFields = new List<string> { "LitQ" };

            survey.RepeatedFields = repeatedFields;

            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, LitQ = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, LitQ = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).LitQ == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PstINotCleared_RepeatedField()
        {
            // Arrange
            List<string> repeatedFields = new List<string> { "PstI" };

            survey.RepeatedFields = repeatedFields;

            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PstI = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PstI = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PstI == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_PstPNotCleared_RepeatedField()
        {
            // Arrange
            List<string> repeatedFields = new List<string> { "PstP" };

            survey.RepeatedFields = repeatedFields;

            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, PstP = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, PstP = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).PstP == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_RespOptionsNotCleared_RepeatedField()
        {
            // Arrange
            List<string> repeatedFields = new List<string> { "RespOptions" };

            survey.RepeatedFields = repeatedFields;

            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, RespOptions = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, RespOptions = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).RespOptions == "");

        }

        [TestMethod]
        [TestCategory("Survey"), TestCategory("RemoveRepeats")]
        public void RemoveRepeats_ID2_NRCodesNotCleared_RepeatedField()
        {
            // Arrange
            List<string> repeatedFields = new List<string> { "NRCodes" };

            survey.RepeatedFields = repeatedFields;

            survey.AddQuestion(new SurveyQuestion("AA000", "000a") { ID = 1, NRCodes = wording });
            survey.AddQuestion(new SurveyQuestion("AA001", "000b") { ID = 2, NRCodes = wording });

            // Act
            survey.RemoveRepeats();

            // Assert
            Assert.IsFalse(survey.QuestionByID(2).NRCodes == "");

        }

    }
}
