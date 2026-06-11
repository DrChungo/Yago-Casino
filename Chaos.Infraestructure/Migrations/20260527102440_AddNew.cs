using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chaos.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddNew : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Casino");

            migrationBuilder.CreateTable(
                name: "AnimalShop",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ShopName = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalShop", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Casino",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Casino", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Games",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GameName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    GameType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    Wallet = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsAdmin = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BlackjackGameConfig",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TableName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MaxPlayers = table.Column<int>(type: "integer", nullable: false, defaultValue: 6),
                    NumberOfDecks = table.Column<int>(type: "integer", nullable: false, defaultValue: 6),
                    DealerStandsOn = table.Column<int>(type: "integer", nullable: false, defaultValue: 17),
                    BlackjackPayout = table.Column<decimal>(type: "numeric(4,2)", nullable: false, defaultValue: 1.50m),
                    AllowDoubleDown = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    AllowInsurance = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackjackGameConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BlackjackGameConfig_Games",
                        column: x => x.GameId,
                        principalSchema: "Casino",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoinFlipGameConfig",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoinFlipGameConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoinFlipGameConfig_Games",
                        column: x => x.GameId,
                        principalSchema: "Casino",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HigherLowerGameConfig",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    BaseMultiplier = table.Column<decimal>(type: "numeric(8,2)", nullable: false, defaultValue: 1.00m),
                    RoundIncrement = table.Column<decimal>(type: "numeric(8,2)", nullable: false, defaultValue: 0.50m),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HigherLowerGameConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HigherLowerGameConfig_Games",
                        column: x => x.GameId,
                        principalSchema: "Casino",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RouletteGameConfig",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TableName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    RouletteType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false, defaultValue: "European"),
                    HasZero = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    HasDoubleZero = table.Column<bool>(type: "boolean", nullable: false),
                    TotalNumbers = table.Column<int>(type: "integer", nullable: false, defaultValue: 37),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouletteGameConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouletteGameConfig_Games",
                        column: x => x.GameId,
                        principalSchema: "Casino",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RussianRouletteGameConfig",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ConfigName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    TotalChambers = table.Column<int>(type: "integer", nullable: false, defaultValue: 6),
                    BulletCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    MaxPlayers = table.Column<int>(type: "integer", nullable: false, defaultValue: 6),
                    MinPlayers = table.Column<int>(type: "integer", nullable: false, defaultValue: 2),
                    FixedPrizePool = table.Column<decimal>(type: "numeric(18,2)", nullable: false, defaultValue: 500.00m),
                    AllowBots = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RussianRouletteGameConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RussianRouletteGameConfig_Games",
                        column: x => x.GameId,
                        principalSchema: "Casino",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlotGameConfig",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MachineName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Multiplier = table.Column<decimal>(type: "numeric(8,2)", nullable: false, defaultValue: 1.00m),
                    NumberOfReels = table.Column<int>(type: "integer", nullable: false, defaultValue: 3),
                    NumberOfRows = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    PayLines = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotGameConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlotGameConfig_Games",
                        column: x => x.GameId,
                        principalSchema: "Casino",
                        principalTable: "Games",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AnimalValueConfig",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    MinAge = table.Column<int>(type: "integer", nullable: false),
                    MaxAge = table.Column<int>(type: "integer", nullable: false),
                    MinWeight = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    MaxWeight = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    MinHeight = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    MaxHeight = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    MinHealth = table.Column<int>(type: "integer", nullable: false),
                    MaxHealth = table.Column<int>(type: "integer", nullable: false),
                    Habitat = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    ImageUrlMecha = table.Column<string>(type: "text", nullable: true),
                    ImageUrlNormal = table.Column<string>(type: "text", nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    UpdatedBy = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AnimalValueConfig", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AnimalValueConfig_Users",
                        column: x => x.UpdatedBy,
                        principalSchema: "Casino",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RouletteBetTypes",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BetName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Payout = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    Description = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    RouletteGameConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouletteBetTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouletteBetTypes_RouletteGameConfig",
                        column: x => x.RouletteGameConfigId,
                        principalSchema: "Casino",
                        principalTable: "RouletteGameConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SlotSymbols",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SymbolName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SymbolCode = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    Rarity = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    BaseValue = table.Column<decimal>(type: "numeric(8,2)", nullable: false, defaultValue: 1.00m),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SlotGameConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotSymbols", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlotSymbols_SlotGameConfig",
                        column: x => x.SlotGameConfigId,
                        principalSchema: "Casino",
                        principalTable: "SlotGameConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Animals",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    AnimalType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Rarity = table.Column<bool>(type: "boolean", nullable: false),
                    Health = table.Column<int>(type: "integer", nullable: false, defaultValue: 100),
                    Age = table.Column<int>(type: "integer", nullable: true),
                    Weight = table.Column<decimal>(type: "numeric(8,2)", nullable: true),
                    Height = table.Column<decimal>(type: "numeric(8,2)", nullable: true),
                    EstimatedValue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    AnimalValueConfigId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Animals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Animals_Users",
                        column: x => x.OwnerId,
                        principalSchema: "Casino",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Animals_ValueConfig",
                        column: x => x.AnimalValueConfigId,
                        principalSchema: "Casino",
                        principalTable: "AnimalValueConfig",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SlotPayoutRules",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Combination = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PayoutMultiplier = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SlotGameConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotSymbolId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotPayoutRules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PayoutRules_SlotGameConfig",
                        column: x => x.SlotGameConfigId,
                        principalSchema: "Casino",
                        principalTable: "SlotGameConfig",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PayoutRules_SlotSymbols",
                        column: x => x.SlotSymbolId,
                        principalSchema: "Casino",
                        principalTable: "SlotSymbols",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "GameSessions",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Result = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    MoneyEarned = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    Multiplier = table.Column<decimal>(type: "numeric(8,2)", nullable: false, defaultValue: 1.00m),
                    PlayedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GameSessions_Animals",
                        column: x => x.AnimalId,
                        principalSchema: "Casino",
                        principalTable: "Animals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameSessions_Games",
                        column: x => x.GameId,
                        principalSchema: "Casino",
                        principalTable: "Games",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_GameSessions_Users",
                        column: x => x.UserId,
                        principalSchema: "Casino",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShopAnimalListings",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ListingPrice = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    IsSold = table.Column<bool>(type: "boolean", nullable: false),
                    ListedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    SoldAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalShopId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopAnimalListings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Listings_AnimalShop",
                        column: x => x.AnimalShopId,
                        principalSchema: "Casino",
                        principalTable: "AnimalShop",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Listings_Animals",
                        column: x => x.AnimalId,
                        principalSchema: "Casino",
                        principalTable: "Animals",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ShopTransactions",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TransactionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Amount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    TransactionDate = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShopTransactions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ShopTrans_Animals",
                        column: x => x.AnimalId,
                        principalSchema: "Casino",
                        principalTable: "Animals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_ShopTrans_Users",
                        column: x => x.UserId,
                        principalSchema: "Casino",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlackjackSessions",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "InProgress"),
                    PlayerScore = table.Column<int>(type: "integer", nullable: false),
                    DealerScore = table.Column<int>(type: "integer", nullable: false),
                    HasSplit = table.Column<bool>(type: "boolean", nullable: false),
                    HasDoubledDown = table.Column<bool>(type: "boolean", nullable: false),
                    HasInsurance = table.Column<bool>(type: "boolean", nullable: false),
                    InsuranceWon = table.Column<bool>(type: "boolean", nullable: true),
                    MoneyEarned = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    StartedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FinishedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    BlackjackGameConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackjackSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BJSessions_Animals",
                        column: x => x.AnimalId,
                        principalSchema: "Casino",
                        principalTable: "Animals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BJSessions_GameConfig",
                        column: x => x.BlackjackGameConfigId,
                        principalSchema: "Casino",
                        principalTable: "BlackjackGameConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BJSessions_GameSessions",
                        column: x => x.GameSessionId,
                        principalSchema: "Casino",
                        principalTable: "GameSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BJSessions_Users",
                        column: x => x.UserId,
                        principalSchema: "Casino",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "CoinFlipSessions",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CoinResult = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    UserChoice = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsWin = table.Column<bool>(type: "boolean", nullable: false),
                    WinProbabilityUsed = table.Column<decimal>(type: "numeric(5,2)", nullable: false),
                    PrizeMultiplierUsed = table.Column<decimal>(type: "numeric(8,2)", nullable: false),
                    MoneyEarned = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PlayedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    CoinFlipGameConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoinFlipSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CFSessions_Animals",
                        column: x => x.AnimalId,
                        principalSchema: "Casino",
                        principalTable: "Animals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CFSessions_GameConfig",
                        column: x => x.CoinFlipGameConfigId,
                        principalSchema: "Casino",
                        principalTable: "CoinFlipGameConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CFSessions_GameSessions",
                        column: x => x.GameSessionId,
                        principalSchema: "Casino",
                        principalTable: "GameSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_CFSessions_Users",
                        column: x => x.UserId,
                        principalSchema: "Casino",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "HigherLowerSessions",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "InProgress"),
                    TotalEarned = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    RoundsPlayed = table.Column<int>(type: "integer", nullable: false),
                    StartedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FinishedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnimalId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    HigherLowerGameConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HigherLowerSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HLSessions_Animals",
                        column: x => x.AnimalId,
                        principalSchema: "Casino",
                        principalTable: "Animals",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HLSessions_GameConfig",
                        column: x => x.HigherLowerGameConfigId,
                        principalSchema: "Casino",
                        principalTable: "HigherLowerGameConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HLSessions_GameSessions",
                        column: x => x.GameSessionId,
                        principalSchema: "Casino",
                        principalTable: "GameSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_HLSessions_Users",
                        column: x => x.UserId,
                        principalSchema: "Casino",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RouletteSession",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    SpinResult = table.Column<int>(type: "integer", nullable: false),
                    RouletteGameConfigId = table.Column<Guid>(type: "uuid", nullable: false),
                    GameSessionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RouletteSession", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RouletteSession_GameSession",
                        column: x => x.GameSessionId,
                        principalSchema: "Casino",
                        principalTable: "GameSessions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RouletteSession_RouletteGameConfig",
                        column: x => x.RouletteGameConfigId,
                        principalSchema: "Casino",
                        principalTable: "RouletteGameConfig",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "RussianRouletteLobbies",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    LobbyCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false, defaultValue: "Waiting"),
                    CurrentPrizePool = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    WinnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    StartedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    FinishedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    GameSessionId = table.Column<Guid>(type: "uuid", nullable: true),
                    RussianRouletteGameConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RussianRouletteLobbies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RRLobbies_GameConfig",
                        column: x => x.RussianRouletteGameConfigId,
                        principalSchema: "Casino",
                        principalTable: "RussianRouletteGameConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RRLobbies_Winner",
                        column: x => x.WinnerId,
                        principalSchema: "Casino",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RussianRouletteLobbies_GameSessions",
                        column: x => x.GameSessionId,
                        principalSchema: "Casino",
                        principalTable: "GameSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SlotSessions",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    BetAmount = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    WinningSymbols = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    GameSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    SlotGameConfigId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlotSession_GameConfig",
                        column: x => x.SlotGameConfigId,
                        principalSchema: "Casino",
                        principalTable: "SlotGameConfig",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SlotSession_GameSessions",
                        column: x => x.GameSessionId,
                        principalSchema: "Casino",
                        principalTable: "GameSessions",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlackjackHands",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    HandOwner = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    HandNumber = table.Column<int>(type: "integer", nullable: false, defaultValue: 1),
                    FinalScore = table.Column<int>(type: "integer", nullable: false),
                    IsBust = table.Column<bool>(type: "boolean", nullable: false),
                    IsBlackjack = table.Column<bool>(type: "boolean", nullable: false),
                    BlackjackSessionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackjackHands", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BJHands_Sessions",
                        column: x => x.BlackjackSessionId,
                        principalSchema: "Casino",
                        principalTable: "BlackjackSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "HigherLowerRounds",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundNumber = table.Column<int>(type: "integer", nullable: false),
                    CurrentCard = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NextCard = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: true),
                    UserGuess = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: true),
                    EarnedThisRound = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    PlayedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    HigherLowerSessionId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HigherLowerRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_HLRounds_HLSessions",
                        column: x => x.HigherLowerSessionId,
                        principalSchema: "Casino",
                        principalTable: "HigherLowerSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RussianRoulettePlayers",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    IsBot = table.Column<bool>(type: "boolean", nullable: false),
                    BotName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    TurnOrder = table.Column<int>(type: "integer", nullable: false),
                    IsAlive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    IsWinner = table.Column<bool>(type: "boolean", nullable: false),
                    EliminatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    JoinedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    UserId = table.Column<Guid>(type: "uuid", nullable: true),
                    LobbyId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RussianRoulettePlayers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RRPlayers_Lobbies",
                        column: x => x.LobbyId,
                        principalSchema: "Casino",
                        principalTable: "RussianRouletteLobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RRPlayers_Users",
                        column: x => x.UserId,
                        principalSchema: "Casino",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "BlackjackActions",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ActionType = table.Column<string>(type: "character varying(15)", maxLength: 15, nullable: false),
                    ActionOrder = table.Column<int>(type: "integer", nullable: false),
                    ActionAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    BlackjackSessionId = table.Column<Guid>(type: "uuid", nullable: false),
                    HandId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackjackActions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BJActions_Hands",
                        column: x => x.HandId,
                        principalSchema: "Casino",
                        principalTable: "BlackjackHands",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_BJActions_Sessions",
                        column: x => x.BlackjackSessionId,
                        principalSchema: "Casino",
                        principalTable: "BlackjackSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BlackjackCards",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    CardValue = table.Column<string>(type: "character varying(5)", maxLength: 5, nullable: false),
                    CardSuit = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    CardDisplay = table.Column<string>(type: "character varying(10)", maxLength: 10, nullable: false),
                    NumericValue = table.Column<int>(type: "integer", nullable: false),
                    IsFaceDown = table.Column<bool>(type: "boolean", nullable: false),
                    DealtAt = table.Column<int>(type: "integer", nullable: false),
                    CardBlackjackValue = table.Column<int>(type: "integer", nullable: false),
                    HandId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlackjackCards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BJCards_Hands",
                        column: x => x.HandId,
                        principalSchema: "Casino",
                        principalTable: "BlackjackHands",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RussianRouletteRounds",
                schema: "Casino",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoundNumber = table.Column<int>(type: "integer", nullable: false),
                    WasBullet = table.Column<bool>(type: "boolean", nullable: false),
                    PlayedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    LobbyId = table.Column<Guid>(type: "uuid", nullable: false),
                    PlayerId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RussianRouletteRounds", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RRRounds_Lobbies",
                        column: x => x.LobbyId,
                        principalSchema: "Casino",
                        principalTable: "RussianRouletteLobbies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RRRounds_Players",
                        column: x => x.PlayerId,
                        principalSchema: "Casino",
                        principalTable: "RussianRoulettePlayers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Animals_AnimalValueConfigId",
                schema: "Casino",
                table: "Animals",
                column: "AnimalValueConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Animals_OwnerId",
                schema: "Casino",
                table: "Animals",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_AnimalValueConfig_AnimalType",
                schema: "Casino",
                table: "AnimalValueConfig",
                column: "AnimalType",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AnimalValueConfig_UpdatedBy",
                schema: "Casino",
                table: "AnimalValueConfig",
                column: "UpdatedBy");

            migrationBuilder.CreateIndex(
                name: "IX_BlackjackActions_BlackjackSessionId",
                schema: "Casino",
                table: "BlackjackActions",
                column: "BlackjackSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackjackActions_HandId",
                schema: "Casino",
                table: "BlackjackActions",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackjackCards_HandId",
                schema: "Casino",
                table: "BlackjackCards",
                column: "HandId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackjackGameConfig_GameId",
                schema: "Casino",
                table: "BlackjackGameConfig",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackjackHands_BlackjackSessionId",
                schema: "Casino",
                table: "BlackjackHands",
                column: "BlackjackSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackjackSessions_AnimalId",
                schema: "Casino",
                table: "BlackjackSessions",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackjackSessions_BlackjackGameConfigId",
                schema: "Casino",
                table: "BlackjackSessions",
                column: "BlackjackGameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackjackSessions_GameSessionId",
                schema: "Casino",
                table: "BlackjackSessions",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_BlackjackSessions_UserId",
                schema: "Casino",
                table: "BlackjackSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_CoinFlipGameConfig_GameId",
                schema: "Casino",
                table: "CoinFlipGameConfig",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_CoinFlipSessions_AnimalId",
                schema: "Casino",
                table: "CoinFlipSessions",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_CoinFlipSessions_CoinFlipGameConfigId",
                schema: "Casino",
                table: "CoinFlipSessions",
                column: "CoinFlipGameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_CoinFlipSessions_GameSessionId",
                schema: "Casino",
                table: "CoinFlipSessions",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_CoinFlipSessions_UserId",
                schema: "Casino",
                table: "CoinFlipSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_AnimalId",
                schema: "Casino",
                table: "GameSessions",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_GameId",
                schema: "Casino",
                table: "GameSessions",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_GameSessions_UserId",
                schema: "Casino",
                table: "GameSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_HigherLowerGameConfig_GameId",
                schema: "Casino",
                table: "HigherLowerGameConfig",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_HigherLowerRounds_HigherLowerSessionId",
                schema: "Casino",
                table: "HigherLowerRounds",
                column: "HigherLowerSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_HigherLowerSessions_AnimalId",
                schema: "Casino",
                table: "HigherLowerSessions",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_HigherLowerSessions_GameSessionId",
                schema: "Casino",
                table: "HigherLowerSessions",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_HigherLowerSessions_HigherLowerGameConfigId",
                schema: "Casino",
                table: "HigherLowerSessions",
                column: "HigherLowerGameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_HigherLowerSessions_UserId",
                schema: "Casino",
                table: "HigherLowerSessions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RouletteBetTypes_RouletteGameConfigId",
                schema: "Casino",
                table: "RouletteBetTypes",
                column: "RouletteGameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_RouletteGameConfig_GameId",
                schema: "Casino",
                table: "RouletteGameConfig",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_RouletteSession_GameSessionId",
                schema: "Casino",
                table: "RouletteSession",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RouletteSession_RouletteGameConfigId",
                schema: "Casino",
                table: "RouletteSession",
                column: "RouletteGameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_RussianRouletteGameConfig_GameId",
                schema: "Casino",
                table: "RussianRouletteGameConfig",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_RussianRouletteLobbies_GameSessionId",
                schema: "Casino",
                table: "RussianRouletteLobbies",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_RussianRouletteLobbies_LobbyCode",
                schema: "Casino",
                table: "RussianRouletteLobbies",
                column: "LobbyCode",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RussianRouletteLobbies_RussianRouletteGameConfigId",
                schema: "Casino",
                table: "RussianRouletteLobbies",
                column: "RussianRouletteGameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_RussianRouletteLobbies_WinnerId",
                schema: "Casino",
                table: "RussianRouletteLobbies",
                column: "WinnerId");

            migrationBuilder.CreateIndex(
                name: "IX_RussianRoulettePlayers_LobbyId",
                schema: "Casino",
                table: "RussianRoulettePlayers",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_RussianRoulettePlayers_UserId",
                schema: "Casino",
                table: "RussianRoulettePlayers",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RussianRouletteRounds_LobbyId",
                schema: "Casino",
                table: "RussianRouletteRounds",
                column: "LobbyId");

            migrationBuilder.CreateIndex(
                name: "IX_RussianRouletteRounds_PlayerId",
                schema: "Casino",
                table: "RussianRouletteRounds",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopAnimalListings_AnimalId",
                schema: "Casino",
                table: "ShopAnimalListings",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopAnimalListings_AnimalShopId",
                schema: "Casino",
                table: "ShopAnimalListings",
                column: "AnimalShopId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopTransactions_AnimalId",
                schema: "Casino",
                table: "ShopTransactions",
                column: "AnimalId");

            migrationBuilder.CreateIndex(
                name: "IX_ShopTransactions_UserId",
                schema: "Casino",
                table: "ShopTransactions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotGameConfig_GameId",
                schema: "Casino",
                table: "SlotGameConfig",
                column: "GameId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotPayoutRules_SlotGameConfigId",
                schema: "Casino",
                table: "SlotPayoutRules",
                column: "SlotGameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotPayoutRules_SlotSymbolId",
                schema: "Casino",
                table: "SlotPayoutRules",
                column: "SlotSymbolId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotSessions_GameSessionId",
                schema: "Casino",
                table: "SlotSessions",
                column: "GameSessionId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotSessions_SlotGameConfigId",
                schema: "Casino",
                table: "SlotSessions",
                column: "SlotGameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_SlotSymbols_SlotGameConfigId",
                schema: "Casino",
                table: "SlotSymbols",
                column: "SlotGameConfigId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Email",
                schema: "Casino",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BlackjackActions",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "BlackjackCards",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "Casino",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "CoinFlipSessions",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "HigherLowerRounds",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "RouletteBetTypes",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "RouletteSession",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "RussianRouletteRounds",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "ShopAnimalListings",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "ShopTransactions",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "SlotPayoutRules",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "SlotSessions",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "BlackjackHands",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "CoinFlipGameConfig",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "HigherLowerSessions",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "RouletteGameConfig",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "RussianRoulettePlayers",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "AnimalShop",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "SlotSymbols",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "BlackjackSessions",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "HigherLowerGameConfig",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "RussianRouletteLobbies",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "SlotGameConfig",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "BlackjackGameConfig",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "RussianRouletteGameConfig",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "GameSessions",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "Animals",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "Games",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "AnimalValueConfig",
                schema: "Casino");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "Casino");
        }
    }
}
