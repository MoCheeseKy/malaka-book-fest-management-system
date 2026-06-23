using System;
using System.Collections.Generic;

namespace MalakaBookFestManagementSystem_GUI.Models
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
    }

    public class BoothDto
    {
        public Guid BoothId { get; set; }
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string BoothName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string BoothNumber { get; set; } = string.Empty;
        public int Category { get; set; } // Simplified to int for GUI
        public bool IsActive { get; set; }
    }

    public class BookDto
    {
        public Guid BookId { get; set; }
        public Guid BoothId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string? Isbn { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string? CoverUrl { get; set; }
    }

    public class TalkshowDto
    {
        public Guid TalkshowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string? SpeakerBio { get; set; }
        public string Venue { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCapacity { get; set; }
        public int RegisteredCount { get; set; }
        public int Status { get; set; } // Simplified to int for GUI
    }

    public class TicketDto
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public int Type { get; set; }
        public int Status { get; set; }
        public string? QrCode { get; set; }
        public decimal PricePaid { get; set; }
        public DateTime PurchasedAt { get; set; }
        public DateOnly ValidDate { get; set; }
    }
}
