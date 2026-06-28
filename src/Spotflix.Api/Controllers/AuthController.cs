using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Spotflix.Api.Authorization;
using Spotflix.Api.Dtos.Auth;
using Spotflix.Api.Models;
using Spotflix.Api.Services;

namespace Spotflix.Api.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ITokenService _tokenService;
    private readonly IEmailSender _emailSender;
    private readonly EmailTemplateRenderer _templateRenderer;
    private readonly ILogger<AuthController> _logger;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        ITokenService tokenService,
        IEmailSender emailSender,
        EmailTemplateRenderer templateRenderer,
        ILogger<AuthController> logger)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
        _emailSender = emailSender;
        _templateRenderer = templateRenderer;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register(RegisterDto dto, CancellationToken ct)
    {
        var user = new ApplicationUser
        {
            UserName = dto.Email,
            Email = dto.Email,
            FullName = dto.FullName,
        };

        var result = await _userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        await _userManager.AddToRoleAsync(user, Roles.User);

        var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
        var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
        var emailBody = _templateRenderer.Render("confirm-email", new()
        {
            ["FullName"] = user.FullName ?? "",
            ["UserId"]   = user.Id.ToString(),
            ["Token"]    = encoded,
        });

        await _emailSender.SendAsync(dto.Email, "Confirme seu e-mail", emailBody, ct);

        return Ok(new { message = "Usuário registrado. Verifique o e-mail para confirmar a conta." });
    }

    [AllowAnonymous]
    [HttpPost("confirm-email")]
    public async Task<IActionResult> ConfirmEmail(ConfirmEmailDto dto)
    {
        var user = await _userManager.FindByIdAsync(dto.UserId);
        if (user is null)
            return BadRequest(new { message = "Solicitação inválida." });

        var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
        var result = await _userManager.ConfirmEmailAsync(user, token);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(new { message = "E-mail confirmado com sucesso." });
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<TokenResponseDto>> Login(LoginDto dto, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
        {
            _logger.LogWarning("Login attempt with non-existent email: {Email}", dto.Email);
            return Unauthorized(new { message = "Credenciais inválidas." });
        }

        var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, lockoutOnFailure: true);
        if (result.IsLockedOut)
        {
            _logger.LogWarning("Login attempt with locked account: {UserId}", user.Id);
            return Unauthorized(new { message = "Conta bloqueada temporariamente. Tente mais tarde." });
        }
        if (result.IsNotAllowed)
        {
            _logger.LogWarning("Login attempt with unconfirmed email: {UserId}", user.Id);
            return Unauthorized(new { message = "É necessário confirmar o e-mail antes de entrar." });
        }
        if (!result.Succeeded)
        {
            _logger.LogWarning("Invalid password for user: {UserId}", user.Id);
            return Unauthorized(new { message = "Credenciais inválidas." });
        }

        var tokens = await _tokenService.CreateTokensAsync(user, ct);
        _logger.LogInformation("User logged in successfully: {UserId}", user.Id);
        return Ok(tokens);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<TokenResponseDto>> Refresh(RefreshTokenDto dto, CancellationToken ct)
    {
        var tokens = await _tokenService.RotateRefreshTokenAsync(dto.RefreshToken, ct);
        if (tokens is null)
            return Unauthorized(new { message = "Refresh token inválido ou expirado." });

        return Ok(tokens);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout(RefreshTokenDto dto, CancellationToken ct)
    {
        await _tokenService.RevokeRefreshTokenAsync(dto.RefreshToken, ct);
        return Ok(new { message = "Logout efetuado." });
    }

    [AllowAnonymous]
    [HttpPost("forgot-password")]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordDto dto, CancellationToken ct)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        // Resposta genérica para não revelar se o e-mail existe.
        if (user is not null && await _userManager.IsEmailConfirmedAsync(user))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var encoded = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            await _emailSender.SendAsync(
                dto.Email,
                "Redefinição de senha",
                $"Use este token para redefinir sua senha: {encoded}",
                ct);
        }

        return Ok(new { message = "Se o e-mail existir, enviaremos instruções de redefinição." });
    }

    [AllowAnonymous]
    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword(ResetPasswordDto dto)
    {
        var user = await _userManager.FindByEmailAsync(dto.Email);
        if (user is null)
            return BadRequest(new { message = "Solicitação inválida." });

        var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(dto.Token));
        var result = await _userManager.ResetPasswordAsync(user, token, dto.NewPassword);
        if (!result.Succeeded)
            return BadRequest(new { errors = result.Errors.Select(e => e.Description) });

        return Ok(new { message = "Senha redefinida com sucesso." });
    }
}
