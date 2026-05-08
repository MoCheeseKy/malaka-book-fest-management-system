using MalakaBookFest.Core.Entities;

namespace MalakaBookFest.Core.Interfaces.Services;

public interface IBoothService
{
    Task<IEnumerable<Booth>> GetAllBoothsAsync();
    Task<Booth> GetBoothByIdAsync(Guid boothId);
    Task<IEnumerable<Booth>> GetBoothsByOrganizerAsync(Guid organizerId);
    Task<Booth> CreateBoothAsync(Booth booth);
    Task<Booth> UpdateBoothAsync(Guid boothId, Booth updated, Guid requesterId);
    Task DeleteBoothAsync(Guid boothId, Guid requesterId);
}
