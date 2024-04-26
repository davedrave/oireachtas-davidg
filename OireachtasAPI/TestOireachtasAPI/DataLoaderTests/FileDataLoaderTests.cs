using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using OireachtasAPI.DataLoaders;
using OireachtasAPI.Settings;
using System;
using System.IO;

namespace TestOireachtasAPI.DataLoaderTests
{
    /// <summary>
    /// Test class for <see cref="FileDataLoaderTests"/>.
    /// </summary>
    [TestClass]
    public class FileDataLoaderTests
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
        /// Tests if valid data is loaded from the file.
        /// </summary>
        [TestMethod]
        public void Load_ValidFile_LoadsValidData()
        {
            FileDataLoader fileDataLoader = new FileDataLoader();

            //TODO need to revise validation criteria here as a count comparison isnt a true test of validity
            dynamic loaded = fileDataLoader.Load(DataPaths.LocalMembers);
            Assert.AreEqual(loaded["results"].Count, expected["results"].Count);
        }
    }
}
