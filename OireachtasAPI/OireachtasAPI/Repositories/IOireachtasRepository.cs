namespace OireachtasAPI.Repositories
{
    /// <summary>
    /// Repository for accessing Oireachtas data.
    /// </summary>
    public interface IOireachtasRepository
    {
        /// <summary>
        /// Retrieves legislative data from a data source.
        /// </summary>
        dynamic GetLegislationData();

        /// <summary>
        /// Retrieves data about members from a data source.
        /// </summary>
        dynamic GetMembersData();
    }
}
