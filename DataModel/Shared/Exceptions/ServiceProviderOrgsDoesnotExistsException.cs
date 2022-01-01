using System;

[Serializable]
public class ServiceProviderOrgsDoesnotExistsException : Exception
{
    public ServiceProviderOrgsDoesnotExistsException(string message)
        : base(message)
    {
    }
    public ServiceProviderOrgsDoesnotExistsException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}