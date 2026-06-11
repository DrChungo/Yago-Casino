using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chaos.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddCasinoSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Casino");

            migrationBuilder.RenameTable(
                name: "Users",
                newName: "Users",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "SlotSymbols",
                newName: "SlotSymbols",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "SlotSessions",
                newName: "SlotSessions",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "SlotPayoutRules",
                newName: "SlotPayoutRules",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "SlotGameConfig",
                newName: "SlotGameConfig",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "ShopTransactions",
                newName: "ShopTransactions",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "ShopAnimalListings",
                newName: "ShopAnimalListings",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "RussianRouletteRounds",
                newName: "RussianRouletteRounds",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "RussianRoulettePlayers",
                newName: "RussianRoulettePlayers",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "RussianRouletteLobbies",
                newName: "RussianRouletteLobbies",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "RussianRouletteGameConfig",
                newName: "RussianRouletteGameConfig",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "RouletteSession",
                newName: "RouletteSession",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "RouletteGameConfig",
                newName: "RouletteGameConfig",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "RouletteBetTypes",
                newName: "RouletteBetTypes",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "HigherLowerSessions",
                newName: "HigherLowerSessions",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "HigherLowerRounds",
                newName: "HigherLowerRounds",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "HigherLowerGameConfig",
                newName: "HigherLowerGameConfig",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "GameSessions",
                newName: "GameSessions",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "Games",
                newName: "Games",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "CoinFlipSessions",
                newName: "CoinFlipSessions",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "CoinFlipGameConfig",
                newName: "CoinFlipGameConfig",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "Casino",
                newName: "Casino",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "BlackjackSessions",
                newName: "BlackjackSessions",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "BlackjackHands",
                newName: "BlackjackHands",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "BlackjackGameConfig",
                newName: "BlackjackGameConfig",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "BlackjackCards",
                newName: "BlackjackCards",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "BlackjackActions",
                newName: "BlackjackActions",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "AnimalValueConfig",
                newName: "AnimalValueConfig",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "AnimalShop",
                newName: "AnimalShop",
                newSchema: "Casino");

            migrationBuilder.RenameTable(
                name: "Animals",
                newName: "Animals",
                newSchema: "Casino");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Users",
                schema: "Casino",
                newName: "Users");

            migrationBuilder.RenameTable(
                name: "SlotSymbols",
                schema: "Casino",
                newName: "SlotSymbols");

            migrationBuilder.RenameTable(
                name: "SlotSessions",
                schema: "Casino",
                newName: "SlotSessions");

            migrationBuilder.RenameTable(
                name: "SlotPayoutRules",
                schema: "Casino",
                newName: "SlotPayoutRules");

            migrationBuilder.RenameTable(
                name: "SlotGameConfig",
                schema: "Casino",
                newName: "SlotGameConfig");

            migrationBuilder.RenameTable(
                name: "ShopTransactions",
                schema: "Casino",
                newName: "ShopTransactions");

            migrationBuilder.RenameTable(
                name: "ShopAnimalListings",
                schema: "Casino",
                newName: "ShopAnimalListings");

            migrationBuilder.RenameTable(
                name: "RussianRouletteRounds",
                schema: "Casino",
                newName: "RussianRouletteRounds");

            migrationBuilder.RenameTable(
                name: "RussianRoulettePlayers",
                schema: "Casino",
                newName: "RussianRoulettePlayers");

            migrationBuilder.RenameTable(
                name: "RussianRouletteLobbies",
                schema: "Casino",
                newName: "RussianRouletteLobbies");

            migrationBuilder.RenameTable(
                name: "RussianRouletteGameConfig",
                schema: "Casino",
                newName: "RussianRouletteGameConfig");

            migrationBuilder.RenameTable(
                name: "RouletteSession",
                schema: "Casino",
                newName: "RouletteSession");

            migrationBuilder.RenameTable(
                name: "RouletteGameConfig",
                schema: "Casino",
                newName: "RouletteGameConfig");

            migrationBuilder.RenameTable(
                name: "RouletteBetTypes",
                schema: "Casino",
                newName: "RouletteBetTypes");

            migrationBuilder.RenameTable(
                name: "HigherLowerSessions",
                schema: "Casino",
                newName: "HigherLowerSessions");

            migrationBuilder.RenameTable(
                name: "HigherLowerRounds",
                schema: "Casino",
                newName: "HigherLowerRounds");

            migrationBuilder.RenameTable(
                name: "HigherLowerGameConfig",
                schema: "Casino",
                newName: "HigherLowerGameConfig");

            migrationBuilder.RenameTable(
                name: "GameSessions",
                schema: "Casino",
                newName: "GameSessions");

            migrationBuilder.RenameTable(
                name: "Games",
                schema: "Casino",
                newName: "Games");

            migrationBuilder.RenameTable(
                name: "CoinFlipSessions",
                schema: "Casino",
                newName: "CoinFlipSessions");

            migrationBuilder.RenameTable(
                name: "CoinFlipGameConfig",
                schema: "Casino",
                newName: "CoinFlipGameConfig");

            migrationBuilder.RenameTable(
                name: "Casino",
                schema: "Casino",
                newName: "Casino");

            migrationBuilder.RenameTable(
                name: "BlackjackSessions",
                schema: "Casino",
                newName: "BlackjackSessions");

            migrationBuilder.RenameTable(
                name: "BlackjackHands",
                schema: "Casino",
                newName: "BlackjackHands");

            migrationBuilder.RenameTable(
                name: "BlackjackGameConfig",
                schema: "Casino",
                newName: "BlackjackGameConfig");

            migrationBuilder.RenameTable(
                name: "BlackjackCards",
                schema: "Casino",
                newName: "BlackjackCards");

            migrationBuilder.RenameTable(
                name: "BlackjackActions",
                schema: "Casino",
                newName: "BlackjackActions");

            migrationBuilder.RenameTable(
                name: "AnimalValueConfig",
                schema: "Casino",
                newName: "AnimalValueConfig");

            migrationBuilder.RenameTable(
                name: "AnimalShop",
                schema: "Casino",
                newName: "AnimalShop");

            migrationBuilder.RenameTable(
                name: "Animals",
                schema: "Casino",
                newName: "Animals");
        }
    }
}
