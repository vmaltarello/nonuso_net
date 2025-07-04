using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nonuso.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateConstraintOnReview : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Review_ReviewerUserId",
                table: "Review");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ReviewerUserId_ReviewedUserId_ProductRequestId",
                table: "Review",
                columns: new[] { "ReviewerUserId", "ReviewedUserId", "ProductRequestId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Review_ReviewerUserId_ReviewedUserId_ProductRequestId",
                table: "Review");

            migrationBuilder.CreateIndex(
                name: "IX_Review_ReviewerUserId",
                table: "Review",
                column: "ReviewerUserId");
        }
    }
}
