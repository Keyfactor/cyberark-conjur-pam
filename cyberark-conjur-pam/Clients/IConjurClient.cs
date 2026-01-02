namespace cyberark_conjur_pam.Clients;

public interface IConjurClient
{
    /// <summary>
    /// Authenticates the login with the provided username and password / api key
    /// </summary>
    /// <param name="login"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public Task LogInAsync(string login, string password);
    
    /// <summary>
    /// Gets the variable value from Conjur for the given variable path
    /// </summary>
    /// <param name="variablePath"></param>
    /// <returns>The secret value from Conjur</returns>
    public Task<string?> GetVariableValueAsync(string variablePath);
}
