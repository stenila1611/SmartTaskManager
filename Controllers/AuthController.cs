using System;
using Microsoft.AspNetCore.Mvc;
using SmartTaskManager.Services;
using SmartTaskManager.DTOs;
namespace SmartTaskManager.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController:ControllerBase
{
    private readonly AuthService _auth;
    public AuthController(AuthService auth)=> _auth = auth;

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto dto)
    {
        var token = await _auth.RegisterAsync(dto.Username, dto.Email, dto.Password);
        if(token == null) return Conflict(new {error = "Email already registered."});
        return Ok(new {token});
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto dto)
    {
        var token = await _auth.LoginAsync(dto.Email, dto.Password);
        if(token == null) return Unauthorized(new {error = "Invalid credentials."});
        return Ok(new {token});
    }

}
