using cyberark_conjur_pam.Clients;
using cyberark_conjur_pam.Exceptions;
using Keyfactor.Extensions.Pam.CyberArk.Conjur;
using Moq;
using Xunit.Abstractions;

namespace cyberark_conjur_pam.UnitTests.Providers;

public class CyberArkConjurPAMTests
{
    private readonly Mock<IConjurClient> _mockConjurClient = new();
    private readonly CyberArkConjurPAM _sut;
    
    public CyberArkConjurPAMTests(ITestOutputHelper output)
    {
        var logger = new XunitLogger("", output);
        _sut = new CyberArkConjurPAM(_mockConjurClient.Object, logger);
    }
    
    [Fact]
    public void GetPassword_PasswordRetrievedSuccessfully_ReturnsPassword()
    {
        // Arrange
        var instanceParameters = GetInstanceParameters();
        var initializationInfo = GetInitializationInfo();
        var expected = "foobar";
        
        SetupLogInSuccess(initializationInfo);
        SetupGetVariableValueSuccess(instanceParameters, expected);
        
        // Act
        var result = _sut.GetPassword(instanceParameters, initializationInfo);
        
        // Assert
        Assert.Equal(expected, result);
    }
    
    [Theory]
    [InlineData("ConjurUrl")]
    [InlineData("AccountId")]
    [InlineData("Login")]
    [InlineData("ApiKey")]
    public void GetPassword_InitializationInfoMissingRequiredField_ThrowsError(string fieldToRemove)
    {
        // Arrange
        var instanceParameters = GetInstanceParameters();
        var initializationInfo = GetInitializationInfo();
        
        Assert.True(initializationInfo.Remove(fieldToRemove));
        
        // Act
        var exception = Assert.Throws<ConjurPamArgumentException>(() => _sut.GetPassword(instanceParameters, initializationInfo));
        
        // Assert
        Assert.Equal($"Required field {fieldToRemove} is missing a value or was not defined as expected in dictionary.", exception.Message);
    }
    
    [Theory]
    [InlineData("VariablePath")]
    public void GetPassword_InstanceParametersMissingRequiredField_ThrowsError(string fieldToRemove)
    {
        // Arrange
        var instanceParameters = GetInstanceParameters();
        var initializationInfo = GetInitializationInfo();
        
        Assert.True(instanceParameters.Remove(fieldToRemove));
        
        // Act
        var exception = Assert.Throws<ConjurPamArgumentException>(() => _sut.GetPassword(instanceParameters, initializationInfo));
        
        // Assert
        Assert.Equal($"Required field {fieldToRemove} is missing a value or was not defined as expected in dictionary.", exception.Message);
    }

    [Fact]
    public void GetPassword_LogInFailure_ThrowsException()
    {
        // Arrange
        var instanceParameters = GetInstanceParameters();
        var initializationInfo = GetInitializationInfo();
        
        SetupLogInFailure(initializationInfo, "whoops");
        
        // Act
        var exception = Assert.Throws<ConjurLoginException>(() => _sut.GetPassword(instanceParameters, initializationInfo));
        
        // Assert
        Assert.Equal("Failed to login to Conjur: whoops", exception.Message);
    }
    
    [Fact]
    public void GetPassword_GetVariableFailure_ThrowsException()
    {
        // Arrange
        var instanceParameters = GetInstanceParameters();
        var initializationInfo = GetInitializationInfo();
        
        SetupLogInSuccess(initializationInfo);
        SetupGetVariableValueFailure(instanceParameters, "whoops");
        
        // Act
        var exception = Assert.Throws<ConjurResourceAccessException>(() => _sut.GetPassword(instanceParameters, initializationInfo));
        
        // Assert
        Assert.Equal("Failed to access resource foo in Conjur: whoops", exception.Message);
    }
    
    private Dictionary<string, string> GetInitializationInfo() => new Dictionary<string, string>
    {
        { "ConjurUrl", "conjur-url" },
        { "AccountId", "account-id" },
        { "Login", "login" },
        { "ApiKey", "api-key" }
    };
    
    private Dictionary<string, string> GetInstanceParameters() => new Dictionary<string, string>
    {
        { "VariablePath", "foo" }
    };
    
    private void SetupLogInFailure(Dictionary<string, string> initializationInfo, string exceptionMessage)
    {
        _mockConjurClient
            .Setup(p => p.LogInAsync(initializationInfo["Login"], initializationInfo["ApiKey"]))
            .ThrowsAsync(new Exception(exceptionMessage));
    }
    
    private void SetupLogInSuccess(Dictionary<string, string> initializationInfo)
    {
        _mockConjurClient
            .Setup(p => p.LogInAsync(initializationInfo["Login"], initializationInfo["ApiKey"]))
            .Returns(Task.FromResult(true));
    }
    
    private void SetupGetVariableValueFailure(Dictionary<string, string> instanceParameters, string exceptionMessage)
    {
        _mockConjurClient
            .Setup(p => p.GetVariableValueAsync(instanceParameters["VariablePath"]))
            .ThrowsAsync(new Exception(exceptionMessage));
    }
    
    private void SetupGetVariableValueSuccess(Dictionary<string, string> instanceParameters, string returnValue)
    {
        _mockConjurClient
            .Setup(p => p.GetVariableValueAsync(instanceParameters["VariablePath"]))
            .ReturnsAsync(returnValue);
    }
}
