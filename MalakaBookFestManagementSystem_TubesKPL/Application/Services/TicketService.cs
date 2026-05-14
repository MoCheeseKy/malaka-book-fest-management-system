using MalakaBookFest.Application.StateMachines;
using MalakaBookFest.Application.Tables;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Enums;
using MalakaBookFest.Core.Interfaces.Repositories;
using MalakaBookFest.Core.Interfaces.Services;
using MalakaBookFest.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace MalakaBookFest.Application.Services;

public class TicketService : ITicketService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly IUserRepository _userRepository;
    private readonly TicketPriceTable _priceTable;
    private readonly TicketConfig _config;

    public TicketService(
        ITicketRepository ticketRepository,
        IUserRepository userRepository,
        TicketPriceTable priceTable,
        IOptions<TicketConfig> options)
    {
        _ticketRepository = ticketRepository;
        _userRepository   = userRepository;
        _priceTable       = priceTable;
        _config           = options.Value;
    }

    public async Task<Ticket> PurchaseTicketAsync(Guid userId, TicketType type, DateOnly validDate)
    {
        var user = await _userRepository.GetByIdAsync(userId)
            ?? throw new KeyNotFoundException("User not found.");

        var activeCount = await _ticketRepository.CountActiveByUserIdAsync(userId);
        if (activeCount >= _config.MaxPerUser)
            throw new InvalidOperationException(
                $"Maximum of {_config.MaxPerUser} active tickets per user reached.");

        if (await _ticketRepository.UserHasTicketTypeAsync(userId, type))
            throw new InvalidOperationException(
                $"You already own a {type} ticket.");

        var price  = _priceTable.GetPrice(type);
        var qrCode = GenerateQrCode(userId, type, validDate);

        var ticket = new Ticket
        {
            TicketId    = Guid.NewGuid(),
            UserId      = userId,
            Type        = type,
            Status      = TicketStatus.Active,
            QrCode      = qrCode,
            PricePaid   = price,
            PurchasedAt = DateTime.UtcNow,
            ValidDate   = validDate,
        };

        return await _ticketRepository.CreateAsync(ticket);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByUserAsync(Guid userId) =>
        await _ticketRepository.GetByUserIdAsync(userId);

    public async Task<Ticket> ScanTicketAsync(string qrCode)
    {
        var ticket = await _ticketRepository.GetByQrCodeAsync(qrCode)
            ?? throw new KeyNotFoundException("Ticket not found for this QR code.");

        var fsm = new TicketStateMachine(ticket.Status);
        fsm.Transition(TicketStatus.Used);

        ticket.Status = fsm.CurrentStatus;
        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task<Ticket> CancelTicketAsync(Guid ticketId, Guid requesterId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId)
            ?? throw new KeyNotFoundException($"Ticket {ticketId} not found.");

        if (ticket.UserId != requesterId)
        {
            var requester = await _userRepository.GetByIdAsync(requesterId)
                ?? throw new KeyNotFoundException("Requester not found.");

            if (requester.Role != UserRole.Admin)
                throw new UnauthorizedAccessException("You can only cancel your own tickets.");
        }

        var fsm = new TicketStateMachine(ticket.Status);
        fsm.Transition(TicketStatus.Cancelled);

        ticket.Status = fsm.CurrentStatus;
        return await _ticketRepository.UpdateAsync(ticket);
    }

    public async Task ExpireTicketsAsync()
    {
        var allTickets = await _ticketRepository.GetAllAsync();
        var today      = DateOnly.FromDateTime(DateTime.UtcNow);

        foreach (var ticket in allTickets.Where(t => t.Status == TicketStatus.Active))
        {
            if (ticket.ValidDate < today)
            {
                var fsm = new TicketStateMachine(ticket.Status);
                fsm.Transition(TicketStatus.Expired);
                ticket.Status = fsm.CurrentStatus;
                await _ticketRepository.UpdateAsync(ticket);
            }
        }
    }

    private static string GenerateQrCode(Guid userId, TicketType type, DateOnly validDate)
    {
        var raw  = $"{userId}-{type}-{validDate}-{Guid.NewGuid()}";
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(raw));
        return Convert.ToHexString(hash)[..32];
    }
}
