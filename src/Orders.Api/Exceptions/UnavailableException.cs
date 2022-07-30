namespace Orders.Api.Exceptions;

public class UnavailableException : Exception
{
    public UnavailableException(string message) : base(message)
    {
    }
}