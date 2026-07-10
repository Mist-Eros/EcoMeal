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
            migrationBuilder.Sql(
                @"IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'No_Package'
                      AND Object_ID = Object_ID(N'[dbo].[Package]')
                )
                BEGIN
                    ALTER TABLE [dbo].[Package] DROP COLUMN [No_Package];
                END");

            migrationBuilder.Sql(
                @"IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'End_Pickup'
                      AND Object_ID = Object_ID(N'[dbo].[Package]')
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'End_PickUp'
                      AND Object_ID = Object_ID(N'[dbo].[Package]')
                )
                BEGIN
                    EXEC sp_rename N'[Package].[End_Pickup]', N'End_PickUp', 'COLUMN';
                END");

            migrationBuilder.Sql(
                @"IF NOT EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'Name'
                      AND Object_ID = Object_ID(N'[dbo].[Package]')
                )
                BEGIN
                    ALTER TABLE [dbo].[Package] ADD [Name] nvarchar(max) NOT NULL CONSTRAINT [DF_Package_Name] DEFAULT N'';
                END");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                @"IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'Name'
                      AND Object_ID = Object_ID(N'[dbo].[Package]')
                )
                BEGIN
                    ALTER TABLE [dbo].[Package] DROP COLUMN [Name];
                END");

            migrationBuilder.Sql(
                @"IF EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'End_PickUp'
                      AND Object_ID = Object_ID(N'[dbo].[Package]')
                )
                AND NOT EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'End_Pickup'
                      AND Object_ID = Object_ID(N'[dbo].[Package]')
                )
                BEGIN
                    EXEC sp_rename N'[Package].[End_PickUp]', N'End_Pickup', 'COLUMN';
                END");

            migrationBuilder.Sql(
                @"IF NOT EXISTS (
                    SELECT 1
                    FROM sys.columns
                    WHERE Name = N'No_Package'
                      AND Object_ID = Object_ID(N'[dbo].[Package]')
                )
                BEGIN
                    ALTER TABLE [dbo].[Package] ADD [No_Package] int NOT NULL CONSTRAINT [DF_Package_No_Package] DEFAULT 0;
                END");
        }
    }
}
