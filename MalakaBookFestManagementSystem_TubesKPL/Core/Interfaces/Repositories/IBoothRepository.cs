using MalakaBookFest.Core.Entities;

namespace MalakaBookFest.Core.Interfaces.Repositories;

public interface IBoothRepository : IRepository<Booth>
{
    Task<IEnumerable<Booth>> GetByOrganizerIdAsync(Guid organizerId);
    Task<bool> BoothNumberExistsAsync(string boothNumber);
}
