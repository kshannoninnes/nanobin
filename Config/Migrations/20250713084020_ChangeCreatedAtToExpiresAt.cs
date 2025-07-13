using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nanobin.Config.Migrations
{
    /// <inheritdoc />
    public partial class ChangeCreatedAtToExpiresAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add the new ExpiresAt column
            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "Pastes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            // Step 2: Copy existing CreatedAt data to ExpiresAt + 7 days
            migrationBuilder.Sql("UPDATE Pastes SET ExpiresAt = datetime(CreatedAt, '+7 days')");

            // Step 3: Drop the old CreatedAt column
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Pastes");

        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Step 1: Add the new CreatedAt column
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Pastes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
            
            // Step 2: Copy existing ExpiresAt data to CreatedAt
            migrationBuilder.Sql("UPDATE Pastes SET CreatedAt = datetime(ExpiresAt, '-7 days')");
            
            // Step 3: Drop the old ExpiresAt column
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "Pastes");
        }
    }
}
