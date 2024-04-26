using OireachtasAPI.DataLoaders;
using OireachtasAPI.Settings;
using Serilog;
using System.Runtime.InteropServices.ComTypes;

namespace OireachtasAPI.Repositories
{
    /// <summary>
    /// Implements the <see cref="IOireachtasRepository"/> interface for accessing legislative and members data
    /// </summary>
    public class OireachtasRepository : IOireachtasRepository
    {
        private readonly IDataLoader dataLoader;
        private readonly ILogger logger;
        private readonly string legislationPath;
        private readonly string membersPath;

        /// <summary>
        /// Initializes a new instance of the <see cref="OireachtasRepository"/> with specified data loader and data paths.
        /// The DataLoader and accompanying paths could correspond to a HttpDataLoader with api urls for example.
        /// </summary>
        /// <param name="dataLoader">The data loader to use for data access.</param>
        /// <param name="legislationPath">The file path or URL to the legislation data.</param>
        /// <param name="membersPath">The file path or URL to the members data.</param>
        public OireachtasRepository(IDataLoader dataLoader, ILogger logger, string legislationPath, string membersPath)
        {
            this.dataLoader = dataLoader;
            this.logger = logger;
            this.legislationPath = legislationPath;
            this.membersPath = membersPath;
        }

        /// <inheritdoc />
        public dynamic GetLegislationData()
        {
            logger.Information("Running GetLegislationData()");
            logger.Debug("Legislation Path: {LegislationPath}", legislationPath);
            return this.dataLoader.Load(this.legislationPath);
        }

        /// <inheritdoc />
        public dynamic GetMembersData()
        {
            logger.Information("Running GetMembersData()");
            logger.Debug("Legislation Path: {LegislationPath}", legislationPath);
            logger.Debug("Members Path: {MembersPath}", membersPath);
            return this.dataLoader.Load(this.membersPath);
        }
    }
}
