﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite.Migrations
{
    [DbContext(typeof(CodeworxIdentityDbContext))]
    partial class CodeworxIdentityDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.0")
                .HasAnnotation("PropertyAccessMode", PropertyAccessMode.Property);

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.AuthenticationProvider", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("EndpointConfiguration")
                        .HasColumnType("TEXT");

                    b.Property<string>("EndpointType")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(100);

                    b.Property<Guid?>("FilterId")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<int>("SortOrder")
                        .HasColumnType("INTEGER");

                    b.Property<int>("Usage")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(1);

                    b.HasKey("Id");

                    b.HasIndex("FilterId");

                    b.ToTable("AuthenticationProvider");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.AuthenticationProviderRightHolder", b =>
                {
                    b.Property<Guid>("RightHolderId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ProviderId")
                        .HasColumnType("TEXT");

                    b.Property<string>("ExternalIdentifier")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(4000);

                    b.HasKey("RightHolderId", "ProviderId");

                    b.HasIndex("ExternalIdentifier")
                        .IsUnique()
                        .HasName("IX_AuthenticationProviderRightHolder_ExternalId_Unique");

                    b.HasIndex("ProviderId");

                    b.ToTable("AuthenticationProviderRightHolder");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.AvailableLicense", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LicenseId")
                        .HasColumnType("TEXT");

                    b.Property<int>("Quantity")
                        .HasColumnType("INTEGER");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("LicenseId");

                    b.HasIndex("TenantId");

                    b.ToTable("AvailableLicense");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ClaimType", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("Target")
                        .HasColumnType("INTEGER");

                    b.Property<string>("TypeKey")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.HasKey("Id");

                    b.ToTable("ClaimType");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ClaimValue", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ClaimTypeId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("TenantId")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .HasColumnType("TEXT")
                        .HasMaxLength(500);

                    b.HasKey("Id");

                    b.HasIndex("ClaimTypeId");

                    b.HasIndex("TenantId");

                    b.HasIndex("UserId");

                    b.ToTable("ClaimValue");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ClientConfiguration", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("AuthenticationMode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<string>("ClientSecretHash")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.Property<int>("ClientType")
                        .HasColumnType("INTEGER");

                    b.Property<TimeSpan>("TokenExpiration")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("ClientConfiguration");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ClientLicense", b =>
                {
                    b.Property<Guid>("ClientId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("LicenseId")
                        .HasColumnType("TEXT");

                    b.HasKey("ClientId", "LicenseId");

                    b.HasIndex("LicenseId");

                    b.ToTable("ClientLicense");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.IdentityCache", b =>
                {
                    b.Property<string>("Key")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.Property<int>("CacheType")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("Disabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("TEXT");

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Key");

                    b.ToTable("IdentityCache");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.License", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("License");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.LicenseAssignment", b =>
                {
                    b.Property<Guid>("LicenseId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.HasKey("LicenseId", "UserId");

                    b.HasIndex("UserId");

                    b.ToTable("LicenseAssignment");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ProviderFilter", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.Property<byte>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("ProviderFilter");

                    b.HasDiscriminator<byte>("Type");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.RightHolder", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(500);

                    b.Property<byte>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("Name")
                        .IsUnique()
                        .HasName("IX_RightHolder_Name_Unique");

                    b.ToTable("RightHolder");

                    b.HasDiscriminator<byte>("Type");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.RightHolderGroup", b =>
                {
                    b.Property<Guid>("RightHolderId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("GroupId")
                        .HasColumnType("TEXT");

                    b.HasKey("RightHolderId", "GroupId");

                    b.HasIndex("GroupId");

                    b.ToTable("RightHolderGroup");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.Scope", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<string>("ScopeKey")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(50);

                    b.Property<byte>("Type")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.ToTable("Scope");

                    b.HasDiscriminator<byte>("Type").HasValue((byte)1);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ScopeAssignment", b =>
                {
                    b.Property<Guid>("ClientId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ScopeId")
                        .HasColumnType("TEXT");

                    b.HasKey("ClientId", "ScopeId");

                    b.HasIndex("ScopeId");

                    b.ToTable("ScopeAssignment");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ScopeClaim", b =>
                {
                    b.Property<Guid>("ScopeId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ClaimTypeId")
                        .HasColumnType("TEXT");

                    b.HasKey("ScopeId", "ClaimTypeId");

                    b.HasIndex("ClaimTypeId");

                    b.ToTable("ScopeClaim");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ScopeHierarchy", b =>
                {
                    b.Property<Guid>("ChildId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ParentId")
                        .HasColumnType("TEXT");

                    b.HasKey("ChildId");

                    b.HasIndex("ParentId");

                    b.ToTable("ScopeHierarchy");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.Tenant", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<int>("AuthenticationMode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT")
                        .HasMaxLength(200);

                    b.HasKey("Id");

                    b.ToTable("Tenant");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.TenantUser", b =>
                {
                    b.Property<Guid>("RightHolderId")
                        .HasColumnType("TEXT");

                    b.Property<Guid>("TenantId")
                        .HasColumnType("TEXT");

                    b.HasKey("RightHolderId", "TenantId");

                    b.HasIndex("TenantId");

                    b.ToTable("TenantUser");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.UserInvitation", b =>
                {
                    b.Property<string>("InvitationCode")
                        .HasColumnType("TEXT")
                        .HasMaxLength(4000);

                    b.Property<int>("Action")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("INTEGER");

                    b.Property<string>("RedirectUri")
                        .HasColumnType("TEXT")
                        .HasMaxLength(2000);

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("TEXT");

                    b.HasKey("InvitationCode");

                    b.HasIndex("UserId");

                    b.ToTable("UserInvitation");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.UserPasswordHistory", b =>
                {
                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.Property<DateTime>("ChangedAt")
                        .HasColumnType("TEXT");

                    b.HasKey("UserId", "PasswordHash");

                    b.ToTable("UserPasswordHistory");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.UserRefreshToken", b =>
                {
                    b.Property<string>("Token")
                        .HasColumnType("TEXT")
                        .HasMaxLength(4000);

                    b.Property<Guid>("ClientId")
                        .HasColumnType("TEXT");

                    b.Property<string>("IdentityData")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("INTEGER");

                    b.Property<Guid>("UserId")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("ValidUntil")
                        .HasColumnType("TEXT");

                    b.HasKey("Token");

                    b.HasIndex("ClientId");

                    b.HasIndex("UserId");

                    b.ToTable("UserRefreshToken");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ValidRedirectUrl", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("TEXT");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("TEXT");

                    b.Property<string>("Url")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("ClientId");

                    b.ToTable("ValidRedirectUrl");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.DomainNameProviderFilter", b =>
                {
                    b.HasBaseType("Codeworx.Identity.EntityFrameworkCore.Model.ProviderFilter");

                    b.Property<string>("DomainName")
                        .HasColumnType("TEXT");

                    b.HasDiscriminator().HasValue((byte)1);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.IPv4ProviderFilter", b =>
                {
                    b.HasBaseType("Codeworx.Identity.EntityFrameworkCore.Model.ProviderFilter");

                    b.Property<byte[]>("RangeEnd")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.Property<byte[]>("RangeStart")
                        .IsRequired()
                        .HasColumnType("BLOB");

                    b.HasDiscriminator().HasValue((byte)2);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.Group", b =>
                {
                    b.HasBaseType("Codeworx.Identity.EntityFrameworkCore.Model.RightHolder");

                    b.HasDiscriminator().HasValue((byte)2);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.User", b =>
                {
                    b.HasBaseType("Codeworx.Identity.EntityFrameworkCore.Model.RightHolder");

                    b.Property<int>("AuthenticationMode")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER")
                        .HasDefaultValue(0);

                    b.Property<string>("ConfirmationCode")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.Property<bool>("ConfirmationPending")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("Created")
                        .HasColumnType("TEXT");

                    b.Property<Guid?>("DefaultTenantId")
                        .HasColumnType("TEXT");

                    b.Property<int>("FailedLoginCount")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("ForceChangePassword")
                        .HasColumnType("INTEGER");

                    b.Property<bool>("IsDisabled")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime?>("LastFailedLoginAttempt")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("PasswordChanged")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT")
                        .HasMaxLength(512);

                    b.HasIndex("DefaultTenantId");

                    b.HasDiscriminator().HasValue((byte)1);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ClientScope", b =>
                {
                    b.HasBaseType("Codeworx.Identity.EntityFrameworkCore.Model.Scope");

                    b.Property<Guid>("ClientId")
                        .HasColumnType("TEXT");

                    b.HasIndex("ClientId");

                    b.HasDiscriminator().HasValue((byte)2);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.AuthenticationProvider", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.ProviderFilter", "Filter")
                        .WithMany()
                        .HasForeignKey("FilterId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.AuthenticationProviderRightHolder", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.AuthenticationProvider", "Provider")
                        .WithMany("RightHolders")
                        .HasForeignKey("ProviderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.RightHolder", "RightHolder")
                        .WithMany("Providers")
                        .HasForeignKey("RightHolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.AvailableLicense", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.License", "License")
                        .WithMany()
                        .HasForeignKey("LicenseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ClaimValue", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.ClaimType", "ClaimType")
                        .WithMany()
                        .HasForeignKey("ClaimTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.Tenant", "Tenant")
                        .WithMany()
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Restrict);

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ClientConfiguration", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.User", "User")
                        .WithMany("Clients")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Restrict);
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ClientLicense", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.ClientConfiguration", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.License", "License")
                        .WithMany("Clients")
                        .HasForeignKey("LicenseId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.LicenseAssignment", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.License", "License")
                        .WithMany()
                        .HasForeignKey("LicenseId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.RightHolderGroup", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.Group", "Group")
                        .WithMany("Members")
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.RightHolder", "RightHolder")
                        .WithMany("MemberOf")
                        .HasForeignKey("RightHolderId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ScopeAssignment", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.ClientConfiguration", "Client")
                        .WithMany("ScopeAssignments")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.Scope", "Scope")
                        .WithMany("ScopeAssignments")
                        .HasForeignKey("ScopeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ScopeClaim", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.ClaimType", "ClaimType")
                        .WithMany()
                        .HasForeignKey("ClaimTypeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.Scope", "Scope")
                        .WithMany("Claims")
                        .HasForeignKey("ScopeId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ScopeHierarchy", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.Scope", "Child")
                        .WithOne("Parent")
                        .HasForeignKey("Codeworx.Identity.EntityFrameworkCore.Model.ScopeHierarchy", "ChildId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.Scope", "Parent")
                        .WithMany("Children")
                        .HasForeignKey("ParentId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.TenantUser", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.User", "User")
                        .WithMany("Tenants")
                        .HasForeignKey("RightHolderId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.Tenant", "Tenant")
                        .WithMany("Users")
                        .HasForeignKey("TenantId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.UserInvitation", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.User", "User")
                        .WithMany("Invitations")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.UserPasswordHistory", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.User", "User")
                        .WithMany("PasswordHistory")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.UserRefreshToken", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.ClientConfiguration", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.User", "User")
                        .WithMany("RefreshTokens")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ValidRedirectUrl", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.ClientConfiguration", "Client")
                        .WithMany("ValidRedirectUrls")
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.User", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.Tenant", "DefaultTenant")
                        .WithMany()
                        .HasForeignKey("DefaultTenantId");
                });

            modelBuilder.Entity("Codeworx.Identity.EntityFrameworkCore.Model.ClientScope", b =>
                {
                    b.HasOne("Codeworx.Identity.EntityFrameworkCore.Model.ClientConfiguration", "Client")
                        .WithMany()
                        .HasForeignKey("ClientId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();
                });
#pragma warning restore 612, 618
        }
    }
}
