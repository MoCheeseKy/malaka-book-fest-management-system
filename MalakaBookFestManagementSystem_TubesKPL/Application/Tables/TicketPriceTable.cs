using MalakaBookFest.Core.Enums;
using Microsoft.Extensions.Options;
using MalakaBookFest.Infrastructure.Configuration;

namespace MalakaBookFest.Application.Tables;

public class TicketPriceTable
{
    private readonly Dictionary<TicketType, decimal> _priceTable;

    public TicketPriceTable(IOptions<TicketConfig> options)
    {
        var cfg = options.Value;
        _priceTable = new Dictionary<TicketType, decimal>
        {
            [TicketType.SingleDay] = cfg.Prices.SingleDay,
            [TicketType.AllAccess] = cfg.Prices.AllAccess,
            [TicketType.VIP]       = cfg.Prices.VIP,
        };
    }

    public decimal GetPrice(TicketType type)
    {
        if (!_priceTable.TryGetValue(type, out var price))
            throw new KeyNotFoundException($"No price defined for ticket type: {type}");
        return price;
    }

    public IReadOnlyDictionary<TicketType, decimal> GetAllPrices() =>
        _priceTable.AsReadOnly();
}
