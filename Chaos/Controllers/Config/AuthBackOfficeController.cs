using Chaos.Api.Interface.Config;
using Chaos.Infraestructure.Data;
using Chaos.Infraestructure.Models;
using Chaos.Shared.Dto;
using Chaos.Shared.RequestEntity;
using Chaos.Shared.ResponseEntity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Controllers.BackOffice
{
    [ApiController]
    [Route("api/backoffice/auth")]
    public class AuthBackOfficeController(
        IAuthServiceBackOffice authService,
        CasinoDBContext context) : ControllerBase
    {
        private readonly IAuthServiceBackOffice _authService = authService;
        private readonly CasinoDBContext _context = context;

        // ─────────────────────────────────────────
        // POST register
        // ─────────────────────────────────────────
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterBackOfficeRequest request)
        {
            var response = await _authService.RegisterAsync(request);
            if (response.Token == null && response.Message != "Usuario registrado correctamente.")
                return BadRequest(response);

            return Ok(response);
        }

        // ─────────────────────────────────────────
        // POST login
        // ─────────────────────────────────────────
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginBackOfficeRequest request)
        {
            var response = await _authService.LoginAsync(request);
            if (response.Token == null)
                return Unauthorized(response);

            return Ok(response);
        }

        // ─────────────────────────────────────────
        // PATCH make-admin
        // ─────────────────────────────────────────
        /// <summary>
        /// Promueve a un usuario existente a administrador.
        /// Solo accesible para admins.
        /// </summary>
        [HttpPatch("make-admin/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> MakeAdmin(Guid userId)
        {
            var response = await _authService.MakeAdminAsync(userId);
            if (response.Message == "Usuario no encontrado.")
                return NotFound(response);

            return Ok(response);
        }

        // ─────────────────────────────────────────
        // PATCH revoke-admin
        // ─────────────────────────────────────────
        /// <summary>
        /// Revoca el rol de administrador a un usuario.
        /// Solo accesible para admins.
        /// </summary>
        [HttpPatch("revoke/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> RevokeAdmin(Guid userId)
        {
            var result = await _authService.RevokeAdminAsync(userId);
            if (result.Message.Contains("no encontrado"))
                return NotFound(new { message = result.Message });

            return Ok(result);
        }

        // ─────────────────────────────────────────
        // GET todos los usuarios
        // ─────────────────────────────────────────
        [HttpGet("users")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllUsers()
        {
            var result = await _context.Users
                .Select(u => new UserConfigDto
                {
                    Id = u.Id,
                    Email = u.Email ?? "",
                    UserName = u.Name ?? "",
                    IsAdmin = u.IsAdmin,
                    IsActive = u.IsActive,
                    Balance = u.Wallet
                })
                .ToListAsync();

            return Ok(result);
        }

        // ─────────────────────────────────────────
        // GET sesiones de un usuario
        // ─────────────────────────────────────────
        [HttpGet("users/{userId}/sessions")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUserSessions(Guid userId)
        {
            var sessions = await _context.GameSessions
                .Where(s => s.UserId == userId)
                .Include(s => s.Game)
                .Include(s => s.BlackjackSessions)
                .Include(s => s.CoinFlipSessions)
                .Include(s => s.HigherLowerSessions)
                .Include(s => s.RouletteSessions)
                .Include(s => s.SlotSessions)
                .ToListAsync();

            var result = sessions.Select(s =>
            {
                var hl = s.HigherLowerSessions
                           .OrderByDescending(h =>
                               DateTime.TryParse(h.StartedAt, out var d) ? d : DateTime.MinValue)
                           .FirstOrDefault();

                var bj = s.BlackjackSessions
                           .OrderByDescending(b =>
                               DateTime.TryParse(b.StartedAt, out var d) ? d : DateTime.MinValue)
                           .FirstOrDefault();

                var cf = s.CoinFlipSessions
                           .OrderByDescending(c =>
                               DateTime.TryParse(c.PlayedAt, out var d) ? d : DateTime.MinValue)
                           .FirstOrDefault();

                var resultLower = s.Result?.ToLower().Trim() ?? "";

                bool isFinished = resultLower != "progress"
                               && resultLower != "started"
                               && resultLower != "";   // por si viene null/vacío

                //Win = resultado positivo Y dinero ganado
                // NO dependemos del texto "win"/"completed" — usamos MoneyEarned directamente
                bool isWin = s.Result?.ToLower().Trim() switch
                {
                    "win" => true,                  // CoinFlip, Roulette, Slots, RussianRoulette
                    "lose" => false,
                    "draw" => false,
                    "completed" => s.MoneyEarned > 0,     // Blackjack, HigherLower
                    "progress" => false,                 // partida en curso
                    _ => s.MoneyEarned > 0      // fallback seguro
                };

                // ─── RoundsPlayed ───
                int roundsPlayed = s.HigherLowerSessions.Any()
                    ? s.HigherLowerSessions.Sum(h => h.RoundsPlayed)
                    : s.BlackjackSessions.Count
                    + s.CoinFlipSessions.Count
                    + s.RouletteSessions.Count
                    + s.SlotSessions.Count;

                return new GameSessionDto
                {
                    Id = s.Id,
                    GameName = s.Game?.GameName ?? "",
                    MoneyDelta = s.MoneyEarned,
                    IsWin = isWin,
                    StartDate = bj?.StartedAt ?? hl?.StartedAt ?? cf?.PlayedAt ?? s.PlayedAt ?? "",
                    EndDate = bj?.FinishedAt ?? hl?.FinishedAt,
                    RoundsPlayed = roundsPlayed
                };
            })
            .OrderByDescending(s =>
                DateTime.TryParse(s.StartDate, out var d) ? d : DateTime.MinValue)
            .ToList();

            return Ok(result);
        }
        // ─────────────────────────────────────────
        // PATCH desactivar usuario
        // ─────────────────────────────────────────
        [HttpPatch("users/{userId}/deactivate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeactivateUser(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null) return NotFound(new { Message = "Usuario no encontrado." });

            user.IsActive = false;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuario desactivado." });
        }


        [HttpPatch("users/{userId}/activate")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ActivateUser(Guid userId)
        {
            var user = await _context.Users.FindAsync(userId);
            if (user is null) return NotFound(new { Message = "Usuario no encontrado." });

            user.IsActive = true;
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Usuario activado." });
        }



        [HttpGet("debug-claims")]
        [AllowAnonymous]  // ← importante, sin autorización
        public IActionResult DebugClaims()
        {
            var claims = User.Claims.Select(c => new { c.Type, c.Value });
            var isAuthenticated = User.Identity?.IsAuthenticated;
            var isAdmin = User.IsInRole("Admin");

            return Ok(new { isAuthenticated, isAdmin, claims });
        }
    }
}