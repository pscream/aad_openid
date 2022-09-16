using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace WebApi.Migrations
{
    public partial class Add_External_Users : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "ExternalId",
                table: "IdentUsers",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsExternal",
                table: "IdentUsers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "IdentUsers");

            migrationBuilder.DropColumn(
                name: "IsExternal",
                table: "IdentUsers");
        }
    }
}
