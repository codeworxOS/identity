﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer.Migrations
{
    public partial class ChangeUserNameCollation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RightHolder",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                collation: "sql_latin1_general_cp1_ci_as",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "RightHolder",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldCollation: "sql_latin1_general_cp1_ci_as");
        }
    }
}
