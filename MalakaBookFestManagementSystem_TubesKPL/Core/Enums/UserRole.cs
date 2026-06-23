using NpgsqlTypes;

namespace MalakaBookFest.Core.Enums;

public enum UserRole
{
    [PgName("Guest")] Guest,
    [PgName("Attendee")] Attendee,
    [PgName("Organizer")] Organizer,
    [PgName("Admin")] Admin
}