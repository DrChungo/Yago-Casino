using Chaos.Api.Interface;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Chaos.Api.Utils;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Api.Service
{
    public class UserService(CasinoDBContext dbContext) : IUserService
    {
        private readonly List<UserResponse> _randomUsers = [];
        private static readonly Random _random = new();
        private readonly CasinoDBContext _dbContext = dbContext;

        // ─────────────────────────────────────────────────────────
        // CREATE USER
        // ─────────────────────────────────────────────────────────
        public async Task<UserResponse> CreateUser(UserRequest request)
        {
            if (GetUserByEmail(request.Email) != null)
                return null!;

            var id = Guid.NewGuid();
            var hashedPassword = PasswordHelper.Hash(request.Password);

            var newUser = new User
            {
                Id = id,
                Name = request.Name,
                Email = request.Email,
                PasswordHash = hashedPassword,
                Wallet = 0,
                CreatedAt = DateTime.UtcNow.ToString(),
                IsActive = true
            };

            _dbContext.Add(newUser);
            await _dbContext.SaveChangesAsync();

            return new UserResponse
            {
                Id = newUser.Id,
                Name = newUser.Name,
                Email = newUser.Email,
                Wallet = newUser.Wallet,
                IsAlive = newUser.IsActive
            };
        }

        // ─────────────────────────────────────────────────────────
        // CREATE N RANDOM USERS (Russian Roulette helpers)
        // ─────────────────────────────────────────────────────────
        public async Task<List<UserResponse>> CreateNUsers(int numUsers)
        {
            int minWalletValue = 0;
            int maxWalletValue = 10_000_000;

            for (int i = 0; i < numUsers; i++)
            {
                var id = Guid.NewGuid();
                var name = AnimalNameHelper.GetRandomName(_random);

                _randomUsers.Add(new UserResponse
                {
                    Id = id,
                    Name = name,
                    Email = $"{name}@gmail.com",
                    Wallet = _random.NextInt64(minWalletValue, maxWalletValue),
                    IsAlive = true
                });
            }

            return await Task.FromResult(_randomUsers);
        }

        // ─────────────────────────────────────────────────────────
        // GET ALL USERS
        // ─────────────────────────────────────────────────────────
        public List<UserResponse> GetAllUsers()
        {
            // ✅ AsNoTracking — no navigation property loading, no cycle risk
            return _dbContext.Users
                .AsNoTracking()
                .Select(user => new UserResponse
                {
                    Id = user.Id,
                    Name = user.Name,
                    Email = user.Email,
                    Wallet = user.Wallet,
                    IsAlive = user.IsActive
                })
                .ToList();
        }

        // ─────────────────────────────────────────────────────────
        // GET USER BY ID
        // ✅ Fixed: FirstOrDefault instead of First (no crash)
        // ✅ Fixed: AsNoTracking breaks the navigation cycle
        // ─────────────────────────────────────────────────────────
        public UserResponse? GetUserById(Guid id)
        {
            var user = _dbContext.Users
                .AsNoTracking()                          // ← ✅ breaks the cycle
                .FirstOrDefault(u => u.Id == id);       // ← ✅ safe, no exception

            if (user == null) return null;

            return new UserResponse
            {
                Id = user.Id,
                Name = user.Name,
                Email = user.Email,
                Wallet = user.Wallet,
                IsAlive = user.IsActive
            };
        }

        // ─────────────────────────────────────────────────────────
        // GET USER BY EMAIL (login only)
        // ─────────────────────────────────────────────────────────
        public User? GetUserByEmail(string email)
        {
            return _dbContext.Users
                .AsNoTracking()
                .FirstOrDefault(e => e.Email == email);
        }

        // ─────────────────────────────────────────────────────────
        // SEND USER TO GRAVEYARD
        // ✅ Fixed: SaveChanges was missing — changes were never persisted
        // ─────────────────────────────────────────────────────────
        public bool SendUserToGraveyard(Guid idUser)
        {
            var deadPlayer = _dbContext.Users.Find(idUser);
            if (deadPlayer == null) return false;

            deadPlayer.IsActive = false;
            _dbContext.SaveChanges();   // ← ✅ was missing before

            return true;
        }

        // ─────────────────────────────────────────────────────────
        // ADD INTO WALLET
        // ✅ Fixed: parameter type changed to long to match wallet size
        // ─────────────────────────────────────────────────────────
        public async Task<bool> AddIntoWallet(Guid walletId, long addAmount)  // ← long ✅
        {
            var walletPlayer = _dbContext.Users.Find(walletId);
            if (walletPlayer == null) return false;

            walletPlayer.Wallet += addAmount;
            await _dbContext.SaveChangesAsync();

            return true;
        }

        // ─────────────────────────────────────────────────────────
        // SELECTED USERS (Russian Roulette)
        // ─────────────────────────────────────────────────────────
        public List<User> SelectedUsers(List<Guid> idPlayers)
        {
            List<User> usersList = [];

            foreach (var userId in idPlayers)
            {
                var player = _dbContext.Find<User>(userId);
                if (player != null)
                    usersList.Add(player);
            }

            return usersList;
        }
    }
}