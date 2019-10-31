using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations
{
    public partial class ClientRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientSecret = table.Column<byte[]>(nullable: true),
                    ClientSecretSalt = table.Column<byte[]>(nullable: true),
                    DefaultRedirectUri = table.Column<string>(nullable: true),
                    FlowTypes = table.Column<int>(nullable: false),
                    TokenExpiration = table.Column<TimeSpan>(nullable: false),
                    ValidRedirectUrls = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientConfiguration", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClientConfiguration");
        }
    }
}