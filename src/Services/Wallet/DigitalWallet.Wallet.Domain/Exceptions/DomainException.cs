namespace DigitalWallet.Wallet.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message)
    {
    }

    public DomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}

public class WalletDomainException : DomainException
{
    public WalletDomainException(string message) : base(message)
    {
    }

    public WalletDomainException(string message, Exception innerException) 
        : base(message, innerException)
    {
    }
}
