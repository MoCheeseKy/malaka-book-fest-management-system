using MalakaBookFest.Core.Entities;

namespace MalakaBookFest.Core.Interfaces.Repositories;

public interface ITalkshowRepository : IRepository<Talkshow>
{
    Task<IEnumerable<Talkshow>> GetUpcomingAsync();
    Task<int> GetRegistrationCountAsync(Guid talkshowId);
}
