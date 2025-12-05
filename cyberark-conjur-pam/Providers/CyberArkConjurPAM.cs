using Conjur;
using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Keyfactor.Extensions.Pam.CyberArk.Conjur;

public class CyberArkConjurPAM: IPAMProvider
{
    public string Name => "CyberArk-Conjur";

    private readonly ILogger _logger;

    public CyberArkConjurPAM()
    {
        _logger = LogHandler.GetClassLogger<CyberArkConjurPAM>();
    }
    
    
    public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
    {
        var parameters = GetCyberArkConjurParameters(instanceParameters, initializationInfo);

        var client = new Client(parameters.ConjurUrl, parameters.AccountId);

        return GetVariableFromConjur(client, parameters)
            .GetAwaiter()
            .GetResult();
    }

    private CyberArkConjurParameters GetCyberArkConjurParameters(Dictionary<string, string> instanceParameters,
        Dictionary<string, string> initializationInfo)
    {
        try
        {
            string conjurUrl = GetRequiredValue(initializationInfo, "ConjurUrl");
            string accountId = GetRequiredValue(initializationInfo, "AccountId");
            string login = GetRequiredValue(initializationInfo, "Login");
            string apiKey = GetRequiredValue(initializationInfo, "ApiKey");
            
            string variablePath = GetRequiredValue(instanceParameters, "VariablePath");

            var parameters = new CyberArkConjurParameters()
            {
                ConjurUrl = conjurUrl,
                AccountId = accountId,
                Login = login,
                ApiKey = apiKey,

                VariablePath = variablePath
            };
            
            _logger.LogDebug($"Conjur PAM properties received: {JsonConvert.SerializeObject(parameters)}");

            return parameters;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred trtying to access PAM parameters");
            _logger.LogError(ex.Message);
            throw;
        }
    }

    private async Task<string> GetVariableFromConjur(Client client, CyberArkConjurParameters parameters)
    {
        await client.LogInAsync(parameters.Login, parameters.ApiKey);
        
        var secretVariable = client.Variable(parameters.VariablePath);
        var secretValue = await secretVariable.GetValueAsync();
        
        return secretValue ?? "";
    }
    
    protected string GetRequiredValue(Dictionary<string, string> dict, string key)
    {
        if (!dict.ContainsKey(key)
            || string.IsNullOrWhiteSpace(dict[key]))
        {
            string error = $"Required field {key} was missing a value or was not defined as expected in dictionary.";
            throw new ArgumentException(error);
        }
        return dict[key];
    }
}

internal class CyberArkConjurParameters
{
    public string ConjurUrl { get; set; }
    public string AccountId { get; set; }
    public string Login { get; set; }
    public string VariablePath { get; set; }
    
    [JsonProperty("api_key")]
    public string ApiKeyMasked => "*****";
    
    // Mask this field from being serialized in logs or output
    [JsonIgnore]
    public string ApiKey { get; set; }
}
