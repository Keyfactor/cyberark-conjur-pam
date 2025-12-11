using Conjur;

namespace cyberark_conjur_pam.Clients;

public class ConjurClient : IConjurClient
{
    private readonly Client _client;
    
    public ConjurClient(string applianceUrl, string accountId)
    {
        _client = new Client(applianceUrl, accountId);
    }
    
    public async Task LogInAsync(string login, string password)
    {
        await _client.LogInAsync(login, password);
    }

    public async Task<string?> GetVariableValueAsync(string variablePath)
    {
        var secretVariable = _client.Variable(variablePath);
        var secretValue = await secretVariable.GetValueAsync();
        return secretValue;
    }
}
