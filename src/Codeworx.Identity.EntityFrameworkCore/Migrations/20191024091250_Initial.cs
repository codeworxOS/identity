using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RightHolder",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Discriminator = table.Column<string>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    RoleId = table.Column<Guid>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true),
                    DefaultTenantId = table.Column<Guid>(nullable: true),
                    IsDisabled = table.Column<bool>(nullable: true),
                    PasswordHash = table.Column<byte[]>(nullable: true),
                    PasswordSalt = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RightHolder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RightHolder_RightHolder_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RightHolder_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUser",
                columns: table => new
                {
                    RightHolderId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUser", x => new { x.RightHolderId, x.TenantId });
                    table.ForeignKey(
                        name: "FK_TenantUser_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUser_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    RoleId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_UserRole_RightHolder_RoleId",
                        column: x => x.RoleId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserRole_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RightHolder_RoleId",
                table: "RightHolder",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RightHolder_TenantId",
                table: "RightHolder",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUser_TenantId",
                table: "TenantUser",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUser_UserId",
                table: "TenantUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TenantUser");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "RightHolder");

            migrationBuilder.DropTable(
                name: "Tenant");
        }
    }
}
