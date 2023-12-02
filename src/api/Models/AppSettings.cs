namespace Api.Models;

public class AppSettings
{
    public readonly string StateStoreName; // Dapr state store
    public readonly string StateStoreQueryIndexName; // Dapr state store

    public AppSettings()
    {
        this.StateStoreName = GetEnvironmentVariable("STATE_STORE_NAME");
        this.StateStoreQueryIndexName = GetEnvironmentVariable("STATE_STORE_QUERY_INDEX_NAME");
    }

    public string GetEnvironmentVariable(string name, bool mandatory = true)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (mandatory && string.IsNullOrEmpty(value))
        {
            throw new Exception($"Environment variable {name} is not set.");
        }
        return string.IsNullOrEmpty(value) ? string.Empty : value;
    }
}
