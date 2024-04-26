using OireachtasAPI.DataLoaders;
using Serilog;
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
        private readonly ILogger logger;

        public DataLoaderFactory(ILogger logger)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Gets a data loader based on the specified source.
        /// </summary>
        /// <param name="source">The source URL or file path for the data loader.</param>
        /// <returns>An <see cref="IDataLoader"/> that can load data from the specified source.</returns>
        /// <exception cref="ArgumentException">Thrown if the source is null or empty.</exception>
        public IDataLoader GetLoader(string source)
        {      
            switch (source)
            {
                case string s when s.StartsWith("http", StringComparison.OrdinalIgnoreCase):
                    logger.Information("http detected. Factory returning HttpDataLoader.");
                    return new HttpDataLoader(logger);

                case string s when s.EndsWith(".json", StringComparison.OrdinalIgnoreCase):
                    logger.Information(".json detected. Factory returning FileDataLoader.");
                    return new FileDataLoader(logger);
                case null:
                    logger.Fatal("Factory failed to create object.");
                    throw new ArgumentNullException(nameof(source), "Source cannot be null.");
                // Additional cases could be easily added here
                default:
                    throw new ArgumentException("Unsupported source type.", nameof(source));
            }
        }
    }
}
