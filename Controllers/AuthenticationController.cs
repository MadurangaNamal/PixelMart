using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PixelMart.API.DbContexts;
using PixelMart.API.Entities;
using PixelMart.API.Helpers;
using PixelMart.API.Models.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace PixelMart.API.Controllers;


[Route("api/auth")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly PixelMartDbContext _context;
    private readonly IConfiguration _configuration;
    private readonly TokenValidationParameters _tokenValidationParameters;
    private readonly RequestLogHelper _requestLogHelper;

    public AuthenticationController(UserManager<ApplicationUser> userManager,
           RoleManager<IdentityRole> roleManager,
           PixelMartDbContext context,
           IConfiguration configuration,
           TokenValidationParameters tokenValidationParameters,
           RequestLogHelper requestLogHelper)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
        _configuration = configuration;
        _tokenValidationParameters = tokenValidationParameters;
        _requestLogHelper = requestLogHelper;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        _requestLogHelper.LogInfo("POST /api/auth/register CALLED TO REGISTER NEW USER");

        if (!ModelState.IsValid)
        {
            return BadRequest("Please, provide all the required fields");
        }

        var userExists = await _userManager.FindByEmailAsync(registerDto.EmailAddress);
        if (userExists != null)
        {
            return BadRequest($"User {registerDto.EmailAddress} already exists");
        }

        ApplicationUser newUser = new()
        {
            FirstName = registerDto.FirstName,
            LastName = registerDto.LastName,
            Email = registerDto.EmailAddress,
            UserName = registerDto.UserName,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        var result = await _userManager.CreateAsync(newUser, registerDto.Password);

        if (result.Succeeded)
        {
            // Add user role

            switch (registerDto.Role)
            {
                case UserRoles.Admin:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Admin);
                    break;
                case UserRoles.User:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.User);
                    break;
                case UserRoles.Client:
                    await _userManager.AddToRoleAsync(newUser, UserRoles.Client);
                    break;
                default:
                    break;
            }

            _requestLogHelper.LogInfo($"NEW {registerDto.Role} USER CREATED SUCCESSFULLY");
            return Ok("User created");
        }

        _requestLogHelper.LogError(null!, "USER CREATION FAILED");
        return BadRequest("User could not be created");
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto)
    {
        _requestLogHelper.LogInfo("POST /api/auth/login CALLED TO LOGIN USER");

        if (!ModelState.IsValid)
        {
            return BadRequest("Please, provide all required fields");
        }

        var userExists = await _userManager.FindByEmailAsync(loginDto.EmailAddress);
        if (userExists != null && await _userManager.CheckPasswordAsync(userExists, loginDto.Password))
        {
            var tokenValue = await GenerateJWTTokenAsync(userExists, null!);
            return Ok(tokenValue);
        }

        _requestLogHelper.LogError(null!, "UNAUTHORIZED LOGIN ATTEMPT");
        return Unauthorized();
    }

    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenRequestDto tokenRequestDto)
    {
        _requestLogHelper.LogInfo("POST /api/auth/refresh-token CALLED TO REFRESH TOKEN");

        if (!ModelState.IsValid)
        {
            return BadRequest("Please, provide all required fields");
        }

        var result = await VerifyAndGenerateTokenAsync(tokenRequestDto);
        return Ok(result);
    }

    private async Task<AuthResultDto> VerifyAndGenerateTokenAsync(TokenRequestDto tokenRequestDto)
    {
        var jwtTokenHandler = new JwtSecurityTokenHandler();
        var storedToken = await _context.RefreshTokens.FirstOrDefaultAsync(x => x.Token == tokenRequestDto.RefreshToken);
        var dbUser = await _userManager.FindByIdAsync(storedToken!.UserId) ?? throw new UnauthorizedAccessException("User not found");

        try
        {
            var tokenCheckResult = jwtTokenHandler.ValidateToken(tokenRequestDto.Token, _tokenValidationParameters, out var validatedToken);

            return await GenerateJWTTokenAsync(dbUser, storedToken);
        }
        catch (SecurityTokenExpiredException)
        {
            if (storedToken.DateExpire >= DateTime.UtcNow)
            {
                return await GenerateJWTTokenAsync(dbUser, storedToken);
            }
            else
            {
                return await GenerateJWTTokenAsync(dbUser, null!);
            }
        }
    }

    private async Task<AuthResultDto> GenerateJWTTokenAsync(ApplicationUser user, RefreshToken rToken)
    {
        var authClaims = new List<Claim>()
            {
                new(ClaimTypes.Name, user.UserName!),
                new(ClaimTypes.NameIdentifier, user.Id),
                new(JwtRegisteredClaimNames.Email, user.Email!),
                new(JwtRegisteredClaimNames.Sub, user.Email!),
                new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

        // Add User Role Claims
        var userRoles = await _userManager.GetRolesAsync(user);
        foreach (var userRole in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(_configuration["JWT_SECRET_KEY"]!));

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:Issuer"],
            audience: _configuration["JWT:Audience"],
            expires: DateTime.UtcNow.AddMinutes(30),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256));

        var jwtToken = new JwtSecurityTokenHandler().WriteToken(token);

        if (rToken != null)
        {
            var rTokenResponse = new AuthResultDto()
            {
                Token = jwtToken,
                RefreshToken = rToken.Token,
                ExpiresAt = token.ValidTo
            };
            return rTokenResponse;
        }

        var refreshToken = new RefreshToken()
        {
            JwtId = token.Id,
            IsRevoked = false,
            UserId = user.Id,
            DateAdded = DateTime.UtcNow,
            DateExpire = DateTime.UtcNow.AddMonths(6),
            Token = Guid.NewGuid().ToString() + "-" + Guid.NewGuid().ToString()
        };
        await _context.RefreshTokens.AddAsync(refreshToken);
        await _context.SaveChangesAsync();

        var response = new AuthResultDto()
        {
            Token = jwtToken,
            RefreshToken = refreshToken.Token,
            ExpiresAt = token.ValidTo
        };

        return response;
    }
}
