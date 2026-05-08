namespace MalakaBookFest.Core.Interfaces.Services;

public interface IAuditLogService
{
    Task LogAsync(Guid? userId, string action, string? entityType = null,
        string? entityId = null, object? oldValue = null,
        object? newValue = null, string? ipAddress = null);
}
