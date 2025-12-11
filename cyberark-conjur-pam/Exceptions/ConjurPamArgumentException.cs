namespace cyberark_conjur_pam.Exceptions;

public class ConjurPamArgumentException(string requiredVariable) : Exception(
    $"Required field {requiredVariable} is missing a value or was not defined as expected in dictionary.");
