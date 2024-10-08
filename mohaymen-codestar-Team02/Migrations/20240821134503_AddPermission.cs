﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace mohaymen_codestar_Team02.Migrations
{
    /// <inheritdoc />
    public partial class AddPermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "Permissions",
                table: "Roles",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Permissions",
                table: "Roles");
        }
    }
}
