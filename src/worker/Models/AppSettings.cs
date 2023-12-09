namespace Worker.Models;

public class AppSettings
{
    public readonly string PubSubRequestsName;
    public readonly string PubSubRequestsTopic;
    public readonly string SecretStoreName;
    public readonly string OpenAIDeploymentName;
    public readonly string RequestsApiAppId;
    public readonly string RequestsApiSearchEndPoint;
    public readonly string RequestsApiCreateEndPoint;

    public AppSettings()
    {
        this.PubSubRequestsName = GetEnvironmentVariable("PUBSUB_REQUESTS_NAME");
        this.PubSubRequestsTopic = GetEnvironmentVariable("PUBSUB_REQUESTS_TOPIC");
        this.SecretStoreName = GetEnvironmentVariable("SECRET_STORE_NAME");
        this.OpenAIDeploymentName = GetEnvironmentVariable("OPENAI_API_DEPLOYMENT_NAME");
        this.RequestsApiAppId = GetEnvironmentVariable("REQUESTS_API_APP_ID");
        this.RequestsApiSearchEndPoint = GetEnvironmentVariable("REQUESTS_API_SEARCH_ENDPOINT");
        this.RequestsApiCreateEndPoint = GetEnvironmentVariable("REQUESTS_API_CREATE_ENDPOINT");
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
