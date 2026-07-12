using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAKKN.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class MakeCustomerIdNullable : Migration
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

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                schema: "dbo",
                table: "SupportTickets",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");
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

            migrationBuilder.AlterColumn<Guid>(
                name: "CustomerId",
                schema: "dbo",
                table: "SupportTickets",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
