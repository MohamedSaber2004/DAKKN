using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAKKN.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddStickerSuggestionsProductFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_StickerSuggestions_ConvertedProductId",
                schema: "dbo",
                table: "StickerSuggestions",
                column: "ConvertedProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_StickerSuggestions_Products_ConvertedProductId",
                schema: "dbo",
                table: "StickerSuggestions",
                column: "ConvertedProductId",
                principalSchema: "dbo",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StickerSuggestions_Products_ConvertedProductId",
                schema: "dbo",
                table: "StickerSuggestions");

            migrationBuilder.DropIndex(
                name: "IX_StickerSuggestions_ConvertedProductId",
                schema: "dbo",
                table: "StickerSuggestions");
        }
    }
}
