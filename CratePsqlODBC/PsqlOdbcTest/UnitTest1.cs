using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace PsqlOdbcTest
{
    [TestClass]
    public class SqlActionTest : SqlTransportExecutionTest
    {
        private static readonly Setup Setup = new Setup(Executor);

        public TestContext TestContext { get; set; }

        [ClassCleanup]
        public static void CleanUp()
        {
            Setup.CleanLocations();
        }

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            Setup.SetupLocations();
        }

        [TestMethod]
        public void TestRowsStatement()
        {
            Execute("select * from odbc.locations");
            Assert.AreEqual(13L, Response.RowCount);
        }
    }
}
