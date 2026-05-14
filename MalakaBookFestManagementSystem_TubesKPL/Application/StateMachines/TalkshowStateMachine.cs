using MalakaBookFest.Core.Enums;

namespace MalakaBookFest.Application.StateMachines;

/// <summary>
/// Finite state machine governing valid Talkshow lifecycle transitions.
/// Ensures talkshow status only moves forward through its defined phases.
/// </summary>
public class TalkshowStateMachine
{
    private static readonly Dictionary<TalkshowStatus, HashSet<TalkshowStatus>> _allowedTransitions = new()
    {
        [TalkshowStatus.Scheduled]  = [TalkshowStatus.Ongoing, TalkshowStatus.Cancelled],
        [TalkshowStatus.Ongoing]    = [TalkshowStatus.Completed, TalkshowStatus.Cancelled],
        [TalkshowStatus.Completed]  = [],
        [TalkshowStatus.Cancelled]  = [],
    };

    public TalkshowStatus CurrentStatus { get; private set; }

    public TalkshowStateMachine(TalkshowStatus initialStatus)
    {
        CurrentStatus = initialStatus;
    }

    public void Transition(TalkshowStatus targetStatus)
    {
        if (!_allowedTransitions.TryGetValue(CurrentStatus, out var allowed) ||
            !allowed.Contains(targetStatus))
        {
            throw new InvalidOperationException(
                $"Invalid talkshow transition: {CurrentStatus} → {targetStatus}");
        }
        CurrentStatus = targetStatus;
    }

    public bool CanTransition(TalkshowStatus targetStatus) =>
        _allowedTransitions.TryGetValue(CurrentStatus, out var allowed) &&
        allowed.Contains(targetStatus);

    public IReadOnlySet<TalkshowStatus> GetAllowedTransitions() =>
        _allowedTransitions.TryGetValue(CurrentStatus, out var allowed)
            ? allowed
            : new HashSet<TalkshowStatus>();
}
