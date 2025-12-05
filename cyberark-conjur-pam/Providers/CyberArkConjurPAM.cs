using Conjur;
using cyberark_conjur_pam.Clients;
using cyberark_conjur_pam.Exceptions;
using cyberark_conjur_pam.Models;
using Keyfactor.Logging;
using Keyfactor.Platform.Extensions;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Keyfactor.Extensions.Pam.CyberArk.Conjur;

public class CyberArkConjurPAM: IPAMProvider
{
    public string Name => "CyberArk-Conjur";

    private readonly ILogger _logger;
    private IConjurClient? _conjurClient;

    public CyberArkConjurPAM()
    {
        _logger = LogHandler.GetClassLogger<CyberArkConjurPAM>();
    }

    // Constructor for dependency injection in unit tests
    public CyberArkConjurPAM(IConjurClient conjurClient, ILogger logger)
    {
        _conjurClient = conjurClient;
        _logger = logger;
    }
    
    public string GetPassword(Dictionary<string, string> instanceParameters, Dictionary<string, string> initializationInfo)
    {
        var parameters = CyberArkConjurParameters.Build(_logger, instanceParameters, initializationInfo);

        _conjurClient ??= new ConjurClient(parameters.ConjurUrl, parameters.AccountId);

        return GetVariableFromConjur(_conjurClient, parameters)
            .GetAwaiter()
            .GetResult();
    }

    private async Task<string> GetVariableFromConjur(IConjurClient client, CyberArkConjurParameters parameters)
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
            throw new ConjurLoginException(ex);
        }
        
        _logger.LogDebug($"Retrieving variable {parameters.VariablePath} from Conjur...");

        string? secretValue;

        try
        {
            secretValue = await client.GetVariableValueAsync(parameters.VariablePath);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred while retrieving the variable from Conjur: " + ex.Message);
            throw new ConjurResourceAccessException(parameters.VariablePath, ex);
        }
        
        return secretValue ?? "";
    }
    
    
}