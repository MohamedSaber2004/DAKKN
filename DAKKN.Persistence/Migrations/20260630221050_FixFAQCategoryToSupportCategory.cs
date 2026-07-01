using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAKKN.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class FixFAQCategoryToSupportCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportFAQs_SupportFAQCategories_CategoryId",
                schema: "dbo",
                table: "SupportFAQs");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportFAQs_SupportCategories_CategoryId",
                schema: "dbo",
                table: "SupportFAQs",
                column: "CategoryId",
                principalSchema: "dbo",
                principalTable: "SupportCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SupportFAQs_SupportCategories_CategoryId",
                schema: "dbo",
                table: "SupportFAQs");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportFAQs_SupportFAQCategories_CategoryId",
                schema: "dbo",
                table: "SupportFAQs",
                column: "CategoryId",
                principalSchema: "dbo",
                principalTable: "SupportFAQCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}