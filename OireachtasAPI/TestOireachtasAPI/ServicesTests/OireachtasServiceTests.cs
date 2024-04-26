using System;
using System.IO;
using System.Collections.Generic;
using Newtonsoft.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OireachtasAPI.Services;
using OireachtasAPI.DataLoaders;
using OireachtasAPI.Repositories;
using OireachtasAPI.Settings;
using Moq;
using Serilog;

namespace TestOireachtasAPI.ServicesTests
{
    [TestClass]
    public class OireachtasServiceTests
    {
        private OireachtasService oireachtasService;

        [TestInitialize]
        public void Initialize()
        {
            Mock<ILogger> mock = new Mock<ILogger>();
            ILogger logger = mock.Object;

            FileDataLoader fileDataLoader = new FileDataLoader(logger);
            OireachtasRepository repository = new OireachtasRepository(fileDataLoader, logger, DataPaths.LocalLegislation, DataPaths.LocalMembers);
            this.oireachtasService = new OireachtasService(repository, logger);
        }

            [TestMethod]
        public void FilterBillsSponsoredBy_ExistingMember_DataReturned()
        {
            List<dynamic> results = this.oireachtasService.FilterBillsSponsoredBy("IvanaBacik");
            Assert.IsTrue(results.Count >= 2);
        }
   
        [TestMethod]
        public void FilterBillsByLastUpdated_ValidRange_DataReturned()
        {
            List<string> expected = new List<string>(){
                "77", "101", "58", "141", "55", "133", "132", "131",
                "111", "135", "134", "91", "129", "103", "138", "106", "139"
            };
            List<string> received = new List<string>();

            DateTime since = new DateTime(2018, 12, 1);
            DateTime until = new DateTime(2019, 1, 1);

            foreach (dynamic bill in this.oireachtasService.FilterBillsByLastUpdated(since, until))
            {
                received.Add(bill["billNo"].ToString());
            }
            CollectionAssert.AreEquivalent(expected, received);
        }
    }
}
