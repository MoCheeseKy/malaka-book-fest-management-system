namespace MalakaBookFest.Infrastructure.Configuration;

/// <summary>
/// Maps the "JwtConfig" section of appsettings.json using the Options pattern.
/// </summary>
public class JwtConfig
{
    public string Secret { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiryMinutes { get; set; } = 60;
}
