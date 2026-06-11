using Chaos.Api.Interface;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Chaos.Api.Utils;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chaos.Api.Service
{
    public class AuthService(IUserService userService, IConfiguration configuration) : IAuthService
    {
        private readonly IUserService _userService = userService;
        private readonly IConfiguration _configuration = configuration;

        public LoginResponse? Login(LoginRequest request)
        {
            //Buscamos el usuario por email
            var user = _userService.GetUserByEmail(request.Email);

            //Si no existe o está inactivo, retornamos null
            if (user == null || !user.IsActive) return null;

            //Verificamos la contraseña hasheada
            var isPasswordValid = PasswordHelper.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid) return null;

            //Generamos el token JWT
            var token = GenerateToken(user);

            return new LoginResponse
            {
                Token = token
            };
        }

        private string GenerateToken(Chaos.Infraestructure.Models.User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}