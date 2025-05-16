using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nonuso.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Update1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastReadAt",
                table: "MessageInfo");

            migrationBuilder.DropColumn(
                name: "DeletedForReceiver",
                table: "Message");

            migrationBuilder.DropColumn(
                name: "DeletedForSender",
                table: "Message");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastReadAt",
                table: "MessageInfo",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "DeletedForReceiver",
                table: "Message",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "DeletedForSender",
                table: "Message",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
