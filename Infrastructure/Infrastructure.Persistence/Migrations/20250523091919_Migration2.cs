using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nonuso.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Migration2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conversation_ProductRequestId",
                table: "Conversation");

            migrationBuilder.AlterColumn<string>(
                name: "ImagesUrl",
                table: "Product",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_ProductRequestId",
                table: "Conversation",
                column: "ProductRequestId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Conversation_ProductRequestId",
                table: "Conversation");

            migrationBuilder.AlterColumn<string>(
                name: "ImagesUrl",
                table: "Product",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Conversation_ProductRequestId",
                table: "Conversation",
                column: "ProductRequestId",
                unique: true);
        }
    }
}
