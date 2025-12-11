namespace cyberark_conjur_pam.IntegrationTests;

internal static class EnvironmentVariables
{
    public static string ConjurUrl => Environment.GetEnvironmentVariable("CONJUR_URL") ?? throw new ArgumentException("CONJUR_URL environment variable is not set.");
    public static string AccountId => Environment.GetEnvironmentVariable("CONJUR_ACCOUNT_ID") ?? throw new ArgumentException("CONJUR_ACCOUNT_ID environment variable is not set.");
    public static string Login => Environment.GetEnvironmentVariable("CONJUR_LOGIN") ?? throw new ArgumentException("CONJUR_LOGIN environment variable is not set.");
    public static string VariablePath => Environment.GetEnvironmentVariable("CONJUR_VARIABLE_PATH") ?? throw new ArgumentException("CONJUR_VARIABLE_PATH environment variable is not set.");
    
    public static string ExpectedSecretValue => Environment.GetEnvironmentVariable("CONJUR_EXPECTED_SECRET_VALUE") ?? throw new ArgumentException("CONJUR_EXPECTED_SECRET_VALUE environment variable is not set.");
    public static string? ApiKey => Environment.GetEnvironmentVariable("CONJUR_API_KEY");
    public static string? Password => Environment.GetEnvironmentVariable("CONJUR_PASSWORD");
}
