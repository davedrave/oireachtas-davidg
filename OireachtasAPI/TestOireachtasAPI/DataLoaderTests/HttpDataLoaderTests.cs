﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Newtonsoft.Json;
using OireachtasAPI.DataLoaders;
using OireachtasAPI.Settings;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestOireachtasAPI.DataLoaderTests
{
    /// <summary>
    /// Test class for <see cref="HttpDataLoader"/>.
    /// </summary>
    [TestClass]
    public class HttpDataLoaderTests
    {
        /// <summary>
        /// Representation of what the data Loaded is expected to be like.
        /// </summary>
        private dynamic expected;

        /// <summary>
        /// Initializes test data.
        /// </summary>
        [TestInitialize]
        public void SetUp()
        {
            using (StreamReader r = new StreamReader(DataPaths.LocalMembers))
            {
                string json = r.ReadToEnd();
                expected = JsonConvert.DeserializeObject(json);
            }
        }

        /// <summary>
        /// Tests if valid data is loaded from the API.
        /// </summary>
        [TestMethod]
        public void Load_ValidFile_LoadsValidData()
        {
            //Arrange
            Mock<ILogger> mock = new Mock<ILogger>();
            ILogger logger = mock.Object;

            HttpDataLoader dataLoader = new HttpDataLoader(logger);

            //Act
            //TODO need to revise validation criteria here as a count comparison isnt a true test of validity
            dynamic loaded = dataLoader.Load(DataPaths.ApiMembers);
            
            //Assert
            Assert.AreEqual(loaded["results"].Count, expected["results"].Count);
        }
    }
}
