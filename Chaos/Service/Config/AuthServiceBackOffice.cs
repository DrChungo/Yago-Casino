using Chaos.Api.Interface.Config;

using Chaos.Infraestructure.Models;
using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Chaos.Api.Service.Config
{
    public class AuthServiceBackOffice(CasinoDBContext context, IConfiguration configuration)
        : IAuthServiceBackOffice
    {
        private readonly CasinoDBContext _context = context;
        private readonly IConfiguration _configuration = configuration;

        public async Task<AuthBackOfficeResponse> RegisterAsync(RegisterBackOfficeRequest request)
        {
            // Verificamos si el email ya existe
            var exists = await _context.Users.AnyAsync(u => u.Email == request.Email);
            if (exists)
                return new AuthBackOfficeResponse { Message = "El email ya está registrado." };

            User user = new User
            {
                Id = Guid.NewGuid(),
                Name = request.Name,
                Email = request.Email,
                PasswordHash = PasswordHelper.Hash(request.Password),
                IsActive = true,
                IsAdmin = false
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return new AuthBackOfficeResponse { Message = "Usuario registrado correctamente." };
        }

        public async Task<AuthBackOfficeResponse> LoginAsync(LoginBackOfficeRequest request)
        {
            // Buscamos el usuario por email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

            // Si no existe o está inactivo
            if (user == null || !user.IsActive)
                return new AuthBackOfficeResponse { Message = "Credenciales inválidas." };

            // Verificamos la contraseña
            var isPasswordValid = PasswordHelper.Verify(request.Password, user.PasswordHash);
            if (!isPasswordValid)
                return new AuthBackOfficeResponse { Message = "Credenciales inválidas." };

            // Solo admins pueden acceder al BackOffice
            if (!user.IsAdmin)
                return new AuthBackOfficeResponse { Message = "No tienes permisos de administrador." };

            var token = GenerateToken(user);
            return new AuthBackOfficeResponse { Token = token, Message = "Login exitoso." };
        }

        public async Task<AuthBackOfficeResponse> MakeAdminAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
                return new AuthBackOfficeResponse { Message = "Usuario no encontrado." };

            user.IsAdmin = true;
            await _context.SaveChangesAsync();

            return new AuthBackOfficeResponse { Message = $"El usuario {user.Email} ahora es administrador." };
        }

        private string GenerateToken(User user)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim("role", "Admin")
            };

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(8),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public async Task<AuthBackOfficeResponse> RevokeAdminAsync(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);

            if (user == null)
                return new AuthBackOfficeResponse { Message = "Usuario no encontrado." };

            if (!user.IsAdmin)
                return new AuthBackOfficeResponse { Message = "El usuario ya no es administrador." };

            user.IsAdmin = false;
            await _context.SaveChangesAsync();

            return new AuthBackOfficeResponse { Message = $"El usuario {user.Email} ya no es administrador." };
        }
      
    }
}