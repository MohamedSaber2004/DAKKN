using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAKKN.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovePrimaryColorFromUserSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.UserSettings', 'PrimaryColor') IS NOT NULL
                BEGIN
                    ALTER TABLE [dbo].[UserSettings] DROP COLUMN [PrimaryColor]
                END
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "PrimaryColor",
                schema: "dbo",
                table: "UserSettings",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "#3B82F6");
        }
    }
}
