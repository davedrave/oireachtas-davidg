using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestOireachtasAPI
{
    [TestClass]
    public class FilterBillsSponsoredByTest
    {
        [TestMethod]
        public void TestSponsor()
        {
            List<dynamic> results = OireachtasAPI.Program.filterBillsSponsoredBy("IvanaBacik");
            Assert.IsTrue(results.Count>=2);
        }
    }

    [TestClass]
    public class FilterBillsByLastUpdatedTest
    {
        [TestMethod]
        public void Testlastupdated()
        {
            List<string> expected = new List<string>(){
                "77", "101", "58", "141", "55", "94", "133", "132", "131",
                "111", "135", "134", "91", "129", "103", "138", "106", "139"
            };
            List<string> received = new List<string>();

            DateTime since = new DateTime(2018, 12, 1);
            DateTime until = new DateTime(2019, 1, 1);

            foreach (dynamic bill in OireachtasAPI.Program.filterBillsByLastUpdated(since, until))
            {
                received.Add(bill["billNo"]);
            }
            CollectionAssert.AreEqual(expected, received);
        }
    }
}
