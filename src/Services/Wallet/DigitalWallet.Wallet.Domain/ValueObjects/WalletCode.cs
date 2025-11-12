namespace DigitalWallet.Wallet.Domain.ValueObjects;

public sealed class WalletCode : IEquatable<WalletCode>
{
    public string Value { get; }

    private WalletCode(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("Wallet code cannot be empty", nameof(value));

        if (value.Length < 8 || value.Length > 20)
            throw new ArgumentException("Wallet code must be between 8 and 20 characters", nameof(value));

        Value = value.ToUpper();
    }

    public static WalletCode Create(string value)
    {
        return new WalletCode(value);
    }

    public static WalletCode Generate()
    {
        var guid = Guid.NewGuid().ToString("N");
        var code = guid.Substring(0, 10).ToUpper();
        return new WalletCode(code);
    }

    public bool Equals(WalletCode? other)
    {
        if (other is null) return false;
        return Value == other.Value;
    }

    public override bool Equals(object? obj)
    {
        return obj is WalletCode other && Equals(other);
    }

    public override int GetHashCode()
    {
        return Value.GetHashCode();
    }

    public static bool operator ==(WalletCode? left, WalletCode? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(WalletCode? left, WalletCode? right)
    {
        return !Equals(left, right);
    }

    public override string ToString()
    {
        return Value;
    }

    public static implicit operator string(WalletCode code) => code.Value;
}
