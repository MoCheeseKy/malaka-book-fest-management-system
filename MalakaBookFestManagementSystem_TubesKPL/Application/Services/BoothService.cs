using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using MalakaBookFest.Core.Interfaces.Repositories;
using MalakaBookFest.Core.Interfaces.Services;
namespace MalakaBookFest.Application.Services;

public class BoothService : IBoothService
{
    private readonly IBoothRepository _boothRepository;
    private readonly IUserRepository _userRepository;
    public BoothService(IBoothRepository boothRepository, IUserRepository userRepository)
    {
        _boothRepository = boothRepository;
        _userRepository = userRepository;
    }
    public async Task<IEnumerable<Booth>> GetAllBoothsAsync() =>
        await _boothRepository.GetAllAsync();
    public async Task<Booth> GetBoothByIdAsync(Guid boothId) =>
        await _boothRepository.GetByIdAsync(boothId)
            ?? throw new KeyNotFoundException($"Booth {boothId} not found.");
    public async Task<IEnumerable<Booth>> GetBoothsByOrganizerAsync(Guid organizerId) =>
        await _boothRepository.GetByOrganizerIdAsync(organizerId);
    public async Task<Booth> CreateBoothAsync(Booth booth)
    {
        if (await _boothRepository.BoothNumberExistsAsync(booth.BoothNumber))
            throw new InvalidOperationException($"Booth number '{booth.BoothNumber}' is already taken.");
        var organizer = await _userRepository.GetByIdAsync(booth.OrganizerId)
            ?? throw new KeyNotFoundException("Organizer not found.");
        if (organizer.Role != UserRole.Organizer && organizer.Role != UserRole.Admin)
            throw new UnauthorizedAccessException("Only Organizers or Admins can own a booth.");
        booth.BoothId = Guid.NewGuid();
        booth.CreatedAt = DateTime.UtcNow;
        booth.UpdatedAt = DateTime.UtcNow;
        return await _boothRepository.CreateAsync(booth);
    }
    public async Task<Booth> UpdateBoothAsync(Guid boothId, Booth updated, Guid requesterId)
    {
        var booth = await _boothRepository.GetByIdAsync(boothId)
            ?? throw new KeyNotFoundException($"Booth {boothId} not found.");
        var requester = await _userRepository.GetByIdAsync(requesterId)
            ?? throw new KeyNotFoundException("Requester not found.");
        if (requester.Role != UserRole.Admin && booth.OrganizerId != requesterId)
            throw new UnauthorizedAccessException("You do not have permission to update this booth.");
        booth.BoothName = updated.BoothName;
        booth.Description = updated.Description;
        booth.Category = updated.Category;
        booth.IsActive = updated.IsActive;
        booth.UpdatedAt = DateTime.UtcNow;
        return await _boothRepository.UpdateAsync(booth);
    }
    public async Task DeleteBoothAsync(Guid boothId, Guid requesterId)
    {
        var booth = await _boothRepository.GetByIdAsync(boothId)
            ?? throw new KeyNotFoundException($"Booth {boothId} not found.");
        var requester = await _userRepository.GetByIdAsync(requesterId)
            ?? throw new KeyNotFoundException("Requester not found.");
        if (requester.Role != UserRole.Admin && booth.OrganizerId != requesterId)
            throw new UnauthorizedAccessException("You do not have permission to delete this booth.");
        await _boothRepository.DeleteAsync(boothId);
    }
}