using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;

namespace Chaos.Infraestructure.Data
{
public static class CasinoDbInitializer
    {
        public static void Initialize(CasinoDBContext context)
        {
            //context.Database.Migrate();
            context.Database.CanConnect();

            // ── 1. USERS ─────────────────────────────────────────────────────────
            if (!context.Users.Any())
            {
            var users = new List<User>
            {
            new() { Id = Guid.NewGuid(), Name = "admin",        Email = "admin@chaos.com",   PasswordHash = "1234", Wallet = 10000m, IsActive = true, IsAdmin = true  },
            new() { Id = Guid.NewGuid(), Name = "miguel angel", Email = "player1@chaos.com", PasswordHash = "1234", Wallet = 5000m,  IsActive = true, IsAdmin = false },
            new() { Id = Guid.NewGuid(), Name = "yuk hank",     Email = "player2@chaos.com", PasswordHash = "1234", Wallet = 3000m,  IsActive = true, IsAdmin = false },
            new() { Id = Guid.NewGuid(), Name = "pere",         Email = "player3@chaos.com", PasswordHash = "1234", Wallet = 2000m,  IsActive = true, IsAdmin = false },
            new() { Id = Guid.NewGuid(), Name = "aaron",        Email = "player4@chaos.com", PasswordHash = "1234", Wallet = 1500m,  IsActive = true, IsAdmin = false },
            new() { Id = Guid.NewGuid(), Name = "aron",         Email = "player5@chaos.com", PasswordHash = "1234", Wallet = 1000m,  IsActive = true, IsAdmin = false },
            };
            context.Users.AddRange(users);
            context.SaveChanges();
            }

            // ── 2. CASINO ────────────────────────────────────────────────────────
            if (!context.Casinos.Any())
            {
            var now = DateTime.UtcNow.ToString();
            context.Casinos.Add(new Casino
            {
            Id = Guid.NewGuid(),
            Name = "Casino Yago Royal",
            Description = "El casino más Yago",
            CreatedAt = now
            });
            context.SaveChanges();
            }

            // ── 3. ANIMAL VALUE CONFIGS ──────────────────────────────────────────
            if (!context.AnimalValueConfigs.Any())
            {
            var adminUser = context.Users.First();
            var now = DateTime.UtcNow.ToString();

            var configs = new List<AnimalValueConfig>
            {
            new() { Id = Guid.NewGuid(), AnimalType = "WHALE",     MinAge = 1, MaxAge = 10, MinWeight = 100m,  MaxWeight = 150000m, MinHeight = 200m, MaxHeight = 3000m, MinHealth = 50, MaxHealth = 100, Habitat = "Ocean",    IsActive = true, UpdatedBy = adminUser.Id },
            new() { Id = Guid.NewGuid(), AnimalType = "FLY",       MinAge = 1, MaxAge = 1,  MinWeight = 0.01m, MaxWeight = 0.05m,   MinHeight = 0.1m, MaxHeight = 1m,    MinHealth = 10, MaxHealth = 100, Habitat = "Urban",    IsActive = true, UpdatedBy = adminUser.Id },
            new() { Id = Guid.NewGuid(), AnimalType = "HAMSTER",   MinAge = 1, MaxAge = 3,  MinWeight = 0.1m,  MaxWeight = 0.3m,    MinHeight = 5m,   MaxHeight = 15m,   MinHealth = 20, MaxHealth = 100, Habitat = "Domestic", IsActive = true, UpdatedBy = adminUser.Id },
            new() { Id = Guid.NewGuid(), AnimalType = "SHEEP",     MinAge = 1, MaxAge = 12, MinWeight = 45m,   MaxWeight = 100m,    MinHeight = 60m,  MaxHeight = 90m,   MinHealth = 30, MaxHealth = 100, Habitat = "Farm",     IsActive = true, UpdatedBy = adminUser.Id },
            new() { Id = Guid.NewGuid(), AnimalType = "HORSE",     MinAge = 1, MaxAge = 30, MinWeight = 380m,  MaxWeight = 1000m,   MinHeight = 140m, MaxHeight = 180m,  MinHealth = 40, MaxHealth = 100, Habitat = "Farm",     IsActive = true, UpdatedBy = adminUser.Id },
            new() { Id = Guid.NewGuid(), AnimalType = "CROCODILE", MinAge = 1, MaxAge = 70, MinWeight = 40m,   MaxWeight = 1000m,   MinHeight = 100m, MaxHeight = 600m,  MinHealth = 50, MaxHealth = 100, Habitat = "Swamp",    IsActive = true, UpdatedBy = adminUser.Id },
            new() { Id = Guid.NewGuid(), AnimalType = "SHARK",     MinAge = 1, MaxAge = 30, MinWeight = 100m,  MaxWeight = 2000m,   MinHeight = 150m, MaxHeight = 700m,  MinHealth = 50, MaxHealth = 100, Habitat = "Ocean",    IsActive = true, UpdatedBy = adminUser.Id },
            new() { Id = Guid.NewGuid(), AnimalType = "DOG",       MinAge = 1, MaxAge = 15, MinWeight = 2m,    MaxWeight = 80m,     MinHeight = 15m,  MaxHeight = 90m,   MinHealth = 20, MaxHealth = 100, Habitat = "Domestic", IsActive = true, UpdatedBy = adminUser.Id },
            new() { Id = Guid.NewGuid(), AnimalType = "CAT",       MinAge = 1, MaxAge = 20, MinWeight = 2m,    MaxWeight = 10m,     MinHeight = 20m,  MaxHeight = 35m,   MinHealth = 20, MaxHealth = 100, Habitat = "Domestic", IsActive = true, UpdatedBy = adminUser.Id },
            new() { Id = Guid.NewGuid(), AnimalType = "COW",       MinAge = 1, MaxAge = 20, MinWeight = 400m,  MaxWeight = 1000m,   MinHeight = 120m, MaxHeight = 160m,  MinHealth = 30, MaxHealth = 100, Habitat = "Farm",     IsActive = true, UpdatedBy = adminUser.Id },
            };
            context.AnimalValueConfigs.AddRange(configs);
            context.SaveChanges();
            }

            // ── 4. ANIMALS ───────────────────────────────────────────────────────
            if (!context.Animals.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var users = context.Users.ToList();
            var configs = context.AnimalValueConfigs.ToList();

            var animals = new List<Animal>
            {
            new() { Id = Guid.NewGuid(), Name = "Rex",      AnimalType = "DOG",   Health = 100, Rarity = true,  Age = 5,  Weight = 30m,   Height = 60m,  EstimatedValue = 1200m, IsAvailable = true, OwnerId = users[0 % users.Count].Id, AnimalValueConfigId = configs.First(c => c.AnimalType == "DOG").Id,   CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "Whiskers", AnimalType = "CAT",   Health = 85,  Rarity = false, Age = 3,  Weight = 5m,    Height = 15m,  EstimatedValue = 300m,  IsAvailable = true, OwnerId = users[1 % users.Count].Id, AnimalValueConfigId = configs.First(c => c.AnimalType == "CAT").Id,   CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "Thunder",  AnimalType = "CAT",   Health = 90,  Rarity = false, Age = 8,  Weight = 8m,    Height = 30m,  EstimatedValue = 8500m, IsAvailable = true, OwnerId = users[2 % users.Count].Id, AnimalValueConfigId = configs.First(c => c.AnimalType == "CAT").Id,   CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "Bessie",   AnimalType = "COW",   Health = 75,  Rarity = false, Age = 10, Weight = 800m,  Height = 140m, EstimatedValue = 6000m, IsAvailable = true, OwnerId = users[3 % users.Count].Id, AnimalValueConfigId = configs.First(c => c.AnimalType == "COW").Id,   CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "Fluffy",   AnimalType = "COW",   Health = 60,  Rarity = false, Age = 1,  Weight = 400m,  Height = 120m, EstimatedValue = 50m,   IsAvailable = true, OwnerId = users[4 % users.Count].Id, AnimalValueConfigId = configs.First(c => c.AnimalType == "COW").Id,   CreatedAt = now },
            new() { Id = Guid.NewGuid(), Name = "Jaws",     AnimalType = "SHARK", Health = 95,  Rarity = true,  Age = 6,  Weight = 500m,  Height = 200m, EstimatedValue = 6500m, IsAvailable = true, OwnerId = users[5 % users.Count].Id, AnimalValueConfigId = configs.First(c => c.AnimalType == "SHARK").Id, CreatedAt = now },
            };
            context.Animals.AddRange(animals);
            context.SaveChanges();
            }

            // ── 5. GAMES ─────────────────────────────────────────────────────────
            if (!context.Games.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var games = new List<Game>
            {
            new() { Id = Guid.NewGuid(), GameName = "Blackjack-Common",         GameType = "Blackjack",       Description = "Llega a 21 sin pasarte",                         IsActive = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), GameName = "Coin Flip-Common",          GameType = "CoinFlip",        Description = "Cara o cruz, tú decides",                        IsActive = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), GameName = "Higher or Lower-Common",    GameType = "HigherLower",     Description = "¿La siguiente carta es mayor?",                  IsActive = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), GameName = "Roulette-Common",           GameType = "Roulette",        Description = "La ruleta de la fortuna",                        IsActive = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), GameName = "Russian Roulette-6Players", GameType = "RussianRoulette", Description = "El juego más peligroso del casino. 6 jugadores", IsActive = true, CreatedAt = now },
            new() { Id = Guid.NewGuid(), GameName = "Slots-Common",              GameType = "Slots",           Description = "Máquina tragamonedas del Chaos Casino",          IsActive = true, CreatedAt = now },
            };
            context.Games.AddRange(games);
            context.SaveChanges();
            }

            // ── 6. COIN FLIP GAME CONFIG ─────────────────────────────────────────
            if (!context.CoinFlipGameConfigs.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var coinFlipGame = context.Games.First(g => g.GameType == "CoinFlip");
            context.CoinFlipGameConfigs.Add(new CoinFlipGameConfig
            {
            Id = Guid.NewGuid(),
            ConfigName = "Standard Coin Flip",
            IsActive = true,
            CreatedAt = now,
            GameId = coinFlipGame.Id
            });
            context.SaveChanges();
            }

            // ── 7. BLACKJACK GAME CONFIG ─────────────────────────────────────────
            if (!context.BlackjackGameConfigs.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var blackjackGame = context.Games.First(g => g.GameType == "Blackjack");
            context.BlackjackGameConfigs.Add(new BlackjackGameConfig
            {
            Id = Guid.NewGuid(),
            TableName = "Main Table",
            MaxPlayers = 6,
            NumberOfDecks = 6,
            DealerStandsOn = 17,
            BlackjackPayout = 1.5m,
            AllowDoubleDown = true,
            AllowInsurance = true,
            IsActive = true,
            CreatedAt = now,
            GameId = blackjackGame.Id
            });
            context.SaveChanges();
            }

            // ── 8. HIGHER OR LOWER GAME CONFIG ───────────────────────────────────
            if (!context.HigherLowerGameConfigs.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var higherLowerGame = context.Games.First(g => g.GameType == "HigherLower");
            context.HigherLowerGameConfigs.Add(new HigherLowerGameConfig
            {
            Id = Guid.NewGuid(),
            ConfigName = "Standard Higher or Lower",
            BaseMultiplier = 2.0m,
            RoundIncrement = 0.1m,
            IsActive = true,
            CreatedAt = now,
            GameId = higherLowerGame.Id
            });
            context.SaveChanges();
            }

            // ── 9. ROULETTE GAME CONFIG ──────────────────────────────────────────
            if (!context.RouletteGameConfigs.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var rouletteGame = context.Games.First(g => g.GameType == "Roulette");
            var rouletteConfigId = Guid.NewGuid();
            context.RouletteGameConfigs.Add(new RouletteGameConfig
            {
            Id = rouletteConfigId,
            TableName = "Main Roulette Table",
            RouletteType = "European",
            HasZero = true,
            HasDoubleZero = false,
            TotalNumbers = 37,
            IsActive = true,
            CreatedAt = now,
            GameId = rouletteGame.Id,
            RouletteBetTypes = new List<RouletteBetType>
            {
            new() { Id = Guid.NewGuid(), BetName = "Straight Up",  Payout = 35m, Description = "Apuesta a un único número",                 IsActive = true, CreatedAt = now, RouletteGameConfigId = rouletteConfigId },
            new() { Id = Guid.NewGuid(), BetName = "Split",        Payout = 17m, Description = "Apuesta a dos números adyacentes",          IsActive = true, CreatedAt = now, RouletteGameConfigId = rouletteConfigId },
            new() { Id = Guid.NewGuid(), BetName = "Street",       Payout = 11m, Description = "Apuesta a tres números en una fila",        IsActive = true, CreatedAt = now, RouletteGameConfigId = rouletteConfigId },
            new() { Id = Guid.NewGuid(), BetName = "Corner",       Payout = 8m,  Description = "Apuesta a cuatro números en esquina",       IsActive = true, CreatedAt = now, RouletteGameConfigId = rouletteConfigId },
            new() { Id = Guid.NewGuid(), BetName = "Red / Black",  Payout = 1m,  Description = "Apuesta al color rojo o negro",             IsActive = true, CreatedAt = now, RouletteGameConfigId = rouletteConfigId },
            new() { Id = Guid.NewGuid(), BetName = "Odd / Even",   Payout = 1m,  Description = "Apuesta a par o impar",                     IsActive = true, CreatedAt = now, RouletteGameConfigId = rouletteConfigId },
            new() { Id = Guid.NewGuid(), BetName = "1-18 / 19-36", Payout = 1m,  Description = "Apuesta a la mitad baja o alta",            IsActive = true, CreatedAt = now, RouletteGameConfigId = rouletteConfigId },
            new() { Id = Guid.NewGuid(), BetName = "Dozen",        Payout = 2m,  Description = "Apuesta a una docena (1-12, 13-24, 25-36)", IsActive = true, CreatedAt = now, RouletteGameConfigId = rouletteConfigId },
            new() { Id = Guid.NewGuid(), BetName = "Column",       Payout = 2m,  Description = "Apuesta a una columna de números",          IsActive = true, CreatedAt = now, RouletteGameConfigId = rouletteConfigId },
            }
            });
            context.SaveChanges();
            }

            // ── 10. RUSSIAN ROULETTE GAME CONFIG ─────────────────────────────────
            if (!context.RussianRouletteGameConfigs.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var russianRouletteGame = context.Games.First(g => g.GameType == "RussianRoulette");
            context.RussianRouletteGameConfigs.Add(new RussianRouletteGameConfig
            {
            Id = Guid.NewGuid(),
            ConfigName = "Standard Russian Roulette",
            TotalChambers = 6,
            BulletCount = 1,
            MaxPlayers = 6,
            MinPlayers = 2,
            FixedPrizePool = 1000m,
            AllowBots = true,
            IsActive = true,
            CreatedAt = now,
            GameId = russianRouletteGame.Id
            });
            context.SaveChanges();
            }

            // ── 11. SLOT GAME CONFIG ──────────────────────────────────────────────
            if (!context.SlotGameConfigs.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var slotGame = context.Games.First(g => g.GameType == "Slots");
            context.SlotGameConfigs.Add(new SlotGameConfig
            {
            Id = Guid.NewGuid(),
            MachineName = "Chaos Mega Slots",
            Multiplier = 1.5m,
            NumberOfReels = 5,
            NumberOfRows = 3,
            PayLines = 20,
            IsActive = true,
            CreatedAt = now,
            GameId = slotGame.Id
            });
            context.SaveChanges();
            }

            // ── 12. SLOT SYMBOLS ──────────────────────────────────────────────────
            if (!context.SlotSymbols.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var slotConfig = context.SlotGameConfigs.First();
            context.SlotSymbols.AddRange(new List<SlotSymbol>
            {
            new() { Id = Guid.NewGuid(), SymbolName = "Cherry",  SymbolCode = "CHERRY",  Rarity = "Common",    BaseValue = 1m,   IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id },
            new() { Id = Guid.NewGuid(), SymbolName = "Lemon",   SymbolCode = "LEMON",   Rarity = "Common",    BaseValue = 1m,   IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id },
            new() { Id = Guid.NewGuid(), SymbolName = "Orange",  SymbolCode = "ORANGE",  Rarity = "Common",    BaseValue = 2m,   IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id },
            new() { Id = Guid.NewGuid(), SymbolName = "Bell",    SymbolCode = "BELL",    Rarity = "Uncommon",  BaseValue = 5m,   IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id },
            new() { Id = Guid.NewGuid(), SymbolName = "Bar",     SymbolCode = "BAR",     Rarity = "Uncommon",  BaseValue = 10m,  IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id },
            new() { Id = Guid.NewGuid(), SymbolName = "Seven",   SymbolCode = "SEVEN",   Rarity = "Rare",      BaseValue = 25m,  IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id },
            new() { Id = Guid.NewGuid(), SymbolName = "Diamond", SymbolCode = "DIAMOND", Rarity = "Epic",      BaseValue = 50m,  IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id },
            new() { Id = Guid.NewGuid(), SymbolName = "Wild",    SymbolCode = "WILD",    Rarity = "Legendary", BaseValue = 100m, IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id },
            });
            context.SaveChanges();
            }

            // ── 13. SLOT PAYOUT RULES ─────────────────────────────────────────────
            if (!context.SlotPayoutRules.Any())
            {
            var now = DateTime.UtcNow.ToString();
            var slotConfig = context.SlotGameConfigs.First();
            var cherry = context.SlotSymbols.First(s => s.SymbolCode == "CHERRY");
            var lemon = context.SlotSymbols.First(s => s.SymbolCode == "LEMON");
            var orange = context.SlotSymbols.First(s => s.SymbolCode == "ORANGE");
            var bell = context.SlotSymbols.First(s => s.SymbolCode == "BELL");
            var bar = context.SlotSymbols.First(s => s.SymbolCode == "BAR");
            var seven = context.SlotSymbols.First(s => s.SymbolCode == "SEVEN");
            var diamond = context.SlotSymbols.First(s => s.SymbolCode == "DIAMOND");
            var wild = context.SlotSymbols.First(s => s.SymbolCode == "WILD");

            context.SlotPayoutRules.AddRange(new List<SlotPayoutRule>
            {
            new() { Id = Guid.NewGuid(), Combination = "CHERRY x3",  PayoutMultiplier = 2m,   IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id, SlotSymbolId = cherry.Id  },
            new() { Id = Guid.NewGuid(), Combination = "LEMON x3",   PayoutMultiplier = 2m,   IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id, SlotSymbolId = lemon.Id   },
            new() { Id = Guid.NewGuid(), Combination = "ORANGE x3",  PayoutMultiplier = 3m,   IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id, SlotSymbolId = orange.Id  },
            new() { Id = Guid.NewGuid(), Combination = "BELL x3",    PayoutMultiplier = 5m,   IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id, SlotSymbolId = bell.Id    },
            new() { Id = Guid.NewGuid(), Combination = "BAR x3",     PayoutMultiplier = 10m,  IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id, SlotSymbolId = bar.Id     },
            new() { Id = Guid.NewGuid(), Combination = "SEVEN x3",   PayoutMultiplier = 25m,  IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id, SlotSymbolId = seven.Id   },
            new() { Id = Guid.NewGuid(), Combination = "DIAMOND x3", PayoutMultiplier = 50m,  IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id, SlotSymbolId = diamond.Id },
            new() { Id = Guid.NewGuid(), Combination = "WILD x3",    PayoutMultiplier = 100m, IsActive = true, CreatedAt = now, SlotGameConfigId = slotConfig.Id, SlotSymbolId = wild.Id    },
            });
            context.SaveChanges();
            }

            // ── 14. GAME SESSIONS ─────────────────────────────────────────────────
            if (!context.GameSessions.Any())
            {
            var user = context.Users.First();
            var animal = context.Animals.First();
            var coinFlipGame = context.Games.First(g => g.GameType == "CoinFlip");
            var blackjackGame = context.Games.First(g => g.GameType == "Blackjack");
            var hlGame = context.Games.First(g => g.GameType == "HigherLower");
            var now = DateTime.UtcNow;

            context.GameSessions.AddRange(new List<GameSession>
            {
            new() { Id = Guid.NewGuid(), Result = "Win", MoneyEarned = 250m, Multiplier = 0.5m, PlayedAt = now.ToString(),                UserId = user.Id, AnimalId = animal.Id, GameId = coinFlipGame.Id  },
            new() { Id = Guid.NewGuid(), Result = "Win", MoneyEarned = 750m, Multiplier = 1.5m, PlayedAt = now.AddMinutes(-30).ToString(), UserId = user.Id, AnimalId = animal.Id, GameId = blackjackGame.Id },
            new() { Id = Guid.NewGuid(), Result = "Win", MoneyEarned = 180m, Multiplier = 2.0m, PlayedAt = now.AddMinutes(-15).ToString(), UserId = user.Id, AnimalId = animal.Id, GameId = hlGame.Id        },
            });
            context.SaveChanges();
            }

            // ── 15. COIN FLIP SESSION ─────────────────────────────────────────────
            if (!context.CoinFlipSessions.Any())
            {
            var now = DateTime.UtcNow;
            var user = context.Users.First();
            var animal = context.Animals.First();
            var coinFlipConfig = context.CoinFlipGameConfigs.First();
            var gameSession = context.GameSessions.First(g => g.GameId == context.Games.First(x => x.GameType == "CoinFlip").Id);

            context.CoinFlipSessions.Add(new CoinFlipSession
            {
            Id = Guid.NewGuid(),
            CoinResult = "Heads",
            UserChoice = "Heads",
            IsWin = true,
            WinProbabilityUsed = 0.60m,
            PrizeMultiplierUsed = 0.5m,
            MoneyEarned = 250m,
            PlayedAt = now.ToString(),
            UserId = user.Id,
            AnimalId = animal.Id,
            GameSessionId = gameSession.Id,
            CoinFlipGameConfigId = coinFlipConfig.Id
            });
            context.SaveChanges();
            }

            // ── 16. BLACKJACK SESSION + HANDS + CARDS + ACTIONS ──────────────────
            if (!context.BlackjackSessions.Any())
            {
            var now = DateTime.UtcNow;
            var user = context.Users.First();
            var animal = context.Animals.First();
            var blackjackConfig = context.BlackjackGameConfigs.First();
            var gameSession = context.GameSessions.First(g => g.GameId == context.Games.First(x => x.GameType == "Blackjack").Id);
            var blackjackSessionId = Guid.NewGuid();
            var playerHandId = Guid.NewGuid();
            var dealerHandId = Guid.NewGuid();

            context.BlackjackSessions.Add(new BlackjackSession
            {
            Id = blackjackSessionId,
            Status = "PlayerWin",
            PlayerScore = 21,
            DealerScore = 17,
            HasSplit = false,
            HasDoubledDown = false,
            HasInsurance = false,
            InsuranceWon = null,
            MoneyEarned = 750m,
            StartedAt = now.AddMinutes(-35).ToString(),
            FinishedAt = now.AddMinutes(-30).ToString(),
            UserId = user.Id,
            AnimalId = animal.Id,
            GameSessionId = gameSession.Id,
            BlackjackGameConfigId = blackjackConfig.Id,
            BlackjackHands = new List<BlackjackHand>
            {
            new()
            {
            Id = playerHandId, HandOwner = "Player", HandNumber = 1,
            FinalScore = 21, IsBust = false, IsBlackjack = true,
            BlackjackSessionId = blackjackSessionId,
            BlackjackCards = new List<BlackjackCard>
            {
            new() { Id = Guid.NewGuid(), CardValue = "A", CardSuit = "Spades", CardDisplay = "A♠", NumericValue = 11, CardBlackjackValue = 11, IsFaceDown = false, DealtAt = 1, HandId = playerHandId },
            new() { Id = Guid.NewGuid(), CardValue = "K", CardSuit = "Hearts", CardDisplay = "K♥", NumericValue = 10, CardBlackjackValue = 10, IsFaceDown = false, DealtAt = 2, HandId = playerHandId },
            },
            BlackjackActions = new List<BlackjackAction>
            {
            new() { Id = Guid.NewGuid(), ActionType = "Stand", ActionOrder = 1, ActionAt = now.AddMinutes(-32).ToString(), BlackjackSessionId = blackjackSessionId, HandId = playerHandId }
            }
            },
            new()
            {
            Id = dealerHandId, HandOwner = "Dealer", HandNumber = 1,
            FinalScore = 17, IsBust = false, IsBlackjack = false,
            BlackjackSessionId = blackjackSessionId,
            BlackjackCards = new List<BlackjackCard>
            {
            new() { Id = Guid.NewGuid(), CardValue = "10", CardSuit = "Clubs",    CardDisplay = "10♣", NumericValue = 10, CardBlackjackValue = 10, IsFaceDown = false, DealtAt = 1, HandId = dealerHandId },
            new() { Id = Guid.NewGuid(), CardValue = "7",  CardSuit = "Diamonds", CardDisplay = "7♦",  NumericValue = 7,  CardBlackjackValue = 7,  IsFaceDown = false, DealtAt = 2, HandId = dealerHandId },
            },
            BlackjackActions = new List<BlackjackAction>
            {
            new() { Id = Guid.NewGuid(), ActionType = "Stand", ActionOrder = 1, ActionAt = now.AddMinutes(-31).ToString(), BlackjackSessionId = blackjackSessionId, HandId = dealerHandId }
            }
            }
            }
            });
            context.SaveChanges();
            }

            // ── 17. HIGHER OR LOWER SESSION + ROUNDS ─────────────────────────────
            if (!context.HigherLowerSessions.Any())
            {
            var now = DateTime.UtcNow;
            var user = context.Users.First();
            var animal = context.Animals.First();
            var config = context.HigherLowerGameConfigs.First();
            var gameSession = context.GameSessions.First(g => g.GameId == context.Games.First(x => x.GameType == "HigherLower").Id);
            var hlSessionId = Guid.NewGuid();

            context.HigherLowerSessions.Add(new HigherLowerSession
            {
            Id = hlSessionId,
            Status = "Cashed",
            TotalEarned = 180m,
            RoundsPlayed = 3,
            StartedAt = now.AddMinutes(-20).ToString(),
            FinishedAt = now.AddMinutes(-15).ToString(),
            UserId = user.Id,
            AnimalId = animal.Id,
            GameSessionId = gameSession.Id,
            HigherLowerGameConfigId = config.Id,
            HigherLowerRounds = new List<HigherLowerRound>
            {
            new() { Id = Guid.NewGuid(), RoundNumber = 1, CurrentCard = "7♠", NextCard = "J♥", UserGuess = "Higher", IsCorrect = true, EarnedThisRound = 100m, PlayedAt = now.AddMinutes(-19).ToString(), HigherLowerSessionId = hlSessionId },
            new() { Id = Guid.NewGuid(), RoundNumber = 2, CurrentCard = "J♥", NextCard = "3♦", UserGuess = "Lower",  IsCorrect = true, EarnedThisRound = 50m,  PlayedAt = now.AddMinutes(-18).ToString(), HigherLowerSessionId = hlSessionId },
            new() { Id = Guid.NewGuid(), RoundNumber = 3, CurrentCard = "3♦", NextCard = "9♣", UserGuess = "Higher", IsCorrect = true, EarnedThisRound = 30m,  PlayedAt = now.AddMinutes(-17).ToString(), HigherLowerSessionId = hlSessionId },
            }
            });
            context.SaveChanges();
            }

            // ── 18. SLOT SESSION ──────────────────────────────────────────────────
            if (!context.SlotSessions.Any())
            {
            var now = DateTime.UtcNow;
            var user = context.Users.First();
            var animal = context.Animals.First();
            var slotConfig = context.SlotGameConfigs.First();
            var slotGame = context.Games.First(g => g.GameType == "Slots");

            var slotGameSessionId = Guid.NewGuid();
            context.GameSessions.Add(new GameSession
            {
            Id = slotGameSessionId,
            Result = "Win",
            MoneyEarned = 500m,
            Multiplier = 1.5m,
            PlayedAt = now.AddMinutes(-10).ToString(),
            UserId = user.Id,
            AnimalId = animal.Id,
            GameId = slotGame.Id
            });
            context.SaveChanges();

            context.SlotSessions.Add(new SlotSession
            {
            Id = Guid.NewGuid(),
            BetAmount = 50m,
            WinningSymbols = "SEVEN,SEVEN,SEVEN",
            GameSessionId = slotGameSessionId,
            SlotGameConfigId = slotConfig.Id
            });
            context.SaveChanges();
            }

            // ── 19. ROULETTE SESSION ──────────────────────────────────────────────
            if (!context.RouletteSessions.Any())
            {
            var now = DateTime.UtcNow;
            var user = context.Users.First();
            var animal = context.Animals.First();
            var rouletteConfig = context.RouletteGameConfigs.First();
            var rouletteGame = context.Games.First(g => g.GameType == "Roulette");

            var rouletteGameSessionId = Guid.NewGuid();
            context.GameSessions.Add(new GameSession
            {
            Id = rouletteGameSessionId,
            Result = "Win",
            MoneyEarned = 350m,
            Multiplier = 35m,
            PlayedAt = now.AddMinutes(-5).ToString(),
            UserId = user.Id,
            AnimalId = animal.Id,
            GameId = rouletteGame.Id
            });
            context.SaveChanges();

            context.RouletteSessions.Add(new RouletteSession
            {
            Id = Guid.NewGuid(),
            SpinResult = 17,
            RouletteGameConfigId = rouletteConfig.Id,
            GameSessionId = rouletteGameSessionId
            });
            context.SaveChanges();
            }

            // ── 20. RUSSIAN ROULETTE LOBBY + PLAYERS + ROUNDS ────────────────────
            if (!context.RussianRouletteLobbies.Any())
            {
            var now = DateTime.UtcNow;
            var user = context.Users.First();
            var animal = context.Animals.First();
            var rrConfig = context.RussianRouletteGameConfigs.First();
            var rrGame = context.Games.First(g => g.GameType == "RussianRoulette");

            var rrGameSessionId = Guid.NewGuid();
            context.GameSessions.Add(new GameSession
            {
            Id = rrGameSessionId,
            Result = "Win",
            MoneyEarned = 1000m,
            Multiplier = 1.0m,
            PlayedAt = now.AddMinutes(-20).ToString(),
            UserId = user.Id,
            AnimalId = animal.Id,
            GameId = rrGame.Id
            });
            context.SaveChanges();

            var lobbyId = Guid.NewGuid();
            var player1Id = Guid.NewGuid();
            var player2Id = Guid.NewGuid();
            var player3Id = Guid.NewGuid();

            context.RussianRouletteLobbies.Add(new RussianRouletteLobby
            {
            Id = lobbyId,
            LobbyCode = "RR-SEED-001",
            Status = "Finished",
            CurrentPrizePool = 1000m,
            WinnerId = user.Id,
            CreatedAt = now.AddMinutes(-30).ToString(),
            StartedAt = now.AddMinutes(-25).ToString(),
            FinishedAt = now.AddMinutes(-20).ToString(),
            GameSessionId = rrGameSessionId,
            RussianRouletteGameConfigId = rrConfig.Id,
            RussianRoulettePlayers = new List<RussianRoulettePlayer>
            {
            new() { Id = player1Id, IsBot = false, BotName = null,         TurnOrder = 1, IsAlive = true,  IsWinner = true,  EliminatedAt = null,                           JoinedAt = now.AddMinutes(-29).ToString(), UserId = user.Id, LobbyId = lobbyId },
            new() { Id = player2Id, IsBot = true,  BotName = "Bot_Shadow", TurnOrder = 2, IsAlive = false, IsWinner = false, EliminatedAt = now.AddMinutes(-23).ToString(), JoinedAt = now.AddMinutes(-29).ToString(), UserId = null,    LobbyId = lobbyId },
            new() { Id = player3Id, IsBot = true,  BotName = "Bot_Viper",  TurnOrder = 3, IsAlive = false, IsWinner = false, EliminatedAt = now.AddMinutes(-21).ToString(), JoinedAt = now.AddMinutes(-29).ToString(), UserId = null,    LobbyId = lobbyId },
            },
            RussianRouletteRounds = new List<RussianRouletteRound>
            {
            new() { Id = Guid.NewGuid(), RoundNumber = 1, WasBullet = false, PlayedAt = now.AddMinutes(-24).ToString(), LobbyId = lobbyId, PlayerId = player1Id },
            new() { Id = Guid.NewGuid(), RoundNumber = 2, WasBullet = true,  PlayedAt = now.AddMinutes(-23).ToString(), LobbyId = lobbyId, PlayerId = player2Id },
            new() { Id = Guid.NewGuid(), RoundNumber = 3, WasBullet = false, PlayedAt = now.AddMinutes(-22).ToString(), LobbyId = lobbyId, PlayerId = player1Id },
            new() { Id = Guid.NewGuid(), RoundNumber = 4, WasBullet = true,  PlayedAt = now.AddMinutes(-21).ToString(), LobbyId = lobbyId, PlayerId = player3Id },
            }
            });
            context.SaveChanges();
            }

            // ── 21. ANIMAL SHOP ───────────────────────────────────────────────────
            if (!context.AnimalShops.Any())
            {
            var now = DateTime.UtcNow.ToString();
            context.AnimalShops.AddRange(new List<AnimalShop>
            {
            new() { Id = Guid.NewGuid(), ShopName = "Yaguete Palace", Description = "El mercado oficial de animales del Casino Chaos", CreatedAt = now },
            new() { Id = Guid.NewGuid(), ShopName = "Exotic Beasts",  Description = "Animales exóticos para apostadores de élite",    CreatedAt = now },
            });
            context.SaveChanges();
            }

            // ── 22. SHOP ANIMAL LISTINGS ──────────────────────────────────────────
            if (!context.ShopAnimalListings.Any())
            {
            var now = DateTime.UtcNow;
            var shop = context.AnimalShops.First();
            var animals = context.Animals.ToList();

            context.ShopAnimalListings.AddRange(new List<ShopAnimalListing>
            {
            new() { Id = Guid.NewGuid(), ListingPrice = 1500m, IsSold = false, ListedAt = now.ToString(),              SoldAt = null,                       AnimalId = animals.First(a => a.Name == "Rex").Id,      AnimalShopId = shop.Id },
            new() { Id = Guid.NewGuid(), ListingPrice = 400m,  IsSold = false, ListedAt = now.ToString(),              SoldAt = null,                       AnimalId = animals.First(a => a.Name == "Whiskers").Id, AnimalShopId = shop.Id },
            new() { Id = Guid.NewGuid(), ListingPrice = 9000m, IsSold = true,  ListedAt = now.AddDays(-5).ToString(), SoldAt = now.AddDays(-2).ToString(), AnimalId = animals.First(a => a.Name == "Thunder").Id,  AnimalShopId = shop.Id },
            new() { Id = Guid.NewGuid(), ListingPrice = 6500m, IsSold = false, ListedAt = now.ToString(),              SoldAt = null,                       AnimalId = animals.First(a => a.Name == "Jaws").Id,     AnimalShopId = shop.Id },
            new() { Id = Guid.NewGuid(), ListingPrice = 75m,   IsSold = false, ListedAt = now.ToString(),              SoldAt = null,                       AnimalId = animals.First(a => a.Name == "Fluffy").Id,   AnimalShopId = shop.Id },
            });
            context.SaveChanges();
            }

            // ── 23. SHOP TRANSACTIONS ─────────────────────────────────────────────
            if (!context.ShopTransactions.Any())
            {
            var now = DateTime.UtcNow;
            var users = context.Users.ToList();
            var animals = context.Animals.ToList();

            context.ShopTransactions.AddRange(new List<ShopTransaction>
            {
            new() { Id = Guid.NewGuid(), TransactionType = "Buy",          Amount = 1500m, TransactionDate = now.AddDays(-3).ToString(), UserId = users[0 % users.Count].Id, AnimalId = animals.First(a => a.Name == "Rex").Id      },
            new() { Id = Guid.NewGuid(), TransactionType = "SellToCasino", Amount = 9000m, TransactionDate = now.AddDays(-2).ToString(), UserId = users[2 % users.Count].Id, AnimalId = animals.First(a => a.Name == "Thunder").Id  },
            new() { Id = Guid.NewGuid(), TransactionType = "Buy",          Amount = 400m,  TransactionDate = now.AddDays(-1).ToString(), UserId = users[1 % users.Count].Id, AnimalId = animals.First(a => a.Name == "Whiskers").Id },
            });
            context.SaveChanges();
            }

            // ── 24. SVG TO URL MIGRATION ──────────────────────────────────────────
            MigrateSvgsToUrls(context);
        }

        private static void MigrateSvgsToUrls(CasinoDBContext context)
        {
            var dbConfigs = context.AnimalValueConfigs.ToList();
            var baseDir = AppContext.BaseDirectory;
            var webRootPath = FindWebRootPath(baseDir);
            bool anyUpdated = false;

            foreach (var config in dbConfigs)
            {
                bool configUpdated = false;

                // Migrate normal image if it's empty, null, or contains inline SVG (starts with <)
                if (string.IsNullOrEmpty(config.ImageUrlNormal) || config.ImageUrlNormal.Trim().StartsWith("<"))
                {
                    var found = FindNormalImage(config.AnimalType, webRootPath);
                    if (found != null)
                    {
                        config.ImageUrlNormal = found;
                        configUpdated = true;
                    }
                }

                // Migrate mecha image if it's empty, null, or contains inline SVG (starts with <)
                if (string.IsNullOrEmpty(config.ImageUrlMecha) || config.ImageUrlMecha.Trim().StartsWith("<"))
                {
                    var found = FindMechaImage(config.AnimalType, webRootPath);
                    if (found != null)
                    {
                        config.ImageUrlMecha = found;
                        configUpdated = true;
                    }
                }

                if (configUpdated)
                {
                    anyUpdated = true;
                }
            }

            if (anyUpdated)
            {
                context.SaveChanges();
            }
        }

        private static string FindWebRootPath(string startDir)
        {
            var dir = new DirectoryInfo(startDir);
            while (dir != null)
            {
                var wwwroot = Path.Combine(dir.FullName, "Chaos", "wwwroot");
                if (Directory.Exists(wwwroot))
                {
                    return wwwroot;
                }

                wwwroot = Path.Combine(dir.FullName, "wwwroot");
                if (Directory.Exists(wwwroot))
                {
                    return wwwroot;
                }
                dir = dir.Parent;
            }
            return null;
        }

        private static string FindNormalImage(string animalType, string webRootPath)
        {
            if (string.IsNullOrEmpty(webRootPath)) return null;
            var dir = Path.Combine(webRootPath, "images", "animals");
            if (!Directory.Exists(dir)) return null;

            var files = Directory.GetFiles(dir, "*.svg");
            var target = animalType.ToLower().Replace(" ", "");

            // 1. Try exact match case insensitive
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file).ToLower();
                if (name == target)
                {
                    return $"/images/animals/{Path.GetFileName(file)}";
                }
            }

            // 2. Try normalized name matching (excluding mecha)
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file).ToLower();
                if (name.Replace("mecha", "") == target && !name.Contains("mecha"))
                {
                    return $"/images/animals/{Path.GetFileName(file)}";
                }
            }

            // 3. Fallback: starts/ends or contains
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file).ToLower();
                if (name.Contains(target) && !name.Contains("mecha"))
                {
                    return $"/images/animals/{Path.GetFileName(file)}";
                }
            }

            return null;
        }

        private static string FindMechaImage(string animalType, string webRootPath)
        {
            if (string.IsNullOrEmpty(webRootPath)) return null;
            var dir = Path.Combine(webRootPath, "images", "animals");
            if (!Directory.Exists(dir)) return null;

            var files = Directory.GetFiles(dir, "*.svg");
            var target = animalType.ToLower().Replace(" ", "");

            // 1. Try exact "mecha" + target or target + "mecha"
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file).ToLower();
                if (name == $"mecha{target}" || name == $"{target}mecha")
                {
                    return $"/images/animals/{Path.GetFileName(file)}";
                }
            }

            // 2. Contains both target and mecha
            foreach (var file in files)
            {
                var name = Path.GetFileNameWithoutExtension(file).ToLower();
                if (name.Contains(target) && name.Contains("mecha"))
                {
                    return $"/images/animals/{Path.GetFileName(file)}";
                }
            }

            return null;
        }
    }
}
