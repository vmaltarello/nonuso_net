using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nonuso.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8821f3d9-fe96-4404-9c2f-8830a043a931"),
                column: "ConcurrencyStamp",
                value: "3E55B188-F822-4BDE-B1CC-96BF79C74797");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9302b5a3-d93b-4152-bd75-9f9ae7e9ff83"),
                column: "ConcurrencyStamp",
                value: "C2A9E755-4A7F-46F2-B36E-364E86DF6DEC");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("aa5334f3-837d-4f65-9ecc-8e471def97e6"),
                column: "ConcurrencyStamp",
                value: "937118E8-A1EA-41DD-8FE7-F47CBDC27FB2");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("8821f3d9-fe96-4404-9c2f-8830a043a931"),
                column: "ConcurrencyStamp",
                value: "65281764-0a8c-4eac-892e-d5f0bf888470");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("9302b5a3-d93b-4152-bd75-9f9ae7e9ff83"),
                column: "ConcurrencyStamp",
                value: "8ee32c87-ab72-4a0e-b7aa-e7872fa38e64");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: new Guid("aa5334f3-837d-4f65-9ecc-8e471def97e6"),
                column: "ConcurrencyStamp",
                value: "365ac856-70d5-461d-8c39-a69654e2cfc7");
        }
    }
}
