namespace cyberark_conjur_pam.Exceptions;

public class ConjurLoginException(Exception exception)
    : Exception($"Failed to login to Conjur: {exception.Message}", exception);
