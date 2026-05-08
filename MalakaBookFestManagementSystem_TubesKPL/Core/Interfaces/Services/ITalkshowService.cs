using MalakaBookFest.Core.Entities;

namespace MalakaBookFest.Core.Interfaces.Services;

public interface ITalkshowService
{
    Task<IEnumerable<Talkshow>> GetAllTalkshowsAsync();
    Task<Talkshow> GetTalkshowByIdAsync(Guid talkshowId);
    Task<Talkshow> CreateTalkshowAsync(Talkshow talkshow);
    Task<Talkshow> UpdateTalkshowAsync(Guid talkshowId, Talkshow updated);
    Task<TalkshowRegistration> RegisterAttendeeAsync(Guid userId, Guid talkshowId);
    Task AdvanceTalkshowStatusAsync(Guid talkshowId);
}
