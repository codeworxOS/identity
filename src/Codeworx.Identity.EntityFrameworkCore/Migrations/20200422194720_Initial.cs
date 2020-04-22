using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClientConfiguration",
                columns: table => new
                {
                    ClientSecret = table.Column<byte[]>(nullable: true),
                    ClientSecretSalt = table.Column<byte[]>(nullable: true),
                    DefaultRedirectUri = table.Column<string>(nullable: true),
                    FlowTypes = table.Column<int>(nullable: false),
                    Id = table.Column<Guid>(nullable: false),
                    TokenExpiration = table.Column<TimeSpan>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientConfiguration", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderFilter",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<string>(nullable: false),
                    DomainName = table.Column<string>(nullable: true),
                    RangeEnd = table.Column<byte[]>(nullable: true),
                    RangeStart = table.Column<byte[]>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderFilter", x => x.Id);
                });

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
                name: "ValidRedirectUrl",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false),
                    Url = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidRedirectUrl", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidRedirectUrl_ClientConfiguration_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ExternalAuthenticationProvider",
                columns: table => new
                {
                    EndpointConfiguration = table.Column<string>(nullable: true),
                    EndpointType = table.Column<string>(maxLength: 100, nullable: false),
                    FilterId = table.Column<Guid>(nullable: true),
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalAuthenticationProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalAuthenticationProvider_ProviderFilter_FilterId",
                        column: x => x.FilterId,
                        principalTable: "ProviderFilter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RightHolder",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    RoleId = table.Column<Guid>(nullable: true),
                    Type = table.Column<string>(nullable: false),
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
                    table.ForeignKey(
                        name: "FK_RightHolder_Tenant_DefaultTenantId",
                        column: x => x.DefaultTenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationProviderUser",
                columns: table => new
                {
                    RightHolderId = table.Column<Guid>(nullable: false),
                    ProviderId = table.Column<Guid>(nullable: false),
                    ExternalIdentifier = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationProviderUser", x => new { x.RightHolderId, x.ProviderId });
                    table.ForeignKey(
                        name: "FK_AuthenticationProviderUser_ExternalAuthenticationProvider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "ExternalAuthenticationProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthenticationProviderUser_RightHolder_RightHolderId",
                        column: x => x.RightHolderId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TenantUser",
                columns: table => new
                {
                    RightHolderId = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TenantUser", x => new { x.RightHolderId, x.TenantId });
                    table.ForeignKey(
                        name: "FK_TenantUser_RightHolder_RightHolderId",
                        column: x => x.RightHolderId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TenantUser_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserRole",
                columns: table => new
                {
                    RoleId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false)
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
                name: "IX_AuthenticationProviderUser_ProviderId",
                table: "AuthenticationProviderUser",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalAuthenticationProvider_FilterId",
                table: "ExternalAuthenticationProvider",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_RightHolder_RoleId",
                table: "RightHolder",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_RightHolder_TenantId",
                table: "RightHolder",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RightHolder_DefaultTenantId",
                table: "RightHolder",
                column: "DefaultTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUser_TenantId",
                table: "TenantUser",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_UserRole_RoleId",
                table: "UserRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidRedirectUrl_ClientId",
                table: "ValidRedirectUrl",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationProviderUser");

            migrationBuilder.DropTable(
                name: "TenantUser");

            migrationBuilder.DropTable(
                name: "UserRole");

            migrationBuilder.DropTable(
                name: "ValidRedirectUrl");

            migrationBuilder.DropTable(
                name: "ExternalAuthenticationProvider");

            migrationBuilder.DropTable(
                name: "RightHolder");

            migrationBuilder.DropTable(
                name: "ClientConfiguration");

            migrationBuilder.DropTable(
                name: "ProviderFilter");

            migrationBuilder.DropTable(
                name: "Tenant");
        }
    }
}
