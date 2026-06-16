using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAKKN.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class updateApplicationUserTbl : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GoogleUserId",
                schema: "dbo",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GoogleUserId",
                schema: "dbo",
                table: "AspNetUsers");
        }
    }
}
