using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace faka.Migrations
{
    /// <inheritdoc />
    public partial class rename : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedKeys_AspNetUsers_UserId",
                table: "AssignedKeys");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedKeys_Order_OrderId",
                table: "AssignedKeys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignedKeys",
                table: "AssignedKeys");

            migrationBuilder.RenameTable(
                name: "AssignedKeys",
                newName: "AssignedKey");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedKeys_UserId",
                table: "AssignedKey",
                newName: "IX_AssignedKey_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedKeys_OrderId",
                table: "AssignedKey",
                newName: "IX_AssignedKey_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignedKey",
                table: "AssignedKey",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedKey_AspNetUsers_UserId",
                table: "AssignedKey",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedKey_Order_OrderId",
                table: "AssignedKey",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssignedKey_AspNetUsers_UserId",
                table: "AssignedKey");

            migrationBuilder.DropForeignKey(
                name: "FK_AssignedKey_Order_OrderId",
                table: "AssignedKey");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssignedKey",
                table: "AssignedKey");

            migrationBuilder.RenameTable(
                name: "AssignedKey",
                newName: "AssignedKeys");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedKey_UserId",
                table: "AssignedKeys",
                newName: "IX_AssignedKeys_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_AssignedKey_OrderId",
                table: "AssignedKeys",
                newName: "IX_AssignedKeys_OrderId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssignedKeys",
                table: "AssignedKeys",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedKeys_AspNetUsers_UserId",
                table: "AssignedKeys",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AssignedKeys_Order_OrderId",
                table: "AssignedKeys",
                column: "OrderId",
                principalTable: "Order",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
