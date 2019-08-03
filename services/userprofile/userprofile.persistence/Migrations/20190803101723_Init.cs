using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace userprofile.persistence.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "AdminUserModel",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AdminUserModel",
                keyColumn: "Id",
                keyValue: 1L,
                columns: new[] { "DateCreated", "Password" },
                values: new object[] { new DateTime(2019, 8, 3, 10, 17, 22, 345, DateTimeKind.Utc), "admin123@@@***" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Password",
                table: "AdminUserModel");

            migrationBuilder.UpdateData(
                table: "AdminUserModel",
                keyColumn: "Id",
                keyValue: 1L,
                column: "DateCreated",
                value: new DateTime(2019, 8, 3, 10, 9, 25, 607, DateTimeKind.Utc));
        }
    }
}
