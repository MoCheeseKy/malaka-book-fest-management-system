using MalakaBookFest.Application.Common;
using MalakaBookFest.Core.Entities;
using MalakaBookFest.Core.Interfaces.Repositories;
using MalakaBookFest.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MalakaBookFest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpGet]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserDto>>>> GetAll()
    {
        var users = await _userRepository.GetAllAsync();
        var dtos = users.Select(u => new UserDto
        {
            UserId = u.UserId,
            Email = u.Email,
            FullName = u.FullName,
            Role = u.Role.ToString(),
            IsActive = u.IsActive,
            CreatedAt = u.CreatedAt
        });
        return Ok(ApiResponse<IEnumerable<UserDto>>.Ok(dtos));
    }

    [HttpPut("{id:guid}/role")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateRole(Guid id, [FromBody] UpdateUserRoleDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse<object>.Fail("User not found."));
        }

        if (Enum.TryParse<UserRole>(dto.Role, true, out var role))
        {
            user.Role = role;
            user.UpdatedAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user);

            var response = new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role.ToString(),
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
            return Ok(ApiResponse<UserDto>.Ok(response, "User role updated successfully."));
        }

        return BadRequest(ApiResponse<object>.Fail("Invalid role name."));
    }

    [HttpPut("{id:guid}/status")]
    public async Task<ActionResult<ApiResponse<UserDto>>> UpdateStatus(Guid id, [FromBody] UpdateUserStatusDto dto)
    {
        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
        {
            return NotFound(ApiResponse<object>.Fail("User not found."));
        }

        user.IsActive = dto.IsActive;
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user);

        var response = new UserDto
        {
            UserId = user.UserId,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role.ToString(),
            IsActive = user.IsActive,
            CreatedAt = user.CreatedAt
        };
        return Ok(ApiResponse<UserDto>.Ok(response, "User status updated successfully."));
    }
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

public class UpdateUserRoleDto
{
    public string Role { get; set; } = string.Empty;
}

public class UpdateUserStatusDto
{
    public bool IsActive { get; set; }
}
