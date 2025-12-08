using cyberark_conjur_pam.Exceptions;
using Keyfactor.Logging;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace cyberark_conjur_pam.Models;

internal class CyberArkConjurParameters
{
    private CyberArkConjurParameters()
    {
    }
    
    public required string ConjurUrl { get; set; }
    public required string AccountId { get; set; }
    public required string Login { get; set; }
    public required string VariablePath { get; set; }
    
    [JsonProperty("api_key")]
    public string ApiKeyMasked => "*****";
    
    /// <summary>
    /// API Key or Password can be used to authenticate the login.
    /// </summary>
    [JsonIgnore]
    public required string ApiKey { get; set; }

    internal static CyberArkConjurParameters Build(ILogger logger, Dictionary<string, string> instanceParameters,
        Dictionary<string, string> initializationInfo)
    {
        logger.MethodEntry();

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

            logger.LogDebug($"Conjur PAM properties received: {JsonConvert.SerializeObject(parameters)}");

            return parameters;
        }
        catch (Exception ex)
        {
            logger.LogError($"An error occurred trying to retrieve PAM parameters: {ex.Message}");
            throw;
        }
        finally
        {
            logger.MethodExit();
        }
    }
    
    private static string GetRequiredValue(Dictionary<string, string> dict, string key)
    {
        if (!dict.ContainsKey(key)
            || string.IsNullOrWhiteSpace(dict[key]))
        {
            throw new ConjurPamArgumentException(key);
        }
        return dict[key];
    }
}