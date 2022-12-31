using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace faka.Migrations
{
    /// <inheritdoc />
    public partial class addrelalationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Key_Product_ProductId",
                table: "Key");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Key",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Key_Product_ProductId",
                table: "Key",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Key_Product_ProductId",
                table: "Key");

            migrationBuilder.AlterColumn<int>(
                name: "ProductId",
                table: "Key",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Key_Product_ProductId",
                table: "Key",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id");
        }
    }
}
