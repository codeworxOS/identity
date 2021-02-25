using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ClaimType",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Target = table.Column<int>(nullable: false),
                    TypeKey = table.Column<string>(maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IdentityCache",
                columns: table => new
                {
                    Key = table.Column<string>(maxLength: 2000, nullable: false),
                    Value = table.Column<string>(nullable: false),
                    ValidUntil = table.Column<DateTime>(nullable: false),
                    CacheType = table.Column<int>(nullable: false),
                    Disabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityCache", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "License",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_License", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderFilter",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    Type = table.Column<byte>(nullable: false),
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
                name: "AuthenticationProvider",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    EndpointConfiguration = table.Column<string>(nullable: true),
                    EndpointType = table.Column<string>(maxLength: 100, nullable: false),
                    FilterId = table.Column<Guid>(nullable: true),
                    Name = table.Column<string>(maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(nullable: false),
                    IsDisabled = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationProvider", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuthenticationProvider_ProviderFilter_FilterId",
                        column: x => x.FilterId,
                        principalTable: "ProviderFilter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AvailableLicense",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    TenantId = table.Column<Guid>(nullable: true),
                    LicenseId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    ValidUntil = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AvailableLicense", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AvailableLicense_License_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "License",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_AvailableLicense_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "RightHolder",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 500, nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    DefaultTenantId = table.Column<Guid>(nullable: true),
                    IsDisabled = table.Column<bool>(nullable: true),
                    ForceChangePassword = table.Column<bool>(nullable: true),
                    Created = table.Column<DateTime>(nullable: true),
                    PasswordChanged = table.Column<DateTime>(nullable: true),
                    LastFailedLoginAttempt = table.Column<DateTime>(nullable: true),
                    FailedLoginCount = table.Column<int>(nullable: true),
                    PasswordHash = table.Column<string>(maxLength: 512, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RightHolder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RightHolder_Tenant_DefaultTenantId",
                        column: x => x.DefaultTenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationProviderRightHolder",
                columns: table => new
                {
                    RightHolderId = table.Column<Guid>(nullable: false),
                    ProviderId = table.Column<Guid>(nullable: false),
                    ExternalIdentifier = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuthenticationProviderRightHolder", x => new { x.RightHolderId, x.ProviderId });
                    table.ForeignKey(
                        name: "FK_AuthenticationProviderRightHolder_AuthenticationProvider_ProviderId",
                        column: x => x.ProviderId,
                        principalTable: "AuthenticationProvider",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AuthenticationProviderRightHolder_RightHolder_RightHolderId",
                        column: x => x.RightHolderId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClaimValue",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClaimTypeId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true),
                    TenantId = table.Column<Guid>(nullable: true),
                    Value = table.Column<string>(maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimValue", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClaimValue_ClaimType_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalTable: "ClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimValue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClaimValue_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ClientSecretHash = table.Column<string>(maxLength: 512, nullable: true),
                    TokenExpiration = table.Column<TimeSpan>(nullable: false),
                    ClientType = table.Column<int>(nullable: false),
                    UserId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientConfiguration", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ClientConfiguration_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LicenseAssignment",
                columns: table => new
                {
                    UserId = table.Column<Guid>(nullable: false),
                    LicenseId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LicenseAssignment", x => new { x.LicenseId, x.UserId });
                    table.ForeignKey(
                        name: "FK_LicenseAssignment_License_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "License",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LicenseAssignment_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RightHolderGroup",
                columns: table => new
                {
                    GroupId = table.Column<Guid>(nullable: false),
                    RightHolderId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RightHolderGroup", x => new { x.RightHolderId, x.GroupId });
                    table.ForeignKey(
                        name: "FK_RightHolderGroup_RightHolder_GroupId",
                        column: x => x.GroupId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_RightHolderGroup_RightHolder_RightHolderId",
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
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TenantUser_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientLicense",
                columns: table => new
                {
                    ClientId = table.Column<Guid>(nullable: false),
                    LicenseId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClientLicense", x => new { x.ClientId, x.LicenseId });
                    table.ForeignKey(
                        name: "FK_ClientLicense_ClientConfiguration_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ClientLicense_License_LicenseId",
                        column: x => x.LicenseId,
                        principalTable: "License",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Scope",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ScopeKey = table.Column<string>(maxLength: 50, nullable: false),
                    Type = table.Column<byte>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Scope", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Scope_ClientConfiguration_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
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
                name: "ScopeAssignment",
                columns: table => new
                {
                    ScopeId = table.Column<Guid>(nullable: false),
                    ClientId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeAssignment", x => new { x.ClientId, x.ScopeId });
                    table.ForeignKey(
                        name: "FK_ScopeAssignment_ClientConfiguration_ClientId",
                        column: x => x.ClientId,
                        principalTable: "ClientConfiguration",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScopeAssignment_Scope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScopeClaim",
                columns: table => new
                {
                    ScopeId = table.Column<Guid>(nullable: false),
                    ClaimTypeId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeClaim", x => new { x.ScopeId, x.ClaimTypeId });
                    table.ForeignKey(
                        name: "FK_ScopeClaim_ClaimType_ClaimTypeId",
                        column: x => x.ClaimTypeId,
                        principalTable: "ClaimType",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScopeClaim_Scope_ScopeId",
                        column: x => x.ScopeId,
                        principalTable: "Scope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ScopeHierarchy",
                columns: table => new
                {
                    ChildId = table.Column<Guid>(nullable: false),
                    ParentId = table.Column<Guid>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ScopeHierarchy", x => x.ChildId);
                    table.ForeignKey(
                        name: "FK_ScopeHierarchy_Scope_ChildId",
                        column: x => x.ChildId,
                        principalTable: "Scope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ScopeHierarchy_Scope_ParentId",
                        column: x => x.ParentId,
                        principalTable: "Scope",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationProvider_FilterId",
                table: "AuthenticationProvider",
                column: "FilterId");

            migrationBuilder.CreateIndex(
                name: "IX_AuthenticationProviderRightHolder_ProviderId",
                table: "AuthenticationProviderRightHolder",
                column: "ProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailableLicense_LicenseId",
                table: "AvailableLicense",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_AvailableLicense_TenantId",
                table: "AvailableLicense",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimValue_ClaimTypeId",
                table: "ClaimValue",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimValue_TenantId",
                table: "ClaimValue",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ClaimValue_UserId",
                table: "ClaimValue",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientConfiguration_UserId",
                table: "ClientConfiguration",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ClientLicense_LicenseId",
                table: "ClientLicense",
                column: "LicenseId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseAssignment_UserId",
                table: "LicenseAssignment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RightHolder_DefaultTenantId",
                table: "RightHolder",
                column: "DefaultTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RightHolderGroup_GroupId",
                table: "RightHolderGroup",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Scope_ClientId",
                table: "Scope",
                column: "ClientId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeAssignment_ScopeId",
                table: "ScopeAssignment",
                column: "ScopeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeClaim_ClaimTypeId",
                table: "ScopeClaim",
                column: "ClaimTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_ScopeHierarchy_ParentId",
                table: "ScopeHierarchy",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_TenantUser_TenantId",
                table: "TenantUser",
                column: "TenantId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidRedirectUrl_ClientId",
                table: "ValidRedirectUrl",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuthenticationProviderRightHolder");

            migrationBuilder.DropTable(
                name: "AvailableLicense");

            migrationBuilder.DropTable(
                name: "ClaimValue");

            migrationBuilder.DropTable(
                name: "ClientLicense");

            migrationBuilder.DropTable(
                name: "IdentityCache");

            migrationBuilder.DropTable(
                name: "LicenseAssignment");

            migrationBuilder.DropTable(
                name: "RightHolderGroup");

            migrationBuilder.DropTable(
                name: "ScopeAssignment");

            migrationBuilder.DropTable(
                name: "ScopeClaim");

            migrationBuilder.DropTable(
                name: "ScopeHierarchy");

            migrationBuilder.DropTable(
                name: "TenantUser");

            migrationBuilder.DropTable(
                name: "ValidRedirectUrl");

            migrationBuilder.DropTable(
                name: "AuthenticationProvider");

            migrationBuilder.DropTable(
                name: "License");

            migrationBuilder.DropTable(
                name: "ClaimType");

            migrationBuilder.DropTable(
                name: "Scope");

            migrationBuilder.DropTable(
                name: "ProviderFilter");

            migrationBuilder.DropTable(
                name: "ClientConfiguration");

            migrationBuilder.DropTable(
                name: "RightHolder");

            migrationBuilder.DropTable(
                name: "Tenant");
        }
    }
}
