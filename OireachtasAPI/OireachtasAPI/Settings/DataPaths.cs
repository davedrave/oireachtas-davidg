namespace OireachtasAPI.Settings
{
    /// <summary>
    /// Centralised Paths settings.
    /// </summary>
    public static class DataPaths
    {
        public static readonly string LocalLegislation = "legislation.json";
        public static readonly string LocalMembers = "members.json";

        public static readonly string ApiLegislation = "https://api.oireachtas.ie/v1/legislation";
        public static readonly string ApiMembers = "https://api.oireachtas.ie/v1/members";
    }
}
