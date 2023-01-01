using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace faka.Migrations
{
    /// <inheritdoc />
    public partial class modifytransaction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PaymentId",
                table: "Transaction",
                newName: "GatewayTradeNumber");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "Gateway",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Gateway",
                table: "Transaction");

            migrationBuilder.RenameColumn(
                name: "GatewayTradeNumber",
                table: "Transaction",
                newName: "PaymentId");

            migrationBuilder.AlterColumn<string>(
                name: "PaymentMethod",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
