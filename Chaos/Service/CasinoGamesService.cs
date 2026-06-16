using Chaos.Api.Enums;
using Chaos.Api.Enums.EnumsEuropeanRoulette;
using Chaos.Api.Interface;
using Chaos.Api.Models;
using Chaos.Api.RequestEntity;
using Chaos.Api.ResponseEntity;
using Chaos.Infraestructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Reflection.Metadata;
using static Bogus.Person.CardAddress;

namespace Chaos.Api.Service
{
    public class CasinoGamesService : ICasinoGamesService
    {
        private readonly IAnimalService _animal;
        public Random _random = Random.Shared;
        private readonly IUserService _user;
        private readonly IDeckService _deckService;
        private readonly CasinoDBContext _dbContext;

        private readonly ICoinFlipSessionService _coinFlipSessionService;
        private readonly IGameSessionervice _GameSessionervice;
        private readonly IBlackjackSessionService _blackjackSessionService;
        private readonly IHigherLowerSessionService _higherLowerSessionService;
        
        private readonly IActiveDrinkEffectService _activeDrinkEffectService;

        //nuevos servicios
        private readonly ISlotSessionService _slotSessionService;


        private static readonly ConcurrentDictionary<Guid, BlackJackGame> _activeJackBlackGames = new();
        private static readonly ConcurrentDictionary<Guid, IDeckService> _gameDecks = [];
        private static readonly ConcurrentDictionary<Guid, HigherLowerGame> _activeHigherLowerGames = [];
        private static readonly ConcurrentDictionary<Guid, IDeckService> _higherLowerGameDecks = [];

        public CasinoGamesService(
            IAnimalService animal,
            IUserService user,
            IDeckService deckService,
            CasinoDBContext dbContext,
            ICoinFlipSessionService coinFlipSessionService,
            IGameSessionervice GameSessionervice,
            IBlackjackSessionService blackjackSessionService,
            IHigherLowerSessionService higherLowerSessionService,
            ISlotSessionService slotSessionService,
            IActiveDrinkEffectService activeDrinkEffectService)
            

        {
            _animal = animal;
            _user = user;
            _deckService = deckService;
            _dbContext = dbContext;
            _coinFlipSessionService = coinFlipSessionService;
            _GameSessionervice = GameSessionervice;
            _blackjackSessionService = blackjackSessionService;
            _higherLowerSessionService = higherLowerSessionService;
            _slotSessionService = slotSessionService;
            _activeDrinkEffectService = activeDrinkEffectService;
        }

        public async Task<bool> CashTransactionAsync(Guid WalletId, int AddAmount)
        {
            return await _user.AddIntoWallet(WalletId, AddAmount);
        }

        // Method that checks if an animal exists by its ID, returning true if it exists and false if it doesn't
        public bool ExistAnimal(Guid id)
        {
            Animal animal = _dbContext.Animals.FirstOrDefault(Animal => Animal.Id == id)!;
            return animal != null;
        }

        //Depende si es raro su valor para la apusta aumenta
        public decimal CalculateMultiplyerByRaririty(bool v)
        {
            if (v == true) return 2;
            return 1;
        }
        #region Symbols
        public List<SlotSymbol> GetSymbolsFromDB()
        {
            return _dbContext.SlotSymbols.ToList();
        }
        #endregion

        #region tragaperrillas
        /// <summary> slot machine game, the player can play 5 times and if 
        /// all the numbers in the slot are the same, they win 10 times the value of the animal </summary>
        public async Task<Tragaperras> PlayTragaPerrillas(CoinGameRequest Coin)
        {
            Animal animal = await _dbContext.Animals.FirstOrDefaultAsync(a => a.Id == Coin.AnimalId)!;
            User user = await _dbContext.Users.FirstOrDefaultAsync(a => a.Id == animal.OwnerId)!;
            Tragaperras Game = CreateTragaperras(Coin);

            for (int i = 1; i <= Game.Config.PayLines; i++)
            {
                string[,] MachineValues = RunSlotMachine(
                    Game.Config.NumberOfRows,
                    Game.Config.NumberOfReels,
                    Game.SlotsSymbols);
                Game.ListMachines.Add(MachineValues);
            }

            Game.AllRewards = WinCombination(Game.ListMachines);
            decimal Reward = AddRewardsToWallet(Game.AllRewards, Game, animal.EstimatedValue);
            user.Wallet += Reward;
            Game.Reward = Reward;

            var gameEntity = await _dbContext.Games
                .FirstOrDefaultAsync(g => g.GameType.ToLower() == "slots");

            if (gameEntity == null)
                throw new Exception("No se encontró un juego de tipo 'Slots' en la tabla Games");

            //Crear GameSession
            var GameSession = new GameSession
            {
                Id = Guid.NewGuid(),
                GameId = gameEntity.Id,
                Result = Reward > 0 ? "WIN" : "LOSE",
                MoneyEarned = Reward,
                Multiplier = (int)Game.Config.Multiplier,
                PlayedAt = DateTime.UtcNow.ToString(),
                UserId = animal.OwnerId ?? Guid.Empty,
                AnimalId = animal.Id
            };

            // serializar los símbolos ganadores a string
            string winningSymbolsJson = System.Text.Json.JsonSerializer
                .Serialize(Game.AllRewards);

            //Crear SlotSession
            var slotSession = new SlotSession
            {
                Id = Guid.NewGuid(),
                BetAmount = animal.EstimatedValue,
                WinningSymbols = winningSymbolsJson,
                GameSessionId = GameSession.Id,
                SlotGameConfigId = Game.Config.Id
            };

            await _GameSessionervice.AddGameSession(GameSession);
            await _slotSessionService.AddSession(slotSession);

            _dbContext.Users.Update(user);
            await _dbContext.SaveChangesAsync();

            return Game;
        }

        private Tragaperras CreateTragaperras(CoinGameRequest Coin)
        {
            Animal Animal = _dbContext.Animals.FirstOrDefault(a => a.Id == Coin.AnimalId)!;
            Tragaperras Game = new()
            {
                Config = new SlotGameConfig(),
                SlotsSymbols = []
            };
            Game.game = _dbContext.Games.FirstOrDefault(Game => Game.GameName == "Slots-Common")!;
            Game.Config = _dbContext.SlotGameConfigs.FirstOrDefault(g => g.MachineName == Coin.GameName)!;
            Game.SlotsSymbols = [.. _dbContext.SlotSymbols];
            return Game;
        }

        private decimal AddRewardsToWallet(List<string> Rewards, Tragaperras Game, decimal ValueAnimal)
        {
            decimal totalReward = 0;
            foreach (string Reward in Rewards)
            {
                var Symbol = Game.SlotsSymbols.Find(x => x.SymbolCode == Reward);
                if (Symbol != null) totalReward += Symbol.BaseValue;

            }
            return (totalReward * ValueAnimal) * Game.Config.Multiplier;
        }

        private string[,] RunSlotMachine(int numberOfRows, int numberOfReels, List<SlotSymbol> symbols)
        {
            string[,] MachineValues = new string[numberOfRows, numberOfReels];
            for (int j = 0; j < numberOfRows; j++)
            {
                for (int k = 0; k < numberOfReels; k++)
                {
                    string valuePosition = symbols[_random.Next(0, symbols.Count)].SymbolCode;
                    MachineValues[j, k] = valuePosition;
                }

            }
            return MachineValues;
        }

        private static List<string> WinCombination(List<string[,]> Machines)
        {
            List<string> rewards = [];
            string[] winConditon = new string [3];
            int rewindCounter = 0;
            foreach (string[,] machine in Machines)
            {
                int cols = machine.GetLength(0);
                int rows = machine.GetLength(1);

                for (int i = 0; i < cols; i++)
                {
                   
                    winConditon[rewindCounter] = machine[i, 0];
                    
                    for (int j = 1; j < rows; j++)
                    {
                        if (rewindCounter >= 2  &&  RewindRow(winConditon))
                         {
                        rewards.Add(winConditon[0]);
                          }
                        if (machine[i, j] != winConditon[rewindCounter])
                        {
                            winConditon[0] = machine[i, j];
                            rewindCounter = 0;
                            break; 
                        }
                        else
                        {
                            winConditon[++rewindCounter] = machine[i, j];
                        }
                    } 
                }
            }

            return rewards;
        }

