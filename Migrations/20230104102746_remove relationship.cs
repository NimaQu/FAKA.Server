using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace faka.Migrations
{
    /// <inheritdoc />
    public partial class removerelationship : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Gateway_ProductGroup_ProductGroupId",
                table: "Gateway");

            migrationBuilder.DropIndex(
                name: "IX_Gateway_ProductGroupId",
                table: "Gateway");

            migrationBuilder.DropColumn(
                name: "ProductGroupId",
                table: "Gateway");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductGroupId",
                table: "Gateway",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Gateway_ProductGroupId",
                table: "Gateway",
                column: "ProductGroupId");

            migrationBuilder.AddForeignKey(
                name: "FK_Gateway_ProductGroup_ProductGroupId",
                table: "Gateway",
                column: "ProductGroupId",
                principalTable: "ProductGroup",
                principalColumn: "Id");
        }
    }
}
