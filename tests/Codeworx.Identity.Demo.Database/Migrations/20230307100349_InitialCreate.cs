using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Codeworx.Identity.Demo.Database.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "__DataMigrationHistory",
                columns: table => new
                {
                    MigrationId = table.Column<string>(type: "TEXT", maxLength: 150, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK___DataMigrationHistory", x => x.MigrationId);
                });

            migrationBuilder.CreateTable(
                name: "ClaimType",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Target = table.Column<int>(type: "INTEGER", nullable: false),
                    TypeKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClaimType", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "License",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_License", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProviderFilter",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Type = table.Column<byte>(type: "INTEGER", nullable: false),
                    DomainName = table.Column<string>(type: "TEXT", nullable: true),
                    RangeEnd = table.Column<byte[]>(type: "BLOB", nullable: true),
                    RangeStart = table.Column<byte[]>(type: "BLOB", nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProviderFilter", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tenant",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    AuthenticationMode = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tenant", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationProvider",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    EndpointConfiguration = table.Column<string>(type: "TEXT", nullable: true),
                    EndpointType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Usage = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    FilterId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    SortOrder = table.Column<int>(type: "INTEGER", nullable: false),
                    IsDisabled = table.Column<bool>(type: "INTEGER", nullable: false),
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LicenseId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Quantity = table.Column<int>(type: "INTEGER", nullable: false),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: false),
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Name = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Type = table.Column<byte>(type: "INTEGER", nullable: false),
                    DefaultTenantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDisabled = table.Column<bool>(type: "INTEGER", nullable: true),
                    ConfirmationPending = table.Column<bool>(type: "INTEGER", nullable: true),
                    ConfirmationCode = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    ForceChangePassword = table.Column<bool>(type: "INTEGER", nullable: true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: true),
                    PasswordChanged = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastFailedLoginAttempt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    FailedLoginCount = table.Column<int>(type: "INTEGER", nullable: true),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    AuthenticationMode = table.Column<int>(type: "INTEGER", nullable: true, defaultValue: 0),
                    Email = table.Column<string>(type: "TEXT", maxLength: 600, nullable: true),
                    FirstName = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                    LastName = table.Column<string>(type: "TEXT", maxLength: 400, nullable: true),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RightHolder", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RightHolder_Tenant_DefaultTenantId",
                        column: x => x.DefaultTenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "AuthenticationProviderRightHolder",
                columns: table => new
                {
                    RightHolderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ProviderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ExternalIdentifier = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    Value = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
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
                        name: "FK_ClaimValue_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClaimValue_Tenant_TenantId",
                        column: x => x.TenantId,
                        principalTable: "Tenant",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ClientConfiguration",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientSecretHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: true),
                    TokenExpiration = table.Column<TimeSpan>(type: "TEXT", nullable: false),
                    AccessTokenType = table.Column<string>(type: "TEXT", maxLength: 50, nullable: true),
                    AccessTokenTypeConfiguration = table.Column<string>(type: "TEXT", nullable: true),
                    AuthenticationMode = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    ClientType = table.Column<int>(type: "INTEGER", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
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
                name: "IdentityCache",
                columns: table => new
                {
                    Key = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: false),
                    Value = table.Column<string>(type: "TEXT", nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CacheType = table.Column<int>(type: "INTEGER", nullable: false),
                    Disabled = table.Column<bool>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityCache", x => x.Key);
                    table.ForeignKey(
                        name: "FK_IdentityCache_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "LicenseAssignment",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LicenseId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    GroupId = table.Column<Guid>(type: "TEXT", nullable: false),
                    RightHolderId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    RightHolderId = table.Column<Guid>(type: "TEXT", nullable: false),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                name: "UserInvitation",
                columns: table => new
                {
                    InvitationCode = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Action = table.Column<int>(type: "INTEGER", nullable: false),
                    RedirectUri = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    ValidUntil = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsDisabled = table.Column<bool>(type: "INTEGER", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserInvitation", x => x.InvitationCode);
                    table.ForeignKey(
                        name: "FK_UserInvitation_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPasswordHistory",
                columns: table => new
                {
                    UserId = table.Column<Guid>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", maxLength: 512, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPasswordHistory", x => new { x.UserId, x.PasswordHash });
                    table.ForeignKey(
                        name: "FK_UserPasswordHistory_RightHolder_UserId",
                        column: x => x.UserId,
                        principalTable: "RightHolder",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ClientLicense",
                columns: table => new
                {
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    LicenseId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ScopeKey = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Type = table.Column<byte>(type: "INTEGER", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: true),
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
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
                    Url = table.Column<string>(type: "TEXT", nullable: true),
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
                    ScopeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClientId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    ScopeId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ClaimTypeId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                    ChildId = table.Column<Guid>(type: "TEXT", nullable: false),
                    ParentId = table.Column<Guid>(type: "TEXT", nullable: false),
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
                name: "IX_AuthenticationProviderRightHolder_ExternalId_Unique",
                table: "AuthenticationProviderRightHolder",
                columns: new[] { "ExternalIdentifier", "ProviderId" },
                unique: true);

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
                name: "IX_IdentityCache_UserId",
                table: "IdentityCache",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_LicenseAssignment_UserId",
                table: "LicenseAssignment",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_RightHolder_DefaultTenantId",
                table: "RightHolder",
                column: "DefaultTenantId");

            migrationBuilder.CreateIndex(
                name: "IX_RightHolder_Name_Unique",
                table: "RightHolder",
                column: "Name",
                unique: true);

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
                name: "IX_UserInvitation_UserId",
                table: "UserInvitation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ValidRedirectUrl_ClientId",
                table: "ValidRedirectUrl",
                column: "ClientId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "__DataMigrationHistory");

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
                name: "UserInvitation");

            migrationBuilder.DropTable(
                name: "UserPasswordHistory");

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
