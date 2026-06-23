using System;
using System.Collections.Generic;

namespace MalakaBookFestManagement_GUI.Services
{
    public class ApiResponse<T>
    {
        public bool Success { get; set; }
        public T? Data { get; set; }
        public string? Message { get; set; }
        public List<string>? Errors { get; set; }

        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message, List<string>? errors = null) =>
            new() { Success = false, Message = message, Errors = errors };
    }

    public class UserDto
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class LoginRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequestDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }

    public class BoothDto
    {
        public Guid BoothId { get; set; }
        public Guid OrganizerId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string BoothName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BoothNumber { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }

    public class CreateBoothDto
    {
        public string BoothName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string BoothNumber { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
    }

    public class UpdateBoothDto
    {
        public string? BoothName { get; set; }
        public string? Description { get; set; }
        public string? Category { get; set; }
        public bool? IsActive { get; set; }
    }

    public class BookDto
    {
        public Guid BookId { get; set; }
        public Guid BoothId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CoverUrl { get; set; } = string.Empty;
    }

    public class CreateBookDto
    {
        public string Title { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public string Isbn { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public string CoverUrl { get; set; } = string.Empty;
    }

    public class UpdateBookDto
    {
        public string? Title { get; set; }
        public string? Author { get; set; }
        public string? Isbn { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
        public string? CoverUrl { get; set; }
    }

    public class TalkshowDto
    {
        public Guid TalkshowId { get; set; }
        public string Title { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string SpeakerBio { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCapacity { get; set; }
        public int RegisteredCount { get; set; }
        public string Status { get; set; } = string.Empty;
    }

    public class CreateTalkshowDto
    {
        public string Title { get; set; } = string.Empty;
        public string SpeakerName { get; set; } = string.Empty;
        public string SpeakerBio { get; set; } = string.Empty;
        public string Venue { get; set; } = string.Empty;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public int MaxCapacity { get; set; }
    }

    public class UpdateTalkshowDto
    {
        public string? Title { get; set; }
        public string? SpeakerName { get; set; }
        public string? SpeakerBio { get; set; }
        public string? Venue { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? MaxCapacity { get; set; }
    }

    public class TicketDto
    {
        public Guid TicketId { get; set; }
        public Guid UserId { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string QrCode { get; set; } = string.Empty;
        public decimal PricePaid { get; set; }
        public DateTime PurchasedAt { get; set; }
        public DateTime ValidDate { get; set; }
    }

    public class PurchaseTicketDto
    {
        public string Type { get; set; } = string.Empty;
        public DateTime ValidDate { get; set; }
    }

    public class UpdateUserRoleDto
    {
        public string Role { get; set; } = string.Empty;
    }

    public class UpdateUserStatusDto
    {
        public bool IsActive { get; set; }
    }
}
