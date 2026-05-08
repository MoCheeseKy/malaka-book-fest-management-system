using MalakaBookFest.Core.Entities;

namespace MalakaBookFest.Core.Interfaces.Repositories;

public interface ITalkshowRegistrationRepository : IRepository<TalkshowRegistration>
{
    Task<IEnumerable<TalkshowRegistration>> GetByUserIdAsync(Guid userId);
    Task<bool> IsAlreadyRegisteredAsync(Guid userId, Guid talkshowId);
}
