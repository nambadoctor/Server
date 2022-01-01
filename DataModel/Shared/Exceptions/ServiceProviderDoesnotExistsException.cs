using System;

[Serializable]
public class ServiceProviderDoesnotExistsException : Exception
{
    public ServiceProviderDoesnotExistsException(string message)
        : base(message)
    {
    }
    public ServiceProviderDoesnotExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}