namespace cyberark_conjur_pam.Exceptions;

public class ConjurResourceAccessException(string resource, Exception exception)
    : Exception($"Failed to access resource {resource} in Conjur: {exception.Message}", exception);
