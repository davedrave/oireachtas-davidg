using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using OireachtasAPI.DataLoaders;
using OireachtasAPI.Factories;
using OireachtasAPI.Repositories;
using OireachtasAPI.Services;
using OireachtasAPI.Settings;
using Serilog;
using System;

namespace TestOireachtasAPI.FactoryTests
{
    /// <summary>
    /// Tests for the DataLoaderFactory class.
    /// </summary>
    [TestClass]
    public class DataLoaderFactoryTests
    {
        private ILogger logger;

        [TestInitialize]
        public void Initialize()
        {
            Mock<ILogger> mock = new Mock<ILogger>();
            this.logger = mock.Object;
        }

        /// <summary>
        /// Verifies that when provided with an HTTP URL, the DataLoaderFactory correctly returns an instance of <see cref="HttpDataLoader"/>.
        /// </summary>
        [TestMethod]
        public void GetLoader_WithHttpUrl_ReturnsHttpDataLoader()
        {
            // Arrange
            string url = "https://api.somewebsite.ie/v1/members?limit=50";

            // Act
            var loader = new DataLoaderFactory(this.logger).GetLoader(url);

            // Assert
            Assert.IsInstanceOfType(loader, typeof(HttpDataLoader));
        }

        /// <summary>
        /// Verifies that when provided with a file path, the DataLoaderFactory returns an instance of <see cref="FileDataLoader"/>.
        /// </summary>
        [TestMethod]
        public void GetLoader_WithFilePath_ReturnsFileDataLoader()
        {
            // Arrange
            string url = "https://api.somewebsite.ie/v1/members?limit=50";
            string filePath = "path/to/local/file.json";


            // Act
            var loader = new DataLoaderFactory(this.logger).GetLoader(filePath);

            // Assert
            Assert.IsInstanceOfType(loader, typeof(FileDataLoader));
        }

        /// <summary>
        /// Checks whether an ArgumentNullException is thrown when a null argument is passed to the GetLoader method of DataLoaderFactory.
        /// </summary>
        [TestMethod]
        public void GetLoader_WithNull_ThrowsArgumentException()
        {
            //Arrange
            string url = "https://api.somewebsite.ie/v1/members?limit=50";
            DataLoaderFactory factory = new DataLoaderFactory(this.logger);

            //Act & Assert
            Assert.ThrowsException<ArgumentNullException>(() => factory.GetLoader(null));
        }

        /// <summary>
        /// Checks whether an ArgumentException is thrown when an empty argument is passed to the GetLoader method of DataLoaderFactory.
        /// </summary>
        [TestMethod]
        public void GetLoader_WithEmptyString_ThrowsArgumentException()
        {
            //Arrange
            string url = "https://api.somewebsite.ie/v1/members?limit=50";
            DataLoaderFactory factory = new DataLoaderFactory(this.logger);

            //Act & Assert
            Assert.ThrowsException<ArgumentException>(() => factory.GetLoader(string.Empty));
        }
    }
}
