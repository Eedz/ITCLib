using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ITCSurveyReportLib;

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
    }
}
