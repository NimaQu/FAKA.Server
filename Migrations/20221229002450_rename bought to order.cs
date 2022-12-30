using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace faka.Migrations
{
    /// <inheritdoc />
    public partial class renameboughttoorder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Bought_AspNetUsers_UserId",
                table: "Bought");

            migrationBuilder.DropForeignKey(
                name: "FK_Bought_Product_ProductId",
                table: "Bought");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Bought",
                table: "Bought");

            migrationBuilder.RenameTable(
                name: "Bought",
                newName: "Order");

            migrationBuilder.RenameIndex(
                name: "IX_Bought_UserId",
                table: "Order",
                newName: "IX_Order_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Bought_ProductId",
                table: "Order",
                newName: "IX_Order_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Order",
                table: "Order",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_AspNetUsers_UserId",
                table: "Order",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Order_Product_ProductId",
                table: "Order",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Order_AspNetUsers_UserId",
                table: "Order");

            migrationBuilder.DropForeignKey(
                name: "FK_Order_Product_ProductId",
                table: "Order");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Order",
                table: "Order");

            migrationBuilder.RenameTable(
                name: "Order",
                newName: "Bought");

            migrationBuilder.RenameIndex(
                name: "IX_Order_UserId",
                table: "Bought",
                newName: "IX_Bought_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Order_ProductId",
                table: "Bought",
                newName: "IX_Bought_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Bought",
                table: "Bought",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bought_AspNetUsers_UserId",
                table: "Bought",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Bought_Product_ProductId",
                table: "Bought",
                column: "ProductId",
                principalTable: "Product",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
