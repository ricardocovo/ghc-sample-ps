using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GhcSamplePs.Core.Migrations
{
    /// <summary>
    /// Initial migration that creates the Players table with all columns, constraints, and indexes.
    /// Also seeds 10 sample players for development purposes.
    /// </summary>
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Gender = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    PhotoUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_DateOfBirth",
                table: "Players",
                column: "DateOfBirth");

            migrationBuilder.CreateIndex(
                name: "IX_Players_Name",
                table: "Players",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId",
                table: "Players",
                column: "UserId");

            // Seed data: 10 sample players for development purposes
            // As specified in EFCore_AzureSQL_Repository_Implementation_Specification.md
            var seedDate = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-001", "Emma Rodriguez", new DateTime(2014, 3, 15), "Female", null, seedDate, "system" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-001", "Liam Johnson", new DateTime(2015, 7, 22), "Male", null, seedDate, "system" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-002", "Olivia Martinez", new DateTime(2013, 11, 8), "Female", null, seedDate, "system" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-002", "Noah Williams", new DateTime(2016, 1, 30), "Male", null, seedDate, "system" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-003", "Ava Brown", new DateTime(2014, 9, 12), "Female", null, seedDate, "system" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-003", "Ethan Davis", new DateTime(2015, 4, 5), "Male", null, seedDate, "system" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-004", "Sophia Garcia", new DateTime(2013, 6, 18), "Non-binary", null, seedDate, "system" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-004", "Mason Miller", new DateTime(2016, 12, 25), "Male", null, seedDate, "system" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-005", "Isabella Wilson", new DateTime(2014, 2, 14), "Female", null, seedDate, "system" });

            migrationBuilder.InsertData(
                table: "Players",
                columns: new[] { "UserId", "Name", "DateOfBirth", "Gender", "PhotoUrl", "CreatedAt", "CreatedBy" },
                values: new object[] { "user-005", "Lucas Anderson", new DateTime(2015, 10, 9), "Prefer not to say", null, seedDate, "system" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
