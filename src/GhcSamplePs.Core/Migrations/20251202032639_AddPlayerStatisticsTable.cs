using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GhcSamplePs.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddPlayerStatisticsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PlayerStatistics",
                columns: table => new
                {
                    PlayerStatisticId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TeamPlayerId = table.Column<int>(type: "int", nullable: false),
                    GameDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    MinutesPlayed = table.Column<int>(type: "int", nullable: false),
                    IsStarter = table.Column<bool>(type: "bit", nullable: false),
                    JerseyNumber = table.Column<int>(type: "int", nullable: false),
                    Goals = table.Column<int>(type: "int", nullable: false),
                    Assists = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PlayerStatistics", x => x.PlayerStatisticId);
                    table.ForeignKey(
                        name: "FK_PlayerStatistics_TeamPlayers_TeamPlayerId",
                        column: x => x.TeamPlayerId,
                        principalTable: "TeamPlayers",
                        principalColumn: "TeamPlayerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatistics_GameDate",
                table: "PlayerStatistics",
                column: "GameDate");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatistics_TeamPlayerId",
                table: "PlayerStatistics",
                column: "TeamPlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_PlayerStatistics_TeamPlayerId_GameDate",
                table: "PlayerStatistics",
                columns: new[] { "TeamPlayerId", "GameDate" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PlayerStatistics");
        }
    }
}
