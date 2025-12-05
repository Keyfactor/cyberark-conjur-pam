using Conjur;
using cyberark_conjur_pam.Models;
using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Keyfactor.Extensions.Pam.CyberArk.Conjur;

public class CyberArkConjurPAM: IPAMProvider
{
    public string Name => "CyberArk-Conjur";

    private readonly ILogger _logger = LogHandler.GetClassLogger<CyberArkConjurPAM>();
    
    public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
    {
        var parameters = CyberArkConjurParameters.Build(_logger, instanceParameters, initializationInfo);

        var client = new Client(parameters.ConjurUrl, parameters.AccountId);

        return GetVariableFromConjur(client, parameters)
            .GetAwaiter()
            .GetResult();
    }

    private async Task<string> GetVariableFromConjur(Client client, CyberArkConjurParameters parameters)
    {
        try
        {
            _logger.LogDebug($"Authenticating login {parameters.Login} to Conjur...");
            
            await client.LogInAsync(parameters.Login, parameters.ApiKey);
            
            _logger.LogDebug($"Successfully authenticated login {parameters.Login} to Conjur.");
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during Conjur authentication: " + ex.Message);
            throw;
        }
        
        _logger.LogDebug($"Retrieving variable {parameters.VariablePath} from Conjur...");

        var secretVariable = client.Variable(parameters.VariablePath);
        var secretValue = await secretVariable.GetValueAsync();
        
        _logger.LogDebug($"Successfully retrieved variable {parameters.VariablePath} from Conjur.");
        
        return secretValue ?? "";
    }
    
    
}