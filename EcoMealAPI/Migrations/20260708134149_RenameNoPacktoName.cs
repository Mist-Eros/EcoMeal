using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EcoMealAPI.Migrations
{
    /// <inheritdoc />
    public partial class RenameNoPacktoName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "No_Package",
                table: "Package");

            migrationBuilder.RenameColumn(
                name: "End_Pickup",
                table: "Package",
                newName: "End_PickUp");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Package",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "Package");

            migrationBuilder.RenameColumn(
                name: "End_PickUp",
                table: "Package",
                newName: "End_Pickup");

            migrationBuilder.AddColumn<int>(
                name: "No_Package",
                table: "Package",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
