namespace FleetManagementSystem_.Net.Extensions
{
    public static class ConfigurationExtensions
{
    public static string GetConnectionString(this IConfiguration configuration)
    {
        IConfigurationSection section = configuration.GetSection("AppSettings");
        if (section == null)
        {
            throw new KeyNotFoundException("No section with name AppSettings found in config");
        }

        string text = section["DatabaseConnectionName"] ?? "Default";
        return configuration.GetConnectionString(text) ?? throw new KeyNotFoundException("No connection string with name " + text + " found in config");
    }
}
}
