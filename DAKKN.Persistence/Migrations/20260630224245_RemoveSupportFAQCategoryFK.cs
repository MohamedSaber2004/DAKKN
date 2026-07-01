using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAKKN.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveSupportFAQCategoryFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // The SupportFAQCategoryId column and FK were already removed in a previous migration
            // This migration only updates the model snapshot to remove the shadow property
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SupportFAQCategoryId",
                schema: "dbo",
                table: "SupportFAQs",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportFAQs_SupportFAQCategoryId",
                schema: "dbo",
                table: "SupportFAQs",
                column: "SupportFAQCategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_SupportFAQs_SupportFAQCategories_SupportFAQCategoryId",
                schema: "dbo",
                table: "SupportFAQs",
                column: "SupportFAQCategoryId",
                principalSchema: "dbo",
                principalTable: "SupportFAQCategories",
                principalColumn: "Id");
        }
    }
}