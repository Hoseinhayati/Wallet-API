namespace DigitalWallet.Transaction.Domain.ValueObjects;

public sealed class ReferenceCode : IEquatable<ReferenceCode>
{
    public string Value { get; }

    private ReferenceCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Reference code cannot be empty", nameof(value));

        if (value.Length < 10 || value.Length > 50)
            throw new ArgumentException("Reference code must be between 10 and 50 characters", nameof(value));

        Value = value.ToUpper();
    }

    public static ReferenceCode Create(string value)
    {
        return new ReferenceCode(value);
    }

    public static ReferenceCode Generate()
    {
        // Format: TXN-{GUID}-{TIMESTAMP}
        var guid = Guid.NewGuid().ToString("N").Substring(0, 8);
        var timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var code = $"TXN-{guid}-{timestamp}".ToUpper();
        return new ReferenceCode(code);
    }

    public bool Equals(ReferenceCode? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is ReferenceCode other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(ReferenceCode? left, ReferenceCode? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(ReferenceCode? left, ReferenceCode? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(ReferenceCode code) => code.Value;
}
