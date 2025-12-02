using Libreria.Core.Entities;
using Libreria.Core.Enums;
using Libreria.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Libreria.Core.Services
{
    public class SecurityService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _config;

        public SecurityService(IUnitOfWork unitOfWork, IConfiguration config)
        {
            _unitOfWork = unitOfWork;
            _config = config;
        }

        // ======================================================
        // REGISTER
        // ======================================================
        public async Task RegisterAsync(string login, string password, string name, RoleType role)
        {
            var exists = _unitOfWork.Security
                .Query()
                .Any(u => u.Login == login);

            if (exists)
                throw new Exception("El usuario ya existe.");

            var hashed = BCrypt.Net.BCrypt.HashPassword(password);

            var user = new Security
            {
                Login = login,
                Password = hashed,
                Name = name,
                Role = role
            };

            await _unitOfWork.Security.Add(user);
            await _unitOfWork.SaveChangesAsync();
        }

        // ======================================================
        // LOGIN (RETORNA TOKEN JWT)
        // ======================================================
        public async Task<string> LoginAsync(string login, string password)
        {
            var user = _unitOfWork.Security
                .Query()
                .FirstOrDefault(u => u.Login == login);

            if (user == null)
                throw new Exception("Usuario o contraseña incorrectos.");

            if (!BCrypt.Net.BCrypt.Verify(password, user.Password))
                throw new Exception("Usuario o contraseña incorrectos.");

            return GenerateToken(user);
        }

        // ======================================================
        // GENERAR JWT
        // ======================================================
        private string GenerateToken(Security user)
        {
            var secretKey = _config["Authentication:SecretKey"];
            var issuer = _config["Authentication:Issuer"];
            var audience = _config["Authentication:Audience"];

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Login),
                new Claim("role", user.Role.ToString()),
                new Claim("name", user.Name),
                new Claim("userId", user.Id.ToString())
            };

            var token = new JwtSecurityToken(
                issuer,
                audience,
                claims,
                expires: DateTime.UtcNow.AddHours(4),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
