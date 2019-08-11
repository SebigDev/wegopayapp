using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace userprofile.persistence.Migrations
{
    public partial class ActivationUpdate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "ActivatedOn",
                table: "UserActivationModel",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AddColumn<DateTime>(
                name: "ExpiresAt",
                table: "UserActivationModel",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.UpdateData(
                table: "AdminUserModel",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateCreated",
                value: new DateTime(2019, 8, 3, 13, 5, 58, 439, DateTimeKind.Utc));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExpiresAt",
                table: "UserActivationModel");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ActivatedOn",
                table: "UserActivationModel",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "AdminUserModel",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateCreated",
                value: new DateTime(2019, 8, 3, 10, 17, 22, 345, DateTimeKind.Utc));
        }
    }
}
