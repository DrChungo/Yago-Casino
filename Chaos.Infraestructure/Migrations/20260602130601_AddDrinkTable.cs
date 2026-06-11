using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Chaos.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDrinkTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {

            migrationBuilder.CreateTable(
                schema: "Casino",
                name: "ActiveDrinkEffects",
                columns: table => new
                {                    
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false),
                    EffectType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    RoundsRemaining = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {                    
                    table.PrimaryKey("PK_ActiveDrinkEffects", x => x.Id);
                    table.ForeignKey(
                        principalSchema: "Casino",
                        name: "FK_ActiveDrinkEffects_Users",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                schema: "Casino",
                name: "IX_ActiveDrinkEffects_UserId",
                table: "ActiveDrinkEffects",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}
