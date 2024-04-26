using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OireachtasAPI.Services;
using OireachtasAPI.DataLoaders;
using OireachtasAPI.Repositories;
using OireachtasAPI.Settings;

namespace TestOireachtasAPI.ServicesTests
{
    [TestClass]
    public class OireachtasServiceTests
    {
        [TestMethod]
        public void TestSponsor()
        {
            FileDataLoader fileDataLoader = new FileDataLoader();
            OireachtasRepository repository = new OireachtasRepository(fileDataLoader, DataPaths.LocalLegislation, DataPaths.LocalMembers);
            OireachtasService oireachtasService = new OireachtasService(repository);
            List<dynamic> results = oireachtasService.FilterBillsSponsoredBy("IvanaBacik");
            Assert.IsTrue(results.Count >= 2);
        }
   
        [TestMethod]
        public void Testlastupdated()
        {
            List<string> expected = new List<string>(){
                "77", "101", "58", "141", "55", "133", "132", "131",
                "111", "135", "134", "91", "129", "103", "138", "106", "139"
            };
            List<string> received = new List<string>();

            DateTime since = new DateTime(2018, 12, 1);
            DateTime until = new DateTime(2019, 1, 1);

            FileDataLoader fileDataLoader = new FileDataLoader();
            OireachtasRepository repository = new OireachtasRepository(fileDataLoader, DataPaths.LocalLegislation, DataPaths.LocalMembers);
            OireachtasService oireachtasService = new OireachtasService(repository);
            foreach (dynamic bill in oireachtasService.FilterBillsByLastUpdated(since, until))
            {
                received.Add(bill["billNo"].ToString());
            }
            CollectionAssert.AreEquivalent(expected, received);
        }
    }
}
