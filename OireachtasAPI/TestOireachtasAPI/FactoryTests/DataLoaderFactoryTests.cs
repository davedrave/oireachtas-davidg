using Microsoft.VisualStudio.TestTools.UnitTesting;
using OireachtasAPI.DataLoaders;
using OireachtasAPI.Factories;
using System;

namespace TestOireachtasAPI.FactoryTests
{
    /// <summary>
    /// Tests for the DataLoaderFactory class.
    /// </summary>
    [TestClass]
    public class DataLoaderFactoryTests
    {
        /// <summary>
        /// Verifies that when provided with an HTTP URL, the DataLoaderFactory correctly returns an instance of <see cref="HttpDataLoader"/>.
        /// </summary>
        [TestMethod]
        public void GetLoader_WithHttpUrl_ReturnsHttpDataLoader()
        {
            // Arrange
            string url = "https://api.somewebsite.ie/v1/members?limit=50";

            // Act
            var loader = DataLoaderFactory.GetLoader(url);

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
            string filePath = "path/to/local/file.json";

            // Act
            var loader = DataLoaderFactory.GetLoader(filePath);

            // Assert
            Assert.IsInstanceOfType(loader, typeof(FileDataLoader));
        }

        /// <summary>
        /// Checks whether an ArgumentNullException is thrown when a null argument is passed to the GetLoader method of DataLoaderFactory.
        /// </summary>
        [TestMethod]
        public void GetLoader_WithNull_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentNullException>(() => DataLoaderFactory.GetLoader(null));
        }

        /// <summary>
        /// Checks whether an ArgumentException is thrown when an empty argument is passed to the GetLoader method of DataLoaderFactory.
        /// </summary>
        [TestMethod]
        public void GetLoader_WithEmptyString_ThrowsArgumentException()
        {
            Assert.ThrowsException<ArgumentException>(() => DataLoaderFactory.GetLoader(string.Empty));
        }
    }
}
