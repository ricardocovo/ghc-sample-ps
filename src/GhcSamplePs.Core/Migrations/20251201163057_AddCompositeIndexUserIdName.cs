using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GhcSamplePs.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddCompositeIndexUserIdName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Players_UserId_Name",
                table: "Players",
                columns: new[] { "UserId", "Name" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Players_UserId_Name",
                table: "Players");
        }
    }
}
