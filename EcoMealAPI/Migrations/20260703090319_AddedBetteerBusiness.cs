using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMealAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddedBetteerBusiness : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Business_BusinessTypeId",
                table: "Business",
                column: "BusinessTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Business_BusinessType_BusinessTypeId",
                table: "Business",
                column: "BusinessTypeId",
                principalTable: "BusinessType",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Business_BusinessType_BusinessTypeId",
                table: "Business");

            migrationBuilder.DropIndex(
                name: "IX_Business_BusinessTypeId",
                table: "Business");
        }
    }
}
