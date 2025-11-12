namespace DigitalWallet.Transaction.Domain.Exceptions;

public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }

    public DomainException(string message, Exception innerException)
        : base(message, innerException) { }
}

public class TransactionDomainException : DomainException
{
    public TransactionDomainException(string message) : base(message) { }

    public TransactionDomainException(string message, Exception innerException)
        : base(message, innerException) { }
}