        private static bool RewindRow(string[] winConditon)
        {
            for (int i = 1; i < winConditon.Length; i++)
            {
                if(winConditon[i] != winConditon[i -1]) return false; 
            }
            return true;
        }

        public List<string[][]> ConvertToJagged(List<string[,]> matrices)
        {
            var result = new List<string[][]>();

            foreach (var array in matrices)
            {
                int rows = array.GetLength(0);
                int cols = array.GetLength(1);

                var jagged = new string[rows][];

                for (int i = 0; i < rows; i++)
                {
                    jagged[i] = new string[cols];

                    for (int j = 0; j < cols; j++)
                    {
                        jagged[i][j] = array[i, j];
                    }
                }

                result.Add(jagged);
            }

            return result;
        }



        #endregion

        #region coingame


        // Executes a coin flip game using an animal, calculating win probability, reward, and multiplier.
        // Validates the animal, determines the result randomly, and stores the session in the database.
        // If the player wins, they receive a reward; otherwise, the animal is removed.
        // Returns the game result with details such as outcome, reward, and message.

        public async Task<CoinGame> CoinGame(Guid id, Guid playerId, string userChoice)
        {
            CoinGame game = new();

            var animal = await _animal.GetAnimalById(id);

            if (animal == null)
            {
                game.Won = false;
                game.Message = $"Animal with ID {id} not found. Please check the ID and try again.";
                game.AnimalName = "Not Found";
                game.AnimalValue = 0;
                game.Reward = 0;
                game.Possibility = 0;
                return game;
            }

            if (animal.EstimatedValue <= 0)
            {
                game.Won = false;
                game.Message = $"Your animal value is to low.";
                game.AnimalName = "Not Found";
                game.AnimalValue = 0;
                game.Reward = 0;
                game.Possibility = 0;
                return game;
            }

            if (animal.OwnerId != playerId)
            {
                game.Won = false;
                game.Message = $"{animal.Name} is not your animal!";
                game.AnimalName = "Not Found";
                game.AnimalValue = 0;
                game.Reward = 0;
                game.Possibility = 0;
                return game;
            }

            if (animal.IsAvailable == false)
            {
                game.Won = false;
                game.Message = $"Your animal {animal.Name} is dead, you can't play with him, it's ugly to do that.";
                game.AnimalName = "Not Found";
                game.AnimalValue = 0;
                game.Reward = 0;
                game.Possibility = 0;
                return game;
            }

            int animalValue = (int)animal.EstimatedValue;

            game.GameId = Guid.NewGuid();
            game.AnimalName = animal.Name;
            game.AnimalValue = animalValue;
            game.Reward = animalValue * (int)CalculateMultiplyerByRaririty(animal.Rarity);
            game.Multiplyer = CalculateMultiplyerByRaririty(animal.Rarity);
            game.Possibility = CalculateWinPossibility(animalValue);

            // ─────────────────────────────────────────────────────
            // 🍸 DRINK EFFECT — GUARANTEED_WIN
            // Check BEFORE the random roll so the effect can override it.
            // ConsumeRoundAsync returns true if the effect was active
            // and deducts 1 round from it automatically.
            // ─────────────────────────────────────────────────────
            bool hasGuaranteedWin = await _activeDrinkEffectService
                .ConsumeRoundAsync(playerId, DrinkEffectTypes.GuaranteedWin);

            int numRandom = _random.Next(101);
            game.Won = hasGuaranteedWin || numRandom < game.Possibility;
            // ─────────────────────────────────────────────────────

            // ─────────────────────────────────────────────────────
            // 🍸 DRINK EFFECT — DOUBLE_REWARDS
            // Check AFTER win is determined but BEFORE reward is used
            // anywhere, so the doubled value flows through correctly.
            // ─────────────────────────────────────────────────────
            bool hasDoubleReward = await _activeDrinkEffectService
                .ConsumeRoundAsync(playerId, DrinkEffectTypes.DoubleRewards);

            if (hasDoubleReward)
                game.Reward *= 2;
            // ─────────────────────────────────────────────────────

            string coinResult = game.Won ? "WIN" : "LOSE";

            var gameConfig = await _dbContext.CoinFlipGameConfigs
                    .FirstOrDefaultAsync(g => g.IsActive)
                    ?? await _dbContext.CoinFlipGameConfigs.FirstAsync();

            var gameEntity = await _dbContext.Games
                .FirstOrDefaultAsync(g => g.GameType.ToLower() == "coinflip");

            if (gameEntity == null)
                throw new Exception("No se encontró un juego de tipo 'CoinFlip' en la tabla Games");

            var GameSession = new GameSession
            {
                Id = Guid.NewGuid(),
                Result = coinResult,
                MoneyEarned = game.Won ? game.Reward : 0,
                Multiplier = 0,
                PlayedAt = DateTime.UtcNow.ToString(),
                UserId = playerId,
                AnimalId = id,
                GameId = gameEntity.Id
            };

            var session = new CoinFlipSession
            {
                Id = Guid.NewGuid(),
                AnimalId = id,
                UserId = playerId,
                MoneyEarned = game.Won ? game.Reward : 0,
                CoinResult = coinResult,
                UserChoice = userChoice,
                PlayedAt = DateTime.UtcNow.ToString(),
                PrizeMultiplierUsed = CalculateMultiplyerByRaririty(animal.Rarity),
                WinProbabilityUsed = game.Possibility,
                IsWin = game.Won,
                GameSessionId = GameSession.Id,
                CoinFlipGameConfigId = gameConfig.Id
            };

            await _GameSessionervice.AddGameSession(GameSession);
            await _coinFlipSessionService.AddSession(session);

            if (game.Won)
            {
                game.Message = hasGuaranteedWin
                    ? $"🍸 YAG TONIC guaranteed your win! {animal.Name} is safe and you receive {game.Reward}!"
                    : $"You win, {animal.Name} will not be sacrificed and you receive {game.Reward}";

                if (hasDoubleReward)
                    game.Message += " 🍹 YAGARETTO doubled your reward!";

                bool actionOfTransaction = await CashTransactionAsync(playerId, game.Reward);

                if (!actionOfTransaction)
                    throw new Exception("Wallet no encontrada para playerId");
            }
            else
            {
               
                bool hasPreventLoss = await _activeDrinkEffectService
                    .ConsumeRoundAsync(playerId, DrinkEffectTypes.PreventLoss);

                if (hasPreventLoss)
                {
                    game.Won = true; 
                    game.Reward = 0; 
                    game.Message = $"🍸 YAGIBU protected {animal.Name}! " +
                                   $"Your animal survived, but no reward this time.";
                }
                else
                {
                    game.Message = $"You lose, {animal.Name} will be sacrificed";
                    await _animal.RemoveAnimal(id);
                }
            }

            await _animal.UpdateAnimalShop();

            return game;
        }

        // Calculates the win probability based on the animal value using an inverse scale.
        public int CalculateWinPossibility(int animalValue)
        {
            if (animalValue < 0)
                throw new ArgumentException("animalValue no puede ser negativo");

            int max = 3000000;

            animalValue = Math.Min(animalValue, max);

            double probability;

            if (animalValue <= 100000)
            {
                probability = 70 - (animalValue * 10.0 / 100000);
            }
            else
            {
                double remaining = animalValue - 100000;
                double range = max - 100000;

                probability = 60 - (remaining * 40.0 / range);
            }

            return (int)Math.Round(Math.Clamp(probability, 20, 70));
        }


        #endregion

        //#region russianroulette


        //public List<RoundEliminated> PlayRussianRoullete(List<User> russianRoulletePlayers, Guid lobbyId)
        //{
        //    // ─────────────────────────────────────────
        //    //CONFIG DESDE BD
        //    // ─────────────────────────────────────────
        //    var lobby = _dbContext.RussianRouletteLobbies
        //        .Include(l => l.RussianRouletteGameConfig)
        //        .Include(l => l.RussianRoulettePlayers)
        //        .FirstOrDefault(l => l.Id == lobbyId)
        //        ?? throw new KeyNotFoundException($"Lobby {lobbyId} not found");

