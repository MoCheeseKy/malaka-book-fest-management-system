using System;
using System.Collections.Generic;

namespace GUI.Models
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class Booth
    {
        public Guid BoothId { get; set; }
        public Guid OrganizerId { get; set; }
        public string BoothName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string BoothNumber { get; set; } = string.Empty;
        public BoothCategory Category { get; set; } = BoothCategory.Other;
        public bool IsActive { get; set; } = true;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Book
    {
        public Guid BookId { get; set; }
        public Guid BoothId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? CoverUrl { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Talkshow
    {
        public Guid TalkshowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string? SpeakerBio { get; set; }
        public string Venue { get; set; } = string.Empty;
        public DateTime StartTime { get; set; } = DateTime.Now;
        public DateTime EndTime { get; set; } = DateTime.Now.AddHours(1);
        public int MaxCapacity { get; set; }
        public TalkshowStatus Status { get; set; } = TalkshowStatus.Scheduled;
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }

    public class Ticket
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public TicketType Type { get; set; } = TicketType.SingleDay;
        public TicketStatus Status { get; set; } = TicketStatus.Active;
        public string? QrCode { get; set; }
        public decimal PricePaid { get; set; }
        public DateTime PurchasedAt { get; set; } = DateTime.Now;
        public DateTime ValidDate { get; set; } = DateTime.Now;
    }

    // Additional models for requests
    public class BookCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? CoverUrl { get; set; }
    }

    public class BoothCreateRequest
    {
        public string BoothName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string BoothNumber { get; set; } = string.Empty;
        public BoothCategory Category { get; set; } = BoothCategory.Other;
    }

    public class TalkshowCreateRequest
    {
        public string Title { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string? SpeakerBio { get; set; }
        public string Venue { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCapacity { get; set; }
    }

    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public IEnumerable<string>? Errors { get; set; }
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public UserRole Role { get; set; }
    }
}
