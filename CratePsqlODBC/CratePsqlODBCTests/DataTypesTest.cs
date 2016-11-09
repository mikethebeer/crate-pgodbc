using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace CratePsqlODBC.Tests
{
    [TestClass()]
    public class DataTypesTest : SqlTransportExecutionTest
    {
        private static readonly Setup Setup = new Setup(Executor);
        private TestContext testContextInstance;

        [ClassCleanup]
        public static void CleanUp()
        {
            Setup.CleanCrateTypes();
        }

        [ClassInitialize]
        public static void Initialize(TestContext testContext)
        {
            Setup.SetupCrateTypes();
        }

        [TestMethod]
        public void TestPrimitiveTypes()
        {
            Execute("select id, ean8, ipv4, number, pin, random_digit, str, unix_time, yesno from doc.crate_types limit 1");
            Assert.AreEqual(1L, Response.RowCount);
            List<Type> types = Response.Types;
            Assert.AreEqual(typeof(int), types[0]);
            Assert.AreEqual(typeof(long), types[1]);
            Assert.AreEqual(typeof(string), types[2]);
            Assert.AreEqual(typeof(float), types[3]);
            Assert.AreEqual(typeof(string), types[4]);
            Assert.AreEqual(typeof(double), types[5]);
            Assert.AreEqual(typeof(string), types[6]);
            Assert.AreEqual(typeof(DateTime), types[7]);
            Assert.AreEqual(typeof(bool), types[8]);
        }
    }
}