        //    var config = lobby.RussianRouletteGameConfig
        //        ?? throw new Exception("No config found for this lobby");

        //    int totalChambers = config.TotalChambers;
        //    int bulletCount = config.BulletCount;
        //    decimal prizePool = lobby.CurrentPrizePool;

        //    int round = 1;
        //    List<RoundEliminated> roundEliminated = [];

        //    // ─────────────────────────────────────────
        //    // LÓGICA DEL JUEGO
        //    // ─────────────────────────────────────────
        //    while (russianRoulletePlayers.Count > 1)
        //    {
        //        bool wasBullet = _random.Next(totalChambers) < bulletCount;

        //        if (wasBullet)
        //        {
        //            int eliminatedIndex = _random.Next(russianRoulletePlayers.Count);
        //            User eliminated = russianRoulletePlayers[eliminatedIndex];

        //            roundEliminated.Add(new RoundEliminated
        //            {
        //                RoundEliminatedPlayer = round,
        //                IdEliminatedPlayer = eliminated.Id
        //            });

        //            russianRoulletePlayers.RemoveAt(eliminatedIndex);
        //        }

        //        round++;
        //    }

        //    // ─────────────────────────────────────────
        //    //GANADOR
        //    // ─────────────────────────────────────────
        //    User winner = russianRoulletePlayers.First();

        //    User winnerInDb = _dbContext.Users.Find(winner.Id)
        //        ?? throw new Exception($"Winner {winner.Id} not found");

        //    winnerInDb.Wallet += prizePool;

        //    roundEliminated.Add(new RoundEliminated
        //    {
        //        RoundEliminatedPlayer = -1,
        //        IdEliminatedPlayer = winner.Id
        //    });

        //    // ─────────────────────────────────────────
        //    // GUARDAR RONDAS EN BD
        //    // ─────────────────────────────────────────
        //    foreach (var eliminatedRound in roundEliminated)
        //    {
        //        // Buscar el RussianRoulettePlayer que corresponde al User
        //        var rrPlayer = lobby.RussianRoulettePlayers
        //            .FirstOrDefault(p => p.UserId == eliminatedRound.IdEliminatedPlayer);

        //        if (rrPlayer != null)
        //        {
        //            // Marcar como eliminado si no es el ganador
        //            if (eliminatedRound.RoundEliminatedPlayer != -1)
        //            {
        //                rrPlayer.IsAlive = false;
        //                rrPlayer.EliminatedAt = DateTime.UtcNow.ToString();
        //            }

        //            // Guardar la ronda
        //            _dbContext.RussianRouletteRounds.Add(new RussianRouletteRound
        //            {
        //                Id = Guid.NewGuid(),
        //                LobbyId = lobbyId,
        //                PlayerId = rrPlayer.Id, //ID del RussianRoulettePlayer, NO del User
        //                PlayedAt = DateTime.UtcNow.ToString()
        //            });
        //        }
        //    }

        //    // ─────────────────────────────────────────
        //    //ACTUALIZAR LOBBY Y GUARDAR TODO
        //    // ─────────────────────────────────────────
        //    lobby.Status = "Finished";
        //    lobby.WinnerId = winner.Id;
        //    lobby.FinishedAt = DateTime.UtcNow.ToString();

        //    _dbContext.SaveChanges();

        //    _animal.UpdateAnimalShop();
        //    return roundEliminated;
        //}

        ////PlayerShooted → ELIMINADO, reemplazado por:
        //// bool wasBullet = _random.Next(totalChambers) < bulletCount;

        //#endregion

        #region blackjack


        // Initializes a Blackjack game for a user using a selected animal, dealing initial cards and checking for instant results.
        // Manages game state in memory if not finished and calculates reward based on rarity.
        // Creates and stores initial game session data in the database.
        // Returns the game object with current state and details.
        public async Task<BlackJackGame> BlackJackGame(Guid AnimalId, Guid UserId)
        {

            var animal = await _animal.GetAnimalById(AnimalId);
            var user = _user.GetUserById(UserId);

            var game = new BlackJackGame
            {
                GameId = Guid.NewGuid(),
                AnimalName = animal.Name,
                AnimalValue = (int)animal.EstimatedValue,
                AnimalId = AnimalId,
                UserId = UserId,
                Reward = (int)animal.EstimatedValue * (int)CalculateMultiplyerByRaririty(animal.Rarity),
                Status = BlackJackGameStatusEnum.PLAYERTURN,
            };

            if (animal == null)
            {
                game.UserId = UserId;
                game.AnimalId = AnimalId;
                game.Message = $"{AnimalId} not found!";
                game.AnimalName = "Not Found";
                game.AnimalValue = 0;
                game.Reward = 0;
                game.Status = BlackJackGameStatusEnum.FINISHED;
                return game;
            }

            if (animal.OwnerId != UserId)
            {
                game.UserId = UserId;
                game.AnimalId = AnimalId;
                game.Message = $"{animal.Name} is not your animal!";
                game.AnimalName = "Not Found";
                game.AnimalValue = 0;
                game.Reward = 0;
                game.Status = BlackJackGameStatusEnum.FINISHED;
                return game;
            }

            if (animal.IsAvailable == false)
            {
                game.UserId = UserId;
                game.AnimalId = AnimalId;
                game.Message = $"Your animal {animal.Name} is dead, you can't play with him, it's ugly to do that.";
                game.AnimalName = "Not Found";
                game.AnimalValue = 0;
                game.Reward = 0;
                game.Status = BlackJackGameStatusEnum.FINISHED;
                return game;
            }

            DeckService deckService1 = new(Random.Shared);
            ((IDeckService)deckService1).InitializeDeck();
            ((IDeckService)deckService1).Shuffle();

            game.PlayerHand.AddCard(((IDeckService)deckService1).CatchCard());
            game.DealerHand.AddCard(((IDeckService)deckService1).CatchCard());
            game.PlayerHand.AddCard(((IDeckService)deckService1).CatchCard());
            game.DealerHand.AddCard(((IDeckService)deckService1).CatchCard());

            game.UserCardsToShow = game.PlayerHand.Cards
                .Select(c => new CardDto { Value = c.BlackjackValue })
                .ToList();

            game.DealerCardsToShow = game.DealerHand.Cards
                .Select(c => new CardDto { Value = c.BlackjackValue })
                .ToList();

            var blackjackGame = await _dbContext.Games
                .FirstAsync(g => g.GameType == "Blackjack");

            var config = await _dbContext.BlackjackGameConfigs.FirstAsync();

            var GameSession = new GameSession
            {
                Id = Guid.NewGuid(),
                GameId = blackjackGame.Id,
                Result = "STARTED",
                MoneyEarned = game.Result == BlackJackGameResultEnum.WIN ? game.Reward : 0,
                Multiplier = (int)CalculateMultiplyerByRaririty(animal.Rarity),
                PlayedAt = DateTime.UtcNow.ToString(),
                UserId = game.UserId,
                AnimalId = game.AnimalId,
            };

            var session = new BlackjackSession
            {
                Id = Guid.NewGuid(),
                Status = "STARTED",
                PlayerScore = game.PlayerScore,
                DealerScore = game.DealerScore,
                MoneyEarned = game.Result == BlackJackGameResultEnum.WIN ? game.Reward : 0,
                StartedAt = DateTime.UtcNow.ToString(),
                FinishedAt = null,
                UserId = game.UserId,
                AnimalId = game.AnimalId,
                GameSessionId = GameSession.Id,
                BlackjackGameConfigId = config.Id
            };

            game.GameSessionDbId = GameSession.Id;

            await _GameSessionervice.AddGameSession(GameSession);
            await _blackjackSessionService.AddSession(session);
            await _dbContext.SaveChangesAsync();

            if (game.PlayerHand.IsTwentyOne && game.PlayerHand.Cards.Count == 2)
            {
                if (game.DealerHand.IsTwentyOne && game.DealerHand.Cards.Count == 2)
                {
                    await FinishGame(game, BlackJackGameResultEnum.DRAW);
                }
                else
                {
                    await FinishGame(game, BlackJackGameResultEnum.WIN);
                }
            }
            else if (game.DealerHand.IsTwentyOne && game.DealerHand.Cards.Count == 2)
            {
                await FinishGame(game, BlackJackGameResultEnum.LOSE);
            }

            if (!game.IsFinished)
            {
                _activeJackBlackGames[game.GameId] = game;
                _gameDecks[game.GameId] = deckService1;
            }

            return game;
        }


