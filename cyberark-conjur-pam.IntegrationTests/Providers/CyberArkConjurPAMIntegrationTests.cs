using System.Runtime.CompilerServices;

namespace cyberark_conjur_pam.IntegrationTests.Providers;

[Trait("Category", "Integration")]
public class CyberArkConjurPAMIntegrationTests
{
    [Fact]
    public void GetPassword_ParametersAreValid_ReturnsPassword()
    {
        // Arrange
        var pamProvider = new Keyfactor.Extensions.Pam.CyberArk.Conjur.CyberArkConjurPAM();

        var instanceParameters = new Dictionary<string, string>
        {
            { "VariablePath", EnvironmentVariables.VariablePath }
        };

        var initializationInfo = new Dictionary<string, string>
        {
            { "ConjurUrl", EnvironmentVariables.ConjurUrl },
            { "AccountId", EnvironmentVariables.AccountId },
            { "Login", EnvironmentVariables.Login },
            { "ApiKey", GetApiKeyOrLoginPassword() }
        };

        // Act
        var password = pamProvider.GetPassword(instanceParameters, initializationInfo);

        // Assert
        Assert.Equal(EnvironmentVariables.ExpectedSecretValue, password);
    }

    private string GetApiKeyOrLoginPassword()
    {
        // Conjur API key takes precedence over password
        // Either API key or password can be used to authenticate
        
        if (!string.IsNullOrWhiteSpace(EnvironmentVariables.ApiKey))
        {
            return EnvironmentVariables.ApiKey;
        }

        Console.WriteLine("CONJUR_API_KEY environment variable not set. Using CONJUR_PASSWORD as API key.");
        
        return EnvironmentVariables.Password ?? throw new ArgumentException("CONJUR_PASSWORD environment variable is not set.");
    }

    [ModuleInitializer]
    public static void Init()
    {
        var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env.test");
        if (File.Exists(envFile))
        {
            foreach (var line in File.ReadAllLines(envFile))
            {
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith("#"))
                    continue;

                var parts = line.Split('=', 2);
                if (parts.Length == 2)
                {
                    Environment.SetEnvironmentVariable(parts[0].Trim(), parts[1].Trim());
                }
            }
        }
        else
        {
            Console.WriteLine(".env.test not found. Going to try to run the tests with existing environment variables.");
        }
    }
}

