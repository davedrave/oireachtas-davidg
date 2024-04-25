using OireachtasAPI.DataLoaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OireachtasAPI.Factories
{
    /// <summary>
    /// Factory class for creating instances of IDataLoader.
    /// </summary>
    public class DataLoaderFactory
    {
        /// <summary>
        /// Gets a data loader based on the specified source.
        /// </summary>
        /// <param name="source">The source URL or file path for the data loader.</param>
        /// <returns>An <see cref="IDataLoader"/> that can load data from the specified source.</returns>
        /// <exception cref="ArgumentException">Thrown if the source is null or empty.</exception>
        public static IDataLoader GetLoader(string source)
        {      
            switch (source)
            {
                case string s when s.StartsWith("http", StringComparison.OrdinalIgnoreCase):
                    return new HttpDataLoader();

                case string s when s.EndsWith(".json", StringComparison.OrdinalIgnoreCase):
                    return new FileDataLoader();
                case null:
                    throw new ArgumentNullException(nameof(source), "Source cannot be null.");
                // Additional cases could be easily added here
                default:
                    throw new ArgumentException("Unsupported source type.", nameof(source));
            }
        }
    }
}