        // Processes a "hit" action in an active Blackjack game, dealing a new card to the player.
        // Validates game state and turn, updating the player's hand and checking for bust or 21.
        // If needed, transitions to dealer turn or finishes the game.
        // Returns the updated game state.
        public async Task<BlackJackGame> BlackJackHit(Guid gameId)
        {

            if (!_activeJackBlackGames.TryGetValue(gameId, out var game))
            {
                throw new KeyNotFoundException($"Active game with ID {gameId} not found");
            }

            if (game.Status != BlackJackGameStatusEnum.PLAYERTURN)
            {
                throw new InvalidOperationException("Cannot hit. It's not player's turn");
            }

            if (!game.CanHit)
            {
                throw new InvalidOperationException("Cannot hit. Player is busted or has 21");
            }

            if (!_gameDecks.TryGetValue(gameId, out var deckServiceForGame))
            {
                throw new InvalidOperationException($"Deck service for game with ID {gameId} not found.");
            }

            Card newCard = deckServiceForGame.CatchCard();
            game.PlayerHand.AddCard(newCard);

            game.UserCardsToShow = game.PlayerHand.Cards
            .Select(c => new CardDto { Value = c.BlackjackValue })
            .ToList();

            if (game.PlayerBusted)
            {
                await FinishGame(game, BlackJackGameResultEnum.LOSE);
            }

            else if (game.PlayerScore == 21)
            {
                game.Status = BlackJackGameStatusEnum.DEALERTURN;
                await PlayDealerTurn(game, deckServiceForGame);
            }

            await _dbContext.SaveChangesAsync();
            return game;
        }


        // Processes a "stand" action in an active Blackjack game, ending the player's turn.
        // Validates the game state and retrieves the deck associated with the game.
        // Triggers the dealer's turn to determine the final outcome.
        // Returns the updated game state.
        public async Task<BlackJackGame> BlackJackStand(Guid gameId)
        {

            if (!_activeJackBlackGames.TryGetValue(gameId, out var game))
            {
                throw new KeyNotFoundException($"Active game with ID {gameId} not found");
            }

            if (game.Status != BlackJackGameStatusEnum.PLAYERTURN)
            {
                throw new InvalidOperationException("Cannot stand. It's not player's turn");
            }

            if (!_gameDecks.TryGetValue(gameId, out var deckServiceForGame))
            {
                throw new InvalidOperationException($"Deck service for game with ID {gameId} not found.");
            }

            game.Status = BlackJackGameStatusEnum.DEALERTURN;
            _ = _gameDecks[gameId];
            await PlayDealerTurn(game, deckServiceForGame);

            return game;
        }

        // Handles the dealer's turn in a Blackjack game, drawing cards until reaching at least 17 points.
        private async Task PlayDealerTurn(BlackJackGame game, IDeckService deck)
        {
            while (game.DealerScore < 17)
            {
                var card = deck.CatchCard();
                game.DealerHand.AddCard(card);
            }

            game.DealerCardsToShow = game.DealerHand.Cards
            .Select(c => new CardDto { Value = c.BlackjackValue })
            .ToList();

            await DetermineWinner(game);
        }


        // Determines the outcome of a Blackjack game by comparing player and dealer scores.
        // Evaluates bust conditions and score differences to decide win, lose, or draw.
        public async Task DetermineWinner(BlackJackGame game)
        {

            bool hasGuaranteedWin = await _activeDrinkEffectService
                .ConsumeRoundAsync(game.UserId, DrinkEffectTypes.GuaranteedWin);

            if (hasGuaranteedWin)
            {
                await FinishGame(game, BlackJackGameResultEnum.WIN);
                return; 
            }

            if (game.DealerBusted)
            {
                await FinishGame(game, BlackJackGameResultEnum.WIN);
            }
            else if (game.PlayerScore > game.DealerScore)
            {
                await FinishGame(game, BlackJackGameResultEnum.WIN);
            }
            else if (game.PlayerScore < game.DealerScore)
            {
                await FinishGame(game, BlackJackGameResultEnum.LOSE);
            }
            else
            {
                await FinishGame(game, BlackJackGameResultEnum.DRAW);
            }
        }


        // Finalizes a Blackjack game by setting the result and applying game logic based on the outcome.
        // Handles rewards, animal removal, and updates related session records in the database.
        public async Task FinishGame(BlackJackGame game, BlackJackGameResultEnum result)
        {
            game.Result = result;
            game.Status = BlackJackGameStatusEnum.FINISHED;

            var animal = _dbContext.Animals
                .FirstOrDefault(a => a.Id == game.AnimalId)
                ?? throw new KeyNotFoundException($"Animal with ID {game.AnimalId} not found");

            switch (result)
            {
                case BlackJackGameResultEnum.WIN:

                    // ─────────────────────────────────────────────────────
                    // 🍸 DRINK EFFECT — DOUBLE_REWARDS
                    // Applied here, inside the WIN case, BEFORE the cash
                    // transaction so the doubled amount is what gets paid.
                    // Also updates game.Reward so the session records the
                    // correct value below.
                    // ─────────────────────────────────────────────────────
                    bool hasDoubleReward = await _activeDrinkEffectService
                        .ConsumeRoundAsync(game.UserId, DrinkEffectTypes.DoubleRewards);

                    if (hasDoubleReward)
                        game.Reward *= 2;
                    // ─────────────────────────────────────────────────────

                    game.Message = $"You win! {game.AnimalName} will not be sacrificed and you receive {game.Reward} coins!";

                    if (hasDoubleReward)
                        game.Message += " 🍹 YAGARETTO doubled your reward!";

                    bool actionOfTransaction = await CashTransactionAsync(game.UserId, game.Reward);

                    if (actionOfTransaction == false)
                        throw new Exception($"Wallet no encontrada para playerId");

                    break;

                case BlackJackGameResultEnum.LOSE:

                    // ─────────────────────────────────────────────────────
                    // 🍸 DRINK EFFECT — PREVENT_LOSS
                    // Checked BEFORE RemoveAnimal so the animal can be
                    // spared. If active, we override the result to DRAW
                    // (animal safe, no reward) and skip the removal.
                    // ─────────────────────────────────────────────────────
                    bool hasPreventLoss = await _activeDrinkEffectService
                        .ConsumeRoundAsync(game.UserId, DrinkEffectTypes.PreventLoss);

                    if (hasPreventLoss)
                    {
                        game.Result = BlackJackGameResultEnum.DRAW; // treat as draw so no reward either
                        game.Message = $"🍸 YAGIBU protected {game.AnimalName}! " +
                                       $"Your animal survived, but no reward this time.";
                        result = BlackJackGameResultEnum.DRAW;      // keep result consistent for session records below
                    }
                    else
                    {
                        game.Message = $"You lose! {game.AnimalName} will be sacrificed.";

                        try
                        {
                            await _animal.RemoveAnimal(game.AnimalId);
                        }
                        catch (Exception) { }
                    }
                    // ─────────────────────────────────────────────────────

                    break;

                case BlackJackGameResultEnum.DRAW:
                    game.Message = $"Draw! Your bet is returned. {game.AnimalName} is safe.";
                    break;
            }

            var blackjackGame = await _dbContext.Games
                .FirstAsync(g => g.GameType == "Blackjack");

            var config = await _dbContext.BlackjackGameConfigs.FirstAsync();

            var GameSession = _dbContext.GameSessions
                .FirstOrDefault((System.Linq.Expressions.Expression<Func<GameSession, bool>>)(g => g.Id == game.GameSessionDbId));

            if (GameSession == null)
                throw new Exception($"GameSession NOT FOUND: {game.GameId}");

            var BlackSession = _dbContext.BlackjackSessions
                .FirstOrDefault(g => g.GameSessionId == GameSession.Id);

            if (BlackSession == null)
                throw new Exception($"HigherLowerSession NOT FOUND for GameSessionId: {GameSession.Id}");

            // game.Reward is already doubled if DOUBLE_REWARDS was active,
            // and result is already overridden if PREVENT_LOSS was active,
            // so these lines naturally record the correct final values.
            GameSession.Result = result.ToString();
            GameSession.MoneyEarned = result == BlackJackGameResultEnum.WIN ? game.Reward : 0;

            BlackSession.MoneyEarned = result == BlackJackGameResultEnum.WIN ? game.Reward : 0;
            BlackSession.Status = "FINISHED";
            BlackSession.FinishedAt = DateTime.UtcNow.ToString();
            BlackSession.DealerScore = game.DealerScore;
            BlackSession.PlayerScore = game.PlayerScore;

            await _dbContext.SaveChangesAsync();
            await _animal.UpdateAnimalShop();

            _activeJackBlackGames.TryRemove(game.GameId, out _);
            _gameDecks.TryRemove(game.GameId, out _);
        }


