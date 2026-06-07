using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Application.StateMachines;

/// <summary>
/// Finite state machine governing valid Ticket lifecycle transitions.
/// Any attempt to perform an illegal transition throws an InvalidOperationException.
/// </summary>
public class TicketStateMachine
{
    private static readonly Dictionary<TicketStatus, HashSet<TicketStatus>> _allowedTransitions = new()
    {
        [TicketStatus.Active]    = [TicketStatus.Used, TicketStatus.Cancelled, TicketStatus.Expired],
        [TicketStatus.Used]      = [],
        [TicketStatus.Cancelled] = [],
        [TicketStatus.Expired]   = [],
    };

    public TicketStatus CurrentStatus { get; private set; }

    public TicketStateMachine(TicketStatus initialStatus)
    {
        CurrentStatus = initialStatus;
    }

    public void Transition(TicketStatus targetStatus)
    {
        if (!_allowedTransitions.TryGetValue(CurrentStatus, out var allowed) ||
            !allowed.Contains(targetStatus))
        {
            throw new InvalidOperationException(
                $"Invalid ticket transition: {CurrentStatus} → {targetStatus}");
        }
        CurrentStatus = targetStatus;
    }

    public bool CanTransition(TicketStatus targetStatus) =>
        _allowedTransitions.TryGetValue(CurrentStatus, out var allowed) &&
        allowed.Contains(targetStatus);

    public IReadOnlySet<TicketStatus> GetAllowedTransitions() =>
        _allowedTransitions.TryGetValue(CurrentStatus, out var allowed)
            ? allowed
            : new HashSet<TicketStatus>();
}
