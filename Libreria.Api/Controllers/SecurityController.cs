using Libreria.Core.Enums;
using Libreria.Core.Services;
using Libreria.Infrastructure.DTOs.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.IdentityModel.Tokens.Jwt;

namespace Libreria.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SecurityController : ControllerBase
    {
        private readonly SecurityService _securityService;

        public SecurityController(SecurityService securityService)
        {
            _securityService = securityService;
        }

        // ======================================================
        // REGISTER
        // ======================================================
        [HttpPost("register")]
        [ProducesResponseType((int)HttpStatusCode.Created)]
        public async Task<IActionResult> Register([FromBody] UserRegisterDto dto)
        {
            if (!Enum.TryParse<RoleType>(dto.Role, true, out var role))
                return BadRequest("Rol inválido. Usa: Admin o User.");

            await _securityService.RegisterAsync(dto.Login, dto.Password, dto.Name, role);

            return StatusCode(201, "Usuario registrado correctamente.");
        }

        // ======================================================
        // LOGIN
        // ======================================================
        [HttpPost("login")]
        [ProducesResponseType((int)HttpStatusCode.OK)]
        public async Task<IActionResult> Login([FromBody] UserLoginDto dto)
        {
            var token = await _securityService.LoginAsync(dto.Login, dto.Password);
            return Ok(new { token });
        }

        // ======================================================
        // ENDPOINT CON TOKEN
        // ======================================================
        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            return Ok(new
            {
                login = User.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value,
                name = User.Claims.FirstOrDefault(c => c.Type == "name")?.Value,
                role = User.Claims.FirstOrDefault(c => c.Type == "role")?.Value
            });
        }



        // ======================================================
        // ENDPOINT SOLO ADMIN
        // ======================================================
        [Authorize(Roles = "Admin")]
        [HttpGet("admin-only")]
        public IActionResult AdminOnly()
        {
            return Ok("Acceso concedido: eres Administrador.");
        }
    }
}
