using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Application.Tables;

/// <summary>
/// Table-driven access control. Defines which actions each role is allowed
/// to perform. Adding a new permission requires only a table update here,
/// not changes scattered across multiple controllers.
/// </summary>
public static class RolePermissionTable
{
    public enum Permission
    {
        ViewBooths,
        ManageOwnBooth,
        ManageAllBooths,
        PurchaseTicket,
        ViewAllTickets,
        ScanTicket,
        RegisterTalkshow,
        ManageTalkshows,
        ViewAuditLogs,
        ManageUsers,
    }

    private static readonly Dictionary<UserRole, HashSet<Permission>> _table = new()
    {
        [UserRole.Guest] =
        [
            Permission.ViewBooths,
        ],
        [UserRole.Attendee] =
        [
            Permission.ViewBooths,
            Permission.PurchaseTicket,
            Permission.RegisterTalkshow,
        ],
        [UserRole.Organizer] =
        [
            Permission.ViewBooths,
            Permission.ManageOwnBooth,
            Permission.PurchaseTicket,
            Permission.RegisterTalkshow,
        ],
        [UserRole.Admin] =
        [
            Permission.ViewBooths,
            Permission.ManageOwnBooth,
            Permission.ManageAllBooths,
            Permission.PurchaseTicket,
            Permission.ViewAllTickets,
            Permission.ScanTicket,
            Permission.RegisterTalkshow,
            Permission.ManageTalkshows,
            Permission.ViewAuditLogs,
            Permission.ManageUsers,
        ],
    };

    public static bool HasPermission(UserRole role, Permission permission) =>
        _table.TryGetValue(role, out var permissions) && permissions.Contains(permission);

    public static IReadOnlySet<Permission> GetPermissions(UserRole role) =>
        _table.TryGetValue(role, out var permissions)
            ? permissions
            : new HashSet<Permission>();
}
