using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAKKN.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class ConvertProductReviewsToBrandReviews : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // 1. Drop FK, old indexes
            migrationBuilder.DropForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                table: "ProductReviews",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_IsFeatured",
                table: "ProductReviews",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_ProductId_UserId",
                table: "ProductReviews",
                schema: "dbo");

            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_UserId",
                table: "ProductReviews",
                schema: "dbo");

            // 2. Drop old columns, add new column
            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductReviews",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "IsFeatured",
                table: "ProductReviews",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "IsHidden",
                table: "ProductReviews",
                schema: "dbo");

            migrationBuilder.AddColumn<bool>(
                name: "IsDisplayed",
                table: "ProductReviews",
                schema: "dbo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // 3. Recreate UserId index, create IsDisplayed index
            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_UserId",
                schema: "dbo",
                table: "ProductReviews",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_IsDisplayed",
                schema: "dbo",
                table: "ProductReviews",
                column: "IsDisplayed");

            // 4. Rename table (this keeps PK/FK/index names intact)
            migrationBuilder.RenameTable(
                name: "ProductReviews",
                schema: "dbo",
                newName: "BrandReviews",
                newSchema: "dbo");

            // 5. Rename indexes using built-in API
            migrationBuilder.RenameIndex(
                name: "IX_ProductReviews_IsApproved",
                table: "BrandReviews",
                newName: "IX_BrandReviews_IsApproved");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReviews_IsDisplayed",
                table: "BrandReviews",
                newName: "IX_BrandReviews_IsDisplayed");

            migrationBuilder.RenameIndex(
                name: "IX_ProductReviews_UserId",
                table: "BrandReviews",
                newName: "IX_BrandReviews_UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // 1. Rename indexes back
            migrationBuilder.RenameIndex(
                name: "IX_BrandReviews_UserId",
                table: "BrandReviews",
                newName: "IX_ProductReviews_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_BrandReviews_IsDisplayed",
                table: "BrandReviews",
                newName: "IX_ProductReviews_IsDisplayed");

            migrationBuilder.RenameIndex(
                name: "IX_BrandReviews_IsApproved",
                table: "BrandReviews",
                newName: "IX_ProductReviews_IsApproved");

            // 2. Rename table back
            migrationBuilder.RenameTable(
                name: "BrandReviews",
                schema: "dbo",
                newName: "ProductReviews",
                newSchema: "dbo");

            // 3. Drop new column, new indexes
            migrationBuilder.DropIndex(
                name: "IX_ProductReviews_IsDisplayed",
                table: "ProductReviews",
                schema: "dbo");

            migrationBuilder.DropColumn(
                name: "IsDisplayed",
                table: "ProductReviews",
                schema: "dbo");

            // 4. Add old columns
            migrationBuilder.AddColumn<Guid>(
                name: "ProductId",
                table: "ProductReviews",
                schema: "dbo",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<bool>(
                name: "IsFeatured",
                table: "ProductReviews",
                schema: "dbo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsHidden",
                table: "ProductReviews",
                schema: "dbo",
                type: "bit",
                nullable: false,
                defaultValue: false);

            // 5. Rebuild old indexes
            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_ProductId_UserId",
                schema: "dbo",
                table: "ProductReviews",
                columns: new[] { "ProductId", "UserId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_IsFeatured",
                schema: "dbo",
                table: "ProductReviews",
                column: "IsFeatured");

            migrationBuilder.CreateIndex(
                name: "IX_ProductReviews_UserId",
                schema: "dbo",
                table: "ProductReviews",
                column: "UserId");

            // 6. Restore FK
            migrationBuilder.AddForeignKey(
                name: "FK_ProductReviews_Products_ProductId",
                schema: "dbo",
                table: "ProductReviews",
                column: "ProductId",
                principalSchema: "dbo",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