        // Retrieves an active Blackjack game from memory using its ID.
        // If the game exists, returns its current state.
        public BlackJackGame BlackJackGetGame(Guid gameId)
        {
            if (_activeJackBlackGames.TryGetValue(gameId, out var game))
            {
                return game;
            }
            else
            {
                throw new KeyNotFoundException($"Active game with ID {gameId} not found.");
            }
        }

        // Maps a BlackJackGame object to a BlackJackResponse DTO for client use.
        public BlackJackResponse MapToBlackJackResponse(BlackJackGame game)
        {
            return new BlackJackResponse
            {
                GameId = game.GameId,
                AnimalName = game.AnimalName,
                Reward = game.Reward,
                Message = game.Message,
                Result = game.IsFinished ? game.Result : null,
                PlayerScore = game.PlayerScore,
                IsFinished = game.IsFinished,
                DealerScore = game.IsFinished || game.Status == BlackJackGameStatusEnum.DEALERTURN ? game.DealerScore : game.DealerHand.Cards.FirstOrDefault()?.BlackjackValue ?? 0,
                DealerCards = game.DealerCardsToShow,
                UserCards = game.UserCardsToShow
            };


        }

        #endregion

        #region europeanroulette



        public MessageWarning CheckNotDuplicateAnimalAndAvailable(SelectionUserEuropeanRoulette Bet, Guid userId)

        {

            List<Guid> IdAnimals = new();

            foreach (var idAnimal in Bet.SelectedNumbers)

            {

                if (idAnimal != null)

                {

                    foreach (var value in idAnimal.Values)

                    {

                        if (value != Guid.Empty)

                            IdAnimals.Add(value);

                    }

                }

            }

            if (Bet.RedNumbers != Guid.Empty) IdAnimals.Add(Bet.RedNumbers);

            if (Bet.BlackNumbers != Guid.Empty) IdAnimals.Add(Bet.BlackNumbers);

            if (Bet.FirstDozen != Guid.Empty) IdAnimals.Add(Bet.FirstDozen);

            if (Bet.SecondDozen != Guid.Empty) IdAnimals.Add(Bet.SecondDozen);

            if (Bet.ThirdDozen != Guid.Empty) IdAnimals.Add(Bet.ThirdDozen);

            if (Bet.FirstHalf != Guid.Empty) IdAnimals.Add(Bet.FirstHalf);

            if (Bet.SecondHalf != Guid.Empty) IdAnimals.Add(Bet.SecondHalf);

            if (Bet.FirstRow != Guid.Empty) IdAnimals.Add(Bet.FirstRow);

            if (Bet.SecondRow != Guid.Empty) IdAnimals.Add(Bet.SecondRow);

            if (Bet.ThirdRow != Guid.Empty) IdAnimals.Add(Bet.ThirdRow);

            if (Bet.EvenNumbers != Guid.Empty) IdAnimals.Add(Bet.EvenNumbers);

            if (Bet.OddNumbers != Guid.Empty) IdAnimals.Add(Bet.OddNumbers);


            var uniqueValues = new HashSet<Guid>();

            foreach (var value in IdAnimals)

            {

                Animal checkAlive = _dbContext.Animals.Find(value) ?? null;

                if (checkAlive == null) return new MessageWarning { Disable = true, Message = "Sorry, but didn't find any animal with this credentials" };

                if (checkAlive != null && !checkAlive.IsAvailable) return new MessageWarning { Disable = true, Message = "The " + checkAlive.AnimalType + " " + checkAlive.Name + " is death" };

                if (checkAlive.OwnerId != userId) return new MessageWarning { Disable = true, Message = "The " + checkAlive.AnimalType + " " + checkAlive.Name + " it's not yours" };

                if (!uniqueValues.Add(value))

                {

                    if (checkAlive != null)

                    {

                        return new MessageWarning { Disable = true, Message = "The " + checkAlive.AnimalType + " " + checkAlive.Name + " is duplicated" };

                    }

                }

            }

            return new MessageWarning { Disable = false, Message = "Everything it's ok" };

        }

        public async Task<CashBack> PlayEuropeanRoullete(SelectionUserEuropeanRoulette Bet, Guid UserId)

        {

            int PositionRoullete = _random.Next(37);

            int ValuesBet;

            List<Guid> losingAnimals;

            if (PositionRoullete != 0)

            {

                ValuesBet = CheckValuesSelectedByUser(Bet, PositionRoullete);

                losingAnimals = GetLosingAnimals(Bet, PositionRoullete);

            }

            else

            {

                ValuesBet = GetValueZero(Bet.SelectedNumbers);

                losingAnimals = GetLosingAnimalsOnZero(Bet);

            }

            int totalBet = GroupBet(Bet);
            string message = BuildResultMessage(ValuesBet, totalBet, PositionRoullete);


           // marca animal perdedor como muerto -> isAvailable = false

            foreach (Guid animalId in losingAnimals)

            {

               Animal AnimalFromDB =   _dbContext.Animals.Find(animalId);

               if (AnimalFromDB != null) AnimalFromDB.IsAvailable = false;

            }

            var user = await _dbContext.Users.FindAsync(UserId)
         ?? throw new Exception("User not found");

            int profit = ValuesBet - totalBet;

            if (profit > 0)
                user.Wallet += profit;

            user.Wallet = profit > 0 ? user.Wallet + profit : user.Wallet;

            await _dbContext.SaveChangesAsync();

            var gameEntity = await _dbContext.Games
        .FirstOrDefaultAsync(g => g.GameType.ToLower() == "roulette")
        ?? throw new Exception("No se encontró un juego de tipo 'Roulette'");

            // Obtener config de la ruleta europea
            var rouletteConfig = await _dbContext.RouletteGameConfigs
                .FirstOrDefaultAsync(r => r.RouletteType == "European" && r.IsActive)
                ?? throw new Exception("No active European Roulette config found");

            // Obtener el primer animal apostado (para AnimalId de GameSession)
            Guid primaryAnimalId = Bet.SelectedNumbers
                .SelectMany(dict => dict.Values)
                .FirstOrDefault(guid => guid != Guid.Empty);

            if (primaryAnimalId == Guid.Empty)
            {
                primaryAnimalId = new[]
                {
            Bet.RedNumbers, Bet.BlackNumbers, Bet.FirstDozen,
            Bet.SecondDozen, Bet.ThirdDozen, Bet.FirstHalf,
            Bet.SecondHalf, Bet.FirstRow, Bet.SecondRow,
            Bet.ThirdRow, Bet.EvenNumbers, Bet.OddNumbers
        }
                .FirstOrDefault(g => g != Guid.Empty);
            }

            // Crear GameSession
            var gameSession = new GameSession
            {
                Id = Guid.NewGuid(),
                GameId = gameEntity.Id,
                Result = ValuesBet > totalBet ? "WIN" : "LOSE",
                MoneyEarned = profit > 0 ? profit : 0,
                Multiplier = 0,
                PlayedAt = DateTime.UtcNow.ToString(),
                UserId = UserId,
                AnimalId = primaryAnimalId
            };

            // Crear RouletteSession
            var rouletteSession = new RouletteSession
            {
                Id = Guid.NewGuid(),
                SpinResult = PositionRoullete,
                RouletteGameConfigId = rouletteConfig.Id,
                GameSessionId = gameSession.Id
            };

            _dbContext.GameSessions.Add(gameSession);
            _dbContext.RouletteSessions.Add(rouletteSession);
           
            await _dbContext.SaveChangesAsync();

            return new CashBack
            {
                Bet = totalBet,
                MoneyBack = ValuesBet,
                PositionEuropeanRoulette = PositionRoullete,
                ResultMessage = message
            };

        }

        private static string BuildResultMessage(int moneyBack, int bet, int position)

        {

            if (moneyBack > 0 && moneyBack - bet > 0)

            {

                return $"🎉 Congratulations! You get {moneyBack} on your wallet. " +

                       $"You bet {bet} and your profit is {moneyBack - bet}. " +

                       $"The roulette's ball stopped at {position}.";

            }

            else if (bet <= 0)

            {

                return $"The roulette's ball stopped at {position}.";

            }

            else

            {

                return $"😔 You lose {bet}. " +

                       $"The roulette's ball stopped at {position}.";

            }

        }

        private int GroupBet(SelectionUserEuropeanRoulette Bet)

        {

            int AllBet = 0;

            int totalSelectedNumbers = (int)Bet.SelectedNumbers

                .SelectMany(dict => dict.Values)

                .Select(guid => _animal.GetAnimalById(guid)?.Result.EstimatedValue ?? 0)

                .Sum();


            AllBet += totalSelectedNumbers;

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.RedNumbers.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.BlackNumbers.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.FirstDozen.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.SecondDozen.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.ThirdDozen.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.FirstHalf.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.SecondHalf.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.FirstRow.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.SecondRow.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.ThirdRow.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.EvenNumbers.ToString())).Result?.EstimatedValue ?? 0);

            AllBet += (int)(_animal.GetAnimalById(Guid.Parse(Bet.OddNumbers.ToString())).Result?.EstimatedValue ?? 0);

            return AllBet;

        }

        private int GetValueZero(List<Dictionary<int, Guid>> SelectedNumber)

        {

            int Value = 0;

            foreach (var number in SelectedNumber)

            {

                if (number.TryGetValue(0, out Guid SelectNumber))

                {

                    if (number != null)

                    {

                        Value += (int)(_animal.GetAnimalById(Guid.Parse(SelectNumber.ToString()))?.Result.EstimatedValue ?? 0) * 36;

                    }

                }

            }

            return Value;

        }

        private int CheckValuesSelectedByUser(SelectionUserEuropeanRoulette Bet, int PositionRoullete)

        {

            bool IsFirstRow = Enum.IsDefined(typeof(FirstRow), PositionRoullete);

            bool IsSecondRow = Enum.IsDefined(typeof(SecondRow), PositionRoullete);

            bool IsThirdRow = Enum.IsDefined(typeof(ThirdRow), PositionRoullete);

            bool IsOddNumber = PositionRoullete % 2 != 0;

            bool BlackOrRed = Enum.IsDefined(typeof(BlackNumbers), PositionRoullete);

            bool FirstHalf = PositionRoullete <= 16;

            bool IsFirtDozen = Enum.IsDefined(typeof(FirstDozen), PositionRoullete);

            bool IsSecondDozen = Enum.IsDefined(typeof(SecondDozen), PositionRoullete);

            bool IsThirdDozen = Enum.IsDefined(typeof(ThirdDozen), PositionRoullete);

            int Amount = 0;


            foreach (var number in Bet.SelectedNumbers)

            {

                if (number.TryGetValue(PositionRoullete, out Guid SelectNumber))

                {

                    if (number != null)

                    {

                        Amount += (int)(_animal.GetAnimalById(Guid.Parse(SelectNumber.ToString()))?.Result.EstimatedValue ?? 0) * 36;

                    }

                }

            }

            if (IsFirstRow && (_animal.GetAnimalById(Guid.Parse(Bet.FirstRow.ToString()))?.Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.FirstRow.ToString())).Result.EstimatedValue * 3; }

            if (IsSecondRow && (_animal.GetAnimalById(Guid.Parse(Bet.SecondRow.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.SecondRow.ToString())).Result.EstimatedValue * 3; }

            if (IsThirdRow && (_animal.GetAnimalById(Guid.Parse(Bet.ThirdRow.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.ThirdRow.ToString())).Result.EstimatedValue * 3; }

            if (IsOddNumber && (_animal.GetAnimalById(Guid.Parse(Bet.OddNumbers.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.OddNumbers.ToString())).Result.EstimatedValue * 2; }

            if (!IsOddNumber && (_animal.GetAnimalById(Guid.Parse(Bet.EvenNumbers.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.EvenNumbers.ToString())).Result.EstimatedValue * 2; }

            if (BlackOrRed && (_animal.GetAnimalById(Guid.Parse(Bet.BlackNumbers.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.BlackNumbers.ToString())).Result.EstimatedValue * 2; }

            if (!BlackOrRed && (_animal.GetAnimalById(Guid.Parse(Bet.RedNumbers.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.RedNumbers.ToString())).Result.EstimatedValue * 2; }

            if (FirstHalf && (_animal.GetAnimalById(Guid.Parse(Bet.FirstHalf.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.FirstHalf.ToString())).Result.EstimatedValue * 2; }

            if (!FirstHalf && (_animal.GetAnimalById(Guid.Parse(Bet.SecondHalf.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.SecondHalf.ToString())).Result.EstimatedValue * 2; }

            if (IsFirtDozen && (_animal.GetAnimalById(Guid.Parse(Bet.FirstDozen.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)(_animal.GetAnimalById(Guid.Parse(Bet.FirstDozen.ToString())).Result.EstimatedValue * 3); }

            if (IsSecondDozen && (_animal.GetAnimalById(Guid.Parse(Bet.SecondDozen.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.SecondDozen.ToString())).Result.EstimatedValue * 3; }

            if (IsThirdDozen && (_animal.GetAnimalById(Guid.Parse(Bet.ThirdDozen.ToString())).Result?.EstimatedValue ?? 0) > 0)

            { Amount += (int)_animal.GetAnimalById(Guid.Parse(Bet.ThirdDozen.ToString())).Result.EstimatedValue * 3; }

            return Amount;

        }


        private static List<Guid> GetLosingAnimals(SelectionUserEuropeanRoulette Bet, int PositionRoullete)
        {
            var losers = new List<Guid>();

            bool IsFirstRow = Enum.IsDefined(typeof(FirstRow), PositionRoullete);
            bool IsSecondRow = Enum.IsDefined(typeof(SecondRow), PositionRoullete);
            bool IsThirdRow = Enum.IsDefined(typeof(ThirdRow), PositionRoullete);
            bool IsOddNumber = PositionRoullete % 2 != 0;
            bool IsBlack = Enum.IsDefined(typeof(BlackNumbers), PositionRoullete);
            bool IsFirstHalf = PositionRoullete <= 16;
            bool IsFirstDozen = Enum.IsDefined(typeof(FirstDozen), PositionRoullete);
            bool IsSecondDozen = Enum.IsDefined(typeof(SecondDozen), PositionRoullete);
            bool IsThirdDozen = Enum.IsDefined(typeof(ThirdDozen), PositionRoullete);

            // SelectedNumbers: pierde si el número apostado NO coincide con la posición
            foreach (var number in Bet.SelectedNumbers)
            {
                // Si el diccionario NO tiene la posición ganadora, todos sus GUIDs pierden
                if (!number.ContainsKey(PositionRoullete))
                {
                    foreach (var guid in number.Values)
                    {
                        if (guid != Guid.Empty) losers.Add(guid);
                    }
                }
            }

            // Filas
            if (!IsFirstRow && Bet.FirstRow != Guid.Empty) losers.Add(Bet.FirstRow);
            if (!IsSecondRow && Bet.SecondRow != Guid.Empty) losers.Add(Bet.SecondRow);
            if (!IsThirdRow && Bet.ThirdRow != Guid.Empty) losers.Add(Bet.ThirdRow);

            // Par / Impar
            if (!IsOddNumber && Bet.OddNumbers != Guid.Empty) losers.Add(Bet.OddNumbers);
            if (IsOddNumber && Bet.EvenNumbers != Guid.Empty) losers.Add(Bet.EvenNumbers);

            // Negro / Rojo
            if (!IsBlack && Bet.BlackNumbers != Guid.Empty) losers.Add(Bet.BlackNumbers);
            if (IsBlack && Bet.RedNumbers != Guid.Empty) losers.Add(Bet.RedNumbers);

            // Mitades
            if (!IsFirstHalf && Bet.FirstHalf != Guid.Empty) losers.Add(Bet.FirstHalf);
            if (IsFirstHalf && Bet.SecondHalf != Guid.Empty) losers.Add(Bet.SecondHalf);

            // Docenas
            if (!IsFirstDozen && Bet.FirstDozen != Guid.Empty) losers.Add(Bet.FirstDozen);
            if (!IsSecondDozen && Bet.SecondDozen != Guid.Empty) losers.Add(Bet.SecondDozen);
            if (!IsThirdDozen && Bet.ThirdDozen != Guid.Empty) losers.Add(Bet.ThirdDozen);

            return losers;
        }

        private static List<Guid> GetLosingAnimalsOnZero(SelectionUserEuropeanRoulette Bet)
        {
            var losers = new List<Guid>();

            // Solo ganan los SelectedNumbers con clave 0, el resto pierde
            foreach (var number in Bet.SelectedNumbers)
            {
                if (!number.ContainsKey(0))
                {
                    foreach (var guid in number.Values)
                    {
                        if (guid != Guid.Empty) losers.Add(guid);
                    }
                }
            }
            return losers;
        }

        #endregion

        #region higherorlower


        // Initializes a Higher or Lower game, creating the game state and storing initial session data in the database.
        // Validates game configuration and animal availability, then prepares the deck and first cards.
        // Stores the active game and deck in memory for further rounds.
        public async Task<HigherLowerGame> StartHigherLowerGame(Guid AnimalId, Guid UserId)
        {



            var gameEntity = await _dbContext.Games.FirstOrDefaultAsync();


            if (gameEntity == null)
            {
                throw new Exception("GameType HigherLower not found in DB");
            }



            var config = await _dbContext.HigherLowerGameConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.IsActive);



            if (config == null)
            {
                throw new Exception("No active HigherLowerGameConfig found");
            }



            var GameSession = new GameSession
            {
                Id = Guid.NewGuid(),
                GameId = gameEntity.Id,
                Result = "Started",
                MoneyEarned = 0,
                Multiplier = 1,
                PlayedAt = DateTime.UtcNow.ToString(),
                UserId = UserId,
                AnimalId = AnimalId
            };



            var session = new HigherLowerSession
            {
                Id = Guid.NewGuid(),
                Status = "Started",
                TotalEarned = 0,
                RoundsPlayed = 0,
                StartedAt = DateTime.UtcNow.ToString(),
                FinishedAt = null,
                UserId = UserId,
                AnimalId = AnimalId,
                GameSessionId = GameSession.Id,
                HigherLowerGameConfigId = config.Id
            };



            var animal = await _animal.GetAnimalById(AnimalId) ?? throw new KeyNotFoundException($"Animal with ID {AnimalId} not found.");



            var game = new HigherLowerGame
            {
                GameId = Guid.NewGuid(),
                AnimalId = AnimalId,
                UserId = UserId,
                AnimalName = animal.Name,
                AnimalValue = (int)animal.EstimatedValue,
                Reward = 0,
                Choice = null,
                Message = "Guess if the next card will be HIGHER or LOWER!",
                GameSessionId = GameSession.Id,
                RoundsPLayed = 0
            };



            if (animal.OwnerId != UserId)
            {
                game.UserId = UserId;
                game.AnimalId = AnimalId;
                game.Message = $"{animal.Name} is not your animal!";
                game.AnimalName = "Not Found";
                game.AnimalValue = 0;
                game.Reward = 0;
                game.Choice = null;
                return game;
            }



            if (animal.IsAvailable == false)
            {
                game.UserId = UserId;
                game.AnimalId = AnimalId;
                game.Message = $"Your animal {animal.Name} is dead, you can't play with him, it's ugly to do that.";
                game.AnimalName = "Not Found";
                game.AnimalValue = 0;
                game.Reward = 0;
                game.Choice = null;
                return game;
            }



            await _GameSessionervice.AddGameSession(GameSession);
            await _higherLowerSessionService.AddSession(session);



            var deckServiceForGame = new DeckService(new Random());
            deckServiceForGame.InitializeDeck();
            deckServiceForGame.Shuffle();



            game.CurrentCard = deckServiceForGame.CatchCard();
            game.NextCard = deckServiceForGame.CatchCard();



            _activeHigherLowerGames[game.GameId] = game;
            _higherLowerGameDecks[game.GameId] = deckServiceForGame;



            Console.WriteLine($"Game created with ID: {game.GameId}");
            Console.WriteLine($"Active games count: {_activeHigherLowerGames.Count}");



            return game;
        }

        // Processes a Higher or Lower round, validating the game state and player choice.
        // Compares current and next card to determine if the player wins, updating reward and game state.
        // Handles deck exhaustion, cash out, or loss, including transactions and animal removal.
        // Updates session status in the database and returns the updated game.
        public async Task<HigherLowerGame> PlayHigherLower(HigherLowerPlayRequest request)
        {
            if (!_activeHigherLowerGames.TryGetValue(request.GameId, out var game))
                throw new KeyNotFoundException($"Active Higher/Lower game with ID {request.GameId} not found.");

            game.RoundsPLayed = game.RoundsPLayed + 1;

            if (game.GameEnded)
                throw new InvalidOperationException("This Higher/Lower game has already finished.");

            game.Choice = request.Choice;

            if (!_higherLowerGameDecks.TryGetValue(request.GameId, out var deckServiceForGame))
                throw new InvalidOperationException($"Deck for Higher/Lower game with ID {request.GameId} not found.");

            if (deckServiceForGame.RemainingCards == 0)
            {
                game.Message = "The deck is empty! Game ends. You automatically cash out.";
                game.GameEnded = true;

                if (game.Reward > 0)
                {
                    game.Message += $" You receive your accumulated reward of {game.Reward} coins.";
                    await CashTransactionAsync(game.UserId, game.Reward);
                }

                await FinishHigherLowerGame(game);
                await _animal.UpdateAnimalShop();
                return game;
            }

            int currentRankValue = (int)game.CurrentCard!.Rank;
            int nextRankValue = (int)game.NextCard!.Rank;

            bool playerWinsRound = false;

            if (nextRankValue > currentRankValue)
            {
                if (game.Choice == HigherOrLowerChoiceEnum.HIGHER)
                    playerWinsRound = true;
            }
            else if (nextRankValue < currentRankValue)
            {
                if (game.Choice == HigherOrLowerChoiceEnum.LOWER)
                    playerWinsRound = true;
            }
            else
            {
                playerWinsRound = true; // tie always wins
            }

            // ─────────────────────────────────────────────────────
            // 🍸 DRINK EFFECT — GUARANTEED_WIN
            // Checked AFTER the card comparison is evaluated but
            // BEFORE playerWinsRound is acted upon, so it can
            // override a loss without skipping the card logic.
            // Consumes 1 round from the effect per call.
            // ─────────────────────────────────────────────────────
            bool hasGuaranteedWin = await _activeDrinkEffectService
                .ConsumeRoundAsync(game.UserId, DrinkEffectTypes.GuaranteedWin);

            if (hasGuaranteedWin)
                playerWinsRound = true;
            // ─────────────────────────────────────────────────────

            if (playerWinsRound)
            {
                game.GameEnded = false;

                if (game.Reward == 0)
                    game.Reward = game.AnimalValue;

                int rewardBeforeThisRound = game.Reward;
                int roundIncrement = game.Reward / 2;
                game.Reward += roundIncrement;

                // ─────────────────────────────────────────────────────
                // 🍸 DRINK EFFECT — DOUBLE_REWARDS
                // Applied AFTER the round increment is calculated so
                // we can double just the increment gained this round.
                // This keeps compounding fair — it only doubles the
                // gain of the current round, not the entire pot.
                // ─────────────────────────────────────────────────────
                bool hasDoubleReward = await _activeDrinkEffectService
                    .ConsumeRoundAsync(game.UserId, DrinkEffectTypes.DoubleRewards);

                if (hasDoubleReward)
                    game.Reward += roundIncrement; // adds another increment = effectively doubled
                                                   // ─────────────────────────────────────────────────────

                game.Message = $"You guessed correctly! The next card was {game.NextCard.DisplayName}. " +
                               $"Your current accumulated reward is {game.Reward} coins. " +
                               $"Keep playing to increase the reward or cash out!";

                if (hasGuaranteedWin)
                    game.Message += " 🍸 YAG TONIC guaranteed your win this round!";

                if (hasDoubleReward)
                    game.Message += " 🍹 YAGARETTO doubled your reward this round!";

                game.CurrentCard = game.NextCard;

                if (deckServiceForGame.RemainingCards > 0)
                {
                    game.NextCard = deckServiceForGame.CatchCard();

                    var gsProgress = await _dbContext.GameSessions
                        .FirstOrDefaultAsync(g => g.Id == game.GameSessionId);
                    var sProgress = await _dbContext.HigherLowerSessions
                        .FirstOrDefaultAsync(g => g.GameSessionId == game.GameSessionId);

                    if (gsProgress != null) gsProgress.Result = "PROGRESS";
                    if (sProgress != null)
                    {
                        sProgress.Status = "PROGRESS";
                        sProgress.RoundsPlayed = game.RoundsPLayed;
                    }

                    await _dbContext.SaveChangesAsync();
                }
                else
                {
                    game.Message += " The deck is now empty! You automatically cash out.";
                    game.GameEnded = true;

                    if (game.Reward > 0)
                        await CashTransactionAsync(game.UserId, game.Reward);

                    await FinishHigherLowerGame(game);
                    await _animal.UpdateAnimalShop();
                }
            }
            else
            {
                // ─────────────────────────────────────────────────────
                // 🍸 DRINK EFFECT — PREVENT_LOSS
                // Checked BEFORE RemoveAnimal so the animal can be
                // spared. If active, the game does NOT end — the player
                // keeps their accumulated reward and continues playing.
                // The effect is consumed (1 round deducted) either way.
                // ─────────────────────────────────────────────────────
                bool hasPreventLoss = await _activeDrinkEffectService
                    .ConsumeRoundAsync(game.UserId, DrinkEffectTypes.PreventLoss);

                if (hasPreventLoss)
                {
                    game.GameEnded = false; // game continues!
                    game.Message = $"You guessed wrong, but 🍸 YAGIBU protected your animal! " +
                                   $"The next card was {game.NextCard.DisplayName}. " +
                                   $"Your accumulated reward of {game.Reward} coins is safe. Keep going!";

                    // Advance the card so the round isn't stuck on the same pair
                    game.CurrentCard = game.NextCard;

                    if (deckServiceForGame.RemainingCards > 0)
                    {
                        game.NextCard = deckServiceForGame.CatchCard();

                        var gsProgress = await _dbContext.GameSessions
                            .FirstOrDefaultAsync(g => g.Id == game.GameSessionId);
                        var sProgress = await _dbContext.HigherLowerSessions
                            .FirstOrDefaultAsync(g => g.GameSessionId == game.GameSessionId);

                        if (gsProgress != null) gsProgress.Result = "PROGRESS";
                        if (sProgress != null)
                        {
                            sProgress.Status = "PROGRESS";
                            sProgress.RoundsPlayed = game.RoundsPLayed;
                        }

                        await _dbContext.SaveChangesAsync();
                    }
                    else
                    {
                        // Deck ran out right after the protected round — auto cash out
                        game.GameEnded = true;
                        game.Message += " The deck is now empty! You automatically cash out.";

                        if (game.Reward > 0)
                            await CashTransactionAsync(game.UserId, game.Reward);

                        await FinishHigherLowerGame(game);
                        await _animal.UpdateAnimalShop();
                    }
                }
                else
                {
                    // No protection — normal loss
                    game.GameEnded = true;
                    game.Message = $"You selected {game.Choice}, but the next card was {game.NextCard.DisplayName}. " +
                                   $"You lose! Your reward is {game.Reward} coins, and your animal is dead.";

                    await CashTransactionAsync(game.UserId, game.Reward);
                    await _animal.RemoveAnimal(game.AnimalId);

                    await FinishHigherLowerGame(game);
                    await _animal.UpdateAnimalShop();
                }
                // ─────────────────────────────────────────────────────
            }

            return game;
        }


        // Retrieves an active Higher or Lower game from memory using its ID.
        // Returns the current game state if found.
        public HigherLowerGame GetHigherLowerGame(Guid gameId)
        {
            if (_activeHigherLowerGames.TryGetValue(gameId, out var game))
            {
                return game;
            }
            throw new KeyNotFoundException($"Higher/Lower game with ID {gameId} not found.");
        }

        // Maps a HigherLowerGame object to a HigherLowerResponse DTO for client consumption.
        public HigherLowerResponse MapToHigherLowerResponse(HigherLowerGame game)
        {
            return new HigherLowerResponse
            {
                GameId = game.GameId,
                AnimalId = game.AnimalId,
                AnimalName = game.AnimalName,
                AnimalValue = game.AnimalValue,
                Reward = game.Reward,
                Message = game.Message,
                CurrentCard = game.CurrentCard,
                GameEnded = game.GameEnded
            };
        }


        // Allows the player to cash out an active Higher or Lower game, ending it immediately.
        // Transfers the accumulated reward if available and updates the game state.
        // Finalizes the game and persists the result.
        public async Task<HigherLowerGame> CashOutHigherLower(Guid gameId)
        {
            if (!_activeHigherLowerGames.TryGetValue(gameId, out var game))
            {
                throw new KeyNotFoundException($"Active Higher/Lower game with ID {gameId} not found.");
            }



            if (game.GameEnded)
            {
                throw new InvalidOperationException("This Higher/Lower game has already finished. No reward to cash out.");
            }

            game.GameEnded = true;
            game.Message = $"You cashed out! You receive your accumulated reward of {game.Reward} coins.";

            if (game.Reward > 0)
            {
                try
                {
                    await CashTransactionAsync(game.UserId, game.Reward);
                }
                catch (Exception ex)
                {
                    throw new InvalidOperationException("Failed to process reward transfer.", ex);
                }
            }
            else
            {
                game.Message = "You cashed out, but there was no accumulated reward to receive.";
            }
            await FinishHigherLowerGame(game);
            return game;
        }


        // Finalizes a Higher or Lower game, updating session data with final results and rewards.
        // Persists completion status, rounds played, and total earnings in the database.
        // Removes the game and its deck from in-memory storage.
        public async Task FinishHigherLowerGame(HigherLowerGame game)
        {

            var GameSession = await _dbContext.GameSessions.FirstOrDefaultAsync((System.Linq.Expressions.Expression<Func<GameSession, bool>>)(g => g.Id == game.GameSessionId));
            var Session = await _dbContext.HigherLowerSessions.FirstOrDefaultAsync(g => g.GameSessionId == game.GameSessionId);

            if (GameSession == null)
                throw new Exception($"GameSession NOT FOUND: {game.GameSessionId}");

            if (Session == null)
                throw new Exception($"HigherLowerSession NOT FOUND for GameSessionId: {game.GameSessionId}");

            GameSession.Result = "Completed";
            GameSession.MoneyEarned = game.Reward;
            Session.Status = "Completed";
            Session.TotalEarned = game.Reward;
            Session.FinishedAt = DateTime.UtcNow.ToString();
            Session.RoundsPlayed = game.RoundsPLayed;

            await _dbContext.SaveChangesAsync();

            _activeHigherLowerGames.TryRemove(game.GameId, out _);
            _higherLowerGameDecks.TryRemove(game.GameId, out _);
        }

        #endregion


    }
}