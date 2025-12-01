using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GhcSamplePs.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddTeamPlayersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamPlayers",
                columns: table => new
                {
                    TeamPlayerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PlayerId = table.Column<int>(type: "int", nullable: false),
                    TeamName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ChampionshipName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    JoinedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LeftDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamPlayers", x => x.TeamPlayerId);
                    table.ForeignKey(
                        name: "FK_TeamPlayers_Players_PlayerId",
                        column: x => x.PlayerId,
                        principalTable: "Players",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_IsActive",
                table: "TeamPlayers",
                column: "LeftDate");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_PlayerId",
                table: "TeamPlayers",
                column: "PlayerId");

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_PlayerId_IsActive",
                table: "TeamPlayers",
                columns: new[] { "PlayerId", "LeftDate" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_PlayerId_TeamName_ChampionshipName",
                table: "TeamPlayers",
                columns: new[] { "PlayerId", "TeamName", "ChampionshipName" });

            migrationBuilder.CreateIndex(
                name: "IX_TeamPlayers_TeamName",
                table: "TeamPlayers",
                column: "TeamName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamPlayers");
        }
    }
}
