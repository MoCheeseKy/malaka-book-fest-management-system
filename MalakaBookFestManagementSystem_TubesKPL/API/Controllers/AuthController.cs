using MalakaBookFest.Application.Common;
using MalakaBookFest.Application.DTOs.Auth;
using MalakaBookFest.Core.Interfaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace MalakaBookFest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Register(
        [FromBody] RegisterRequestDto dto)
    {
        var user = await _authService.RegisterAsync(dto.Email, dto.Password, dto.FullName);
        var (token, _) = await _authService.LoginAsync(dto.Email, dto.Password);

        var response = new LoginResponseDto
        {
            Token    = token,
            UserId   = user.UserId,
            FullName = user.FullName,
            Email    = user.Email,
            Role     = user.Role,
        };

        return Ok(ApiResponse<LoginResponseDto>.Ok(response, "Registration successful."));
    }

    [HttpPost("login")]
    public async Task<ActionResult<ApiResponse<LoginResponseDto>>> Login(
        [FromBody] LoginRequestDto dto)
    {
        var (token, user) = await _authService.LoginAsync(dto.Email, dto.Password);

        var response = new LoginResponseDto
        {
            Token    = token,
            UserId   = user.UserId,
            FullName = user.FullName,
            Email    = user.Email,
            Role     = user.Role,
        };

        return Ok(ApiResponse<LoginResponseDto>.Ok(response, "Login successful."));
    }
}
