namespace Worker.Models;

public class AppSettings
{
    public readonly string PubSubRequestsName;
    public readonly string PubSubRequestsTopic;
    public readonly string SecretStoreName;
    public readonly string OpenAIDeploymentName;


    public AppSettings()
    {
        this.PubSubRequestsName = GetEnvironmentVariable("PUBSUB_REQUESTS_NAME");
        this.PubSubRequestsTopic = GetEnvironmentVariable("PUBSUB_REQUESTS_TOPIC");
        this.SecretStoreName = GetEnvironmentVariable("SECRET_STORE_NAME");
        this.OpenAIDeploymentName = GetEnvironmentVariable("OPENAI_API_DEPLOYMENT_NAME");
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
