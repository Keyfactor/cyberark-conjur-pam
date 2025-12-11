using System.Runtime.CompilerServices;
using Keyfactor.Extensions.Pam.CyberArk.Conjur.Providers;
using MartinCostello.Logging.XUnit;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace cyberark_conjur_pam.IntegrationTests.Providers;

[Trait("Category", "Integration")]
public class CyberArkConjurPAMIntegrationTests
{
    private ILogger _logger;
    
    public CyberArkConjurPAMIntegrationTests(ITestOutputHelper output)
    {
        var loggerFactory = LoggerFactory.Create(builder =>
            builder.AddProvider(new XUnitLoggerProvider(output, new XUnitLoggerOptions()))
                .SetMinimumLevel(LogLevel.Trace));
        
        _logger = loggerFactory.CreateLogger<CyberArkConjurPAMIntegrationTests>();
    }
    
    [Fact]
    public void GetPassword_ParametersAreValid_ReturnsPassword()
    {
        // Arrange
        var pamProvider = new CyberArkConjurPAM();

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
            _logger.LogDebug("Using API key to authenticate to Conjur.");
            return EnvironmentVariables.ApiKey;
        }

        _logger.LogDebug("CONJUR_API_KEY environment variable not set. Using CONJUR_PASSWORD as API key.");
        
        return EnvironmentVariables.Password ?? throw new ArgumentException("CONJUR_PASSWORD environment variable is not set.");
    }

    [ModuleInitializer]
    public static void Init()
    {
        var envFile = Path.Combine(Directory.GetCurrentDirectory(), ".env.test");
        
        // Load environment variables from .env.test file
        // If .env.test file does not exist, skip loading and rely on existing environment variables
        if (!File.Exists(envFile)) return;
        
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
}

