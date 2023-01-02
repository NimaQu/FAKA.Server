using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace faka.Migrations
{
    /// <inheritdoc />
    public partial class addreleationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gateway",
                table: "Transaction");

            migrationBuilder.AddColumn<int>(
                name: "GatewayId",
                table: "Transaction",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "GatewayId",
                table: "Order",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transaction_GatewayId",
                table: "Transaction",
                column: "GatewayId");

            migrationBuilder.CreateIndex(
                name: "IX_Order_GatewayId",
                table: "Order",
                column: "GatewayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Gateway_GatewayId",
                table: "Order",
                column: "GatewayId",
                principalTable: "Gateway",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Transaction_Gateway_GatewayId",
                table: "Transaction",
                column: "GatewayId",
                principalTable: "Gateway",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_Gateway_GatewayId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Transaction_Gateway_GatewayId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Transaction_GatewayId",
                table: "Transaction");

            migrationBuilder.DropIndex(
                name: "IX_Order_GatewayId",
                table: "Order");

            migrationBuilder.DropColumn(
                name: "GatewayId",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "GatewayId",
                table: "Order");

            migrationBuilder.AddColumn<string>(
                name: "Gateway",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
