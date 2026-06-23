using NpgsqlTypes;

namespace MalakaBookFest.Core.Enums;

public enum TalkshowStatus
{
    [PgName("Scheduled")] Scheduled,
    [PgName("Ongoing")] Ongoing,
    [PgName("Completed")] Completed,
    [PgName("Cancelled")] Cancelled
}