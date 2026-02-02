namespace Domain.Exceptions.AuthExceptions;

public class AuthException : DomainException
{
    public AuthException(string message) : base(message) {}
}