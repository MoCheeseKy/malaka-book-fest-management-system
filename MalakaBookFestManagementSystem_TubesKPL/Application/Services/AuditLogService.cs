using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Repositories;
using MalakaBookFest.Core.Interfaces.Services;
using System.Text.Json;
namespace MalakaBookFest.Application.Services;

public class AuditLogService : IAuditLogService
{
    private readonly IRepository<AuditLog> _auditLogRepository;
    public AuditLogService(IRepository<AuditLog> auditLogRepository)
    {
        _auditLogRepository = auditLogRepository;
    }
    public async Task LogAsync(Guid? userId, string action, string? entityType = null,
        string? entityId = null, object? oldValue = null,
        object? newValue = null, string? ipAddress = null)
    {
        var log = new AuditLog
        {
            LogId = Guid.NewGuid(),
            UserId = userId,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValue = oldValue is null ? null : JsonSerializer.Serialize(oldValue),
            NewValue = newValue is null ? null : JsonSerializer.Serialize(newValue),
            IpAddress = ipAddress,
            OccurredAt = DateTime.UtcNow,
        };
        await _auditLogRepository.CreateAsync(log);
    }
}