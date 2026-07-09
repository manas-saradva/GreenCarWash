using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GreenCarWash.Api.Migrations
{
    /// <inheritdoc />
    public partial class ConvertAddOnsToSingleFK : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AddOnsJson",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "AddOnId",
                table: "Orders",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_AddOnId",
                table: "Orders",
                column: "AddOnId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_AddOns_AddOnId",
                table: "Orders",
                column: "AddOnId",
                principalTable: "AddOns",
                principalColumn: "AddOnId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_AddOns_AddOnId",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_AddOnId",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "AddOnId",
                table: "Orders");

            migrationBuilder.AddColumn<string>(
                name: "AddOnsJson",
                table: "Orders",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
