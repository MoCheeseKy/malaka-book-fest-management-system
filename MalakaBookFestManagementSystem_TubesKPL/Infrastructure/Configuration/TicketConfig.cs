namespace MalakaBookFest.Infrastructure.Configuration;

/// <summary>
/// Maps the "TicketConfig" section of appsettings.json.
/// Controls pricing and purchase limits without recompilation.
/// </summary>
public class TicketConfig
{
    public TicketPrices Prices { get; set; } = new();
    public int MaxPerUser { get; set; } = 3;
}

public class TicketPrices
{
    public decimal SingleDay { get; set; } = 50_000;
    public decimal AllAccess { get; set; } = 150_000;
    public decimal VIP { get; set; } = 350_000;
}
