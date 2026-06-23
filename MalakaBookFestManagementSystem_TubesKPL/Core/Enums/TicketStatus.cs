using NpgsqlTypes;

namespace MalakaBookFest.Core.Enums;

public enum TicketStatus
{
    [PgName("Active")] Active,
    [PgName("Used")] Used,
    [PgName("Cancelled")] Cancelled,
    [PgName("Expired")] Expired
}