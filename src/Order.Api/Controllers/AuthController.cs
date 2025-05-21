using Order.Api.Extensions;
using Order.Aplicacao.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Order.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ILogger<AuthController> _logger;
    private readonly JwtSettings _jwtSettings;

    public AuthController(
        SignInManager<IdentityUser> signInManager,
        UserManager<IdentityUser> userManager,
        ILogger<AuthController> logger,
        IOptions<JwtSettings> jwtSettings)
    {
        _signInManager = signInManager;
        _userManager = userManager;
        _logger = logger;
        _jwtSettings = jwtSettings.Value;
    }

    [HttpPost("nova-conta")]
    public async Task<ActionResult> Registrar(RegisterUserRequest registerUser)
    {
        if (!ModelState.IsValid)
            return BadRequest();

        var user = new IdentityUser
        {
            UserName = registerUser.Email,
            Email = registerUser.Email,
            EmailConfirmed = true
        };

        var result = await _userManager.CreateAsync(user, registerUser.Password);
        if (result.Succeeded)
        {
            await _signInManager.SignInAsync(user, false);
            return Ok(await GerarJwt(user.Email));
        }

        // Retorna os erros detalhados do Identity
        return BadRequest(new { Errors = result.Errors.Select(e => e.Description) });
    }

    [HttpPost("login")]
    public async Task<ActionResult> Login(LoginUserRequest loginUser)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var result = await _signInManager.PasswordSignInAsync(loginUser.Email, loginUser.Password, false, true);

        if (result.Succeeded)
        {
            _logger.LogInformation("Usuario " + loginUser.Email + " logado com sucesso");
            return Ok(await GerarJwt(loginUser.Email));
        }
        if (result.IsLockedOut)
        {
            return BadRequest("Conta bloqueada. Tente novamente mais tarde.");
        }

        return Unauthorized("Usuário ou senha inválido.");
    }

    private async Task<LoginResponse> GerarJwt(string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        var claims = await _userManager.GetClaimsAsync(user);
        var userRoles = await _userManager.GetRolesAsync(user);

        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
        claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
        claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, ToUnixEpochDate(DateTime.UtcNow).ToString()));
        claims.Add(new Claim(JwtRegisteredClaimNames.Iat, ToUnixEpochDate(DateTime.UtcNow).ToString(), ClaimValueTypes.Integer64));
        foreach (var userRole in userRoles)
        {
            claims.Add(new Claim("role", userRole));
        }

        var identityClaims = new ClaimsIdentity();
        identityClaims.AddClaims(claims);

        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.ASCII.GetBytes(_jwtSettings.Secret);
        var token = tokenHandler.CreateToken(new SecurityTokenDescriptor
        {
            Issuer = _jwtSettings.Emissor,
            Audience = _jwtSettings.ValidoEm,
            Subject = identityClaims,
            Expires = DateTime.UtcNow.AddHours(_jwtSettings.ExpiracaoHoras),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        });

        var encodedToken = tokenHandler.WriteToken(token);

        var response = new LoginResponse
        {
            AccessToken = encodedToken,
            ExpiresIn = TimeSpan.FromHours(_jwtSettings.ExpiracaoHoras).TotalSeconds,
            UserToken = new UserTokenRequest
            {
                Id = user.Id,
                Email = user.Email,
                Claims = claims.Select(c => new ClaimRequest { Type = c.Type, Value = c.Value })
            }
        };

        return response;
    }

    private static long ToUnixEpochDate(DateTime date)
        => (long)Math.Round((date.ToUniversalTime() - new DateTimeOffset(1970, 1, 1, 0, 0, 0, TimeSpan.Zero)).TotalSeconds);
}
