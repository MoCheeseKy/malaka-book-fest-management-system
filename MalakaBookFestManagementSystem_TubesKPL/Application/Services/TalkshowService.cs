using MalakaBookFest.Application.StateMachines;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Repositories;
using MalakaBookFest.Core.Interfaces.Services;
using MalakaBookFest.Infrastructure.Configuration;
using Microsoft.Extensions.Options;

namespace MalakaBookFest.Application.Services;

public class TalkshowService : ITalkshowService
{
    private readonly ITalkshowRepository _talkshowRepository;
    private readonly ITalkshowRegistrationRepository _registrationRepository;
    private readonly IUserRepository _userRepository;
    private readonly TalkshowConfig _config;

    public TalkshowService(
        ITalkshowRepository talkshowRepository,
        ITalkshowRegistrationRepository registrationRepository,
        IUserRepository userRepository,
        IOptions<TalkshowConfig> options)
    {
        _talkshowRepository     = talkshowRepository;
        _registrationRepository = registrationRepository;
        _userRepository         = userRepository;
        _config                 = options.Value;
    }

    public async Task<IEnumerable<Talkshow>> GetAllTalkshowsAsync() =>
        await _talkshowRepository.GetAllAsync();

    public async Task<Talkshow> GetTalkshowByIdAsync(Guid talkshowId) =>
        await _talkshowRepository.GetByIdAsync(talkshowId)
            ?? throw new KeyNotFoundException($"Talkshow {talkshowId} not found.");

    public async Task<Talkshow> CreateTalkshowAsync(Talkshow talkshow)
    {
        talkshow.StartTime = talkshow.StartTime.ToUniversalTime();
        talkshow.EndTime = talkshow.EndTime.ToUniversalTime();

        if (talkshow.EndTime <= talkshow.StartTime)
            throw new ArgumentException("End time must be after start time.");

        if (talkshow.MaxCapacity <= 0)
            talkshow.MaxCapacity = _config.DefaultMaxCapacity;

        talkshow.TalkshowId = Guid.NewGuid();
        talkshow.CreatedAt  = DateTime.UtcNow;
        talkshow.UpdatedAt  = DateTime.UtcNow;

        return await _talkshowRepository.CreateAsync(talkshow);
    }

    public async Task<Talkshow> UpdateTalkshowAsync(Guid talkshowId, Talkshow updated)
    {
        var talkshow = await _talkshowRepository.GetByIdAsync(talkshowId)
            ?? throw new KeyNotFoundException($"Talkshow {talkshowId} not found.");

        var fsm = new TalkshowStateMachine(talkshow.Status);

        if (!fsm.CanTransition(updated.Status) && talkshow.Status != updated.Status)
            throw new InvalidOperationException(
                $"Cannot update talkshow: invalid status transition {talkshow.Status} → {updated.Status}.");

        talkshow.Title       = updated.Title;
        talkshow.SpeakerName = updated.SpeakerName;
        talkshow.SpeakerBio  = updated.SpeakerBio;
        talkshow.Venue       = updated.Venue;
        talkshow.StartTime   = updated.StartTime.ToUniversalTime();
        talkshow.EndTime     = updated.EndTime.ToUniversalTime();
        talkshow.MaxCapacity = updated.MaxCapacity;
        talkshow.Status      = updated.Status;
        talkshow.UpdatedAt   = DateTime.UtcNow;

        return await _talkshowRepository.UpdateAsync(talkshow);
    }

    public async Task<TalkshowRegistration> RegisterAttendeeAsync(Guid userId, Guid talkshowId)
    {
        var talkshow = await _talkshowRepository.GetByIdAsync(talkshowId)
            ?? throw new KeyNotFoundException($"Talkshow {talkshowId} not found.");

        if (talkshow.Status != Core.Enums.TalkshowStatus.Scheduled)
            throw new InvalidOperationException("Registration is only open for scheduled talkshows.");

        var registrationOpen = DateTime.UtcNow >=
            talkshow.StartTime.AddDays(-_config.RegistrationOpenDaysBeforeEvent);

        if (!registrationOpen)
            throw new InvalidOperationException(
                $"Registration opens {_config.RegistrationOpenDaysBeforeEvent} days before the event.");

        if (await _registrationRepository.IsAlreadyRegisteredAsync(userId, talkshowId))
            throw new InvalidOperationException("You are already registered for this talkshow.");

        var registeredCount = await _talkshowRepository.GetRegistrationCountAsync(talkshowId);
        if (registeredCount >= talkshow.MaxCapacity)
            throw new InvalidOperationException("This talkshow is fully booked.");

        var seatNumber   = registeredCount + 1;
        var registration = new TalkshowRegistration
        {
            RegistrationId = Guid.NewGuid(),
            UserId         = userId,
            TalkshowId     = talkshowId,
            RegisteredAt   = DateTime.UtcNow,
            SeatCode       = $"SEAT-{seatNumber:D4}",
        };

        return await _registrationRepository.CreateAsync(registration);
    }

    public async Task AdvanceTalkshowStatusAsync(Guid talkshowId)
    {
        var talkshow = await _talkshowRepository.GetByIdAsync(talkshowId)
            ?? throw new KeyNotFoundException($"Talkshow {talkshowId} not found.");

        var fsm = new TalkshowStateMachine(talkshow.Status);

        var nextStatus = talkshow.Status switch
        {
            Core.Enums.TalkshowStatus.Scheduled => Core.Enums.TalkshowStatus.Ongoing,
            Core.Enums.TalkshowStatus.Ongoing   => Core.Enums.TalkshowStatus.Completed,
            _ => throw new InvalidOperationException(
                     $"Talkshow cannot be advanced from status: {talkshow.Status}.")
        };

        fsm.Transition(nextStatus);
        talkshow.Status    = fsm.CurrentStatus;
        talkshow.UpdatedAt = DateTime.UtcNow;

        await _talkshowRepository.UpdateAsync(talkshow);
    }
}
