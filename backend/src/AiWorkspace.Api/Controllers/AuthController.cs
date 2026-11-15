
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AiWorkspace.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace AiWorkspace.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<IActionResult> Register(
        [FromBody] RegisterRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Email) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new
            {
                message = "Email and password are required."
            });
        }

        var existingUser = await _userManager
            .FindByEmailAsync(request.Email);

        if (existingUser is not null)
        {
            return Conflict(new
            {
                message = "An account with this email already exists."
            });
        }

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            CreatedAtUtc = DateTime.UtcNow
        };

        var result = await _userManager.CreateAsync(
            user,
            request.Password);

        if (!result.Succeeded)
        {
            return BadRequest(new
            {
                message = "User registration failed.",
                errors = result.Errors.Select(x => x.Description)
            });
        }

        return StatusCode(StatusCodes.Status201Created, new
        {
            message = "User registered successfully.",
            userId = user.Id,
            email = user.Email
        });
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login(
        [FromBody] LoginRequest request)
    {
        var user = await _userManager
            .FindByEmailAsync(request.Email ?? string.Empty);

        if (user is null ||
            !await _userManager.CheckPasswordAsync(
                user,
                request.Password ?? string.Empty))
        {
            return Unauthorized(new
            {
                message = "Invalid email or password."
            });
        }

        var signingKey = _configuration["Jwt:SigningKey"];

        if (string.IsNullOrWhiteSpace(signingKey) ||
            Encoding.UTF8.GetByteCount(signingKey) < 32)
        {
            throw new InvalidOperationException(
                "JWT signing key is missing or too short.");
        }

        var issuer = _configuration["Jwt:Issuer"]
            ?? "AiWorkspace.Api";

        var audience = _configuration["Jwt:Audience"]
            ?? "AiWorkspace.Client";

        var expiresAt = DateTime.UtcNow.AddHours(1);

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!)
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(signingKey));

        var credentials = new SigningCredentials(
            key,
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: expiresAt,
            signingCredentials: credentials);

        var accessToken = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return Ok(new
        {
            accessToken,
            tokenType = "Bearer",
            expiresIn = 3600,
            expiresAtUtc = expiresAt
        });
    }

    [HttpGet("me")]
    [Authorize(AuthenticationSchemes = "Bearer")]
    public async Task<IActionResult> Me()
    {
        var userId = User.FindFirstValue(
            ClaimTypes.NameIdentifier);

        if (string.IsNullOrWhiteSpace(userId))
        {
            return Unauthorized(new
            {
                message = "Invalid authentication token."
            });
        }

        var user = await _userManager.FindByIdAsync(userId);

        if (user is null)
        {
            return Unauthorized(new
            {
                message = "User account not found."
            });
        }

        return Ok(new
        {
            id = user.Id,
            email = user.Email,
            userName = user.UserName,
            createdAtUtc = user.CreatedAtUtc
        });
    }

    public sealed class RegisterRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public sealed class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
