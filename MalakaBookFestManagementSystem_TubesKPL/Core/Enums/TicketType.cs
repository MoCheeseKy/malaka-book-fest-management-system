using NpgsqlTypes;

namespace MalakaBookFest.Core.Enums;

public enum TicketType
{
    [PgName("SingleDay")] SingleDay,
    [PgName("AllAccess")] AllAccess,
    [PgName("VIP")] VIP
}