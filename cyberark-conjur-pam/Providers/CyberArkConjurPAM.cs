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
        _logger.MethodEntry();
        
        var parameters = CyberArkConjurParameters.Build(_logger, instanceParameters, initializationInfo);

        _conjurClient ??= new ConjurClient(parameters.ConjurUrl, parameters.AccountId);

        var variable = GetVariableFromConjur(_conjurClient, parameters)
            .GetAwaiter()
            .GetResult();

        _logger.MethodExit();

        return variable;
    }

    /// <summary>
    /// Authenticates and retrieves the desired variable from Conjur
    /// </summary>
    /// <param name="client"></param>
    /// <param name="parameters"></param>
    /// <returns>The secret value for the Conjur variable</returns>
    /// <exception cref="ConjurLoginException">Thrown when authentication with Conjur fails</exception>
    /// <exception cref="ConjurResourceAccessException">Thrown when the login identity does not have access to the Conjur variable</exception>
    private async Task<string> GetVariableFromConjur(IConjurClient client, CyberArkConjurParameters parameters)
    {
        _logger.MethodEntry();
        
        try
        {
            _logger.LogTrace($"Authenticating login {parameters.Login} to Conjur...");
            
            await client.LogInAsync(parameters.Login, parameters.ApiKey);
            
            _logger.LogDebug($"Successfully authenticated login {parameters.Login} to Conjur.");
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during Conjur authentication: " + ex.Message);
            throw new ConjurLoginException(ex);
        }
        
        _logger.LogTrace($"Retrieving variable {parameters.VariablePath} from Conjur...");

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
        
        _logger.LogDebug($"Successfully retrieved variable {parameters.VariablePath} from Conjur.");
        _logger.MethodExit();
        
        return secretValue ?? "";
    }
    
    
}