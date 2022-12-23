using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace faka.Migrations
{
    /// <inheritdoc />
    public partial class addupdateat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Bought");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Key",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Key",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Key");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Key");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Bought",
                type: "datetime2",
                nullable: true);
        }
    }
}
