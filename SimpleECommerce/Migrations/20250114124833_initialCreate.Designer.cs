﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using SimpleECommerce.DataAndContext;

#nullable disable

namespace SimpleECommerce.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20250114124833_initialCreate")]
    partial class initialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "6.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 128);

            SqlServerModelBuilderExtensions.UseIdentityColumns(modelBuilder, 1L, 1);

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRole", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Name")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedName")
                        .IsUnique()
                        .HasDatabaseName("RoleNameIndex")
                        .HasFilter("[NormalizedName] IS NOT NULL");

                    b.ToTable("AspNetRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RoleId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetRoleClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("ClaimType")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("ClaimValue")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserClaims", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderKey")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("ProviderDisplayName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("LoginProvider", "ProviderKey");

                    b.HasIndex("UserId");

                    b.ToTable("AspNetUserLogins", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("RoleId")
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("UserId", "RoleId");

                    b.HasIndex("RoleId");

                    b.ToTable("AspNetUserRoles", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("LoginProvider")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Name")
                        .HasColumnType("nvarchar(450)");

                    b.Property<string>("Value")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("UserId", "LoginProvider", "Name");

                    b.ToTable("AspNetUserTokens", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.Models.ApplicationUser", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("nvarchar(450)");

                    b.Property<int>("AccessFailedCount")
                        .HasColumnType("int");

                    b.Property<string>("ConcurrencyStamp")
                        .IsConcurrencyToken()
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<bool>("EmailConfirmed")
                        .HasColumnType("bit");

                    b.Property<bool>("LockoutEnabled")
                        .HasColumnType("bit");

                    b.Property<DateTimeOffset?>("LockoutEnd")
                        .HasColumnType("datetimeoffset");

                    b.Property<string>("NormalizedEmail")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("NormalizedUserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PhoneNumber")
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("PhoneNumberConfirmed")
                        .HasColumnType("bit");

                    b.Property<string>("SecurityStamp")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("TokenForRessetingPass")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)");

                    b.Property<bool>("TwoFactorEnabled")
                        .HasColumnType("bit");

                    b.Property<string>("UserName")
                        .HasMaxLength(256)
                        .HasColumnType("nvarchar(256)");

                    b.HasKey("Id");

                    b.HasIndex("NormalizedEmail")
                        .HasDatabaseName("EmailIndex");

                    b.HasIndex("NormalizedUserName")
                        .IsUnique()
                        .HasDatabaseName("UserNameIndex")
                        .HasFilter("[NormalizedUserName] IS NOT NULL");

                    b.ToTable("AspNetUsers", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Address", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("BuildingName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("buildingName");

                    b.Property<string>("City")
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("city");

                    b.Property<string>("Country")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("country");

                    b.Property<string>("FullName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("fullName");

                    b.Property<bool>("IsDefault")
                        .HasColumnType("bit")
                        .HasColumnName("isDefault");

                    b.Property<string>("MobileNumber")
                        .IsRequired()
                        .HasMaxLength(20)
                        .HasColumnType("nvarchar(20)")
                        .HasColumnName("mobileNumber");

                    b.Property<string>("StreetName")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("streetName");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("userId");

                    b.HasKey("Id");

                    b.ToTable("Address", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.CartRow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasMaxLength(255)
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ProductVariationId")
                        .HasColumnType("int")
                        .HasColumnName("productVariationId");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("userId");

                    b.HasKey("Id");

                    b.HasIndex("ProductVariationId");

                    b.ToTable("CartRow", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Category", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("Category", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Color", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("Color", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.EmailOtp", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("email");

                    b.Property<string>("Otp")
                        .IsRequired()
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("otp");

                    b.Property<DateTime>("ValidTo")
                        .HasColumnType("datetime2")
                        .HasColumnName("validTo");

                    b.HasKey("Id");

                    b.ToTable("emailOtps", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Order", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("AddressId")
                        .HasColumnType("int")
                        .HasColumnName("addressId");

                    b.Property<DateTime>("DateOfOrder")
                        .HasColumnType("datetime")
                        .HasColumnName("dateOfOrder");

                    b.Property<string>("OrderStatus")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("orderStatus");

                    b.Property<decimal>("TotalAmount")
                        .HasColumnType("decimal(10,2)")
                        .HasColumnName("totalAmount");

                    b.Property<string>("UserId")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("userId");

                    b.HasKey("Id");

                    b.HasIndex("AddressId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.OrderRow", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("OrderId")
                        .HasColumnType("int")
                        .HasColumnName("orderId");

                    b.Property<decimal>("PriceForProduct")
                        .HasColumnType("decimal(10,2)")
                        .HasColumnName("priceForProduct");

                    b.Property<int>("ProductVariationId")
                        .HasColumnType("int")
                        .HasColumnName("productVariationId");

                    b.Property<int>("Quantity")
                        .HasColumnType("int")
                        .HasColumnName("quantity");

                    b.HasKey("Id");

                    b.HasIndex("OrderId");

                    b.HasIndex("ProductVariationId");

                    b.ToTable("OrderRow", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Photo", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Path")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("path");

                    b.Property<int>("ProductVariationId")
                        .HasColumnType("int")
                        .HasColumnName("productVariationId");

                    b.HasKey("Id");

                    b.HasIndex("ProductVariationId");

                    b.ToTable("Photo", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Product", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)")
                        .HasColumnName("description");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasMaxLength(255)
                        .HasColumnType("nvarchar(255)")
                        .HasColumnName("name");

                    b.Property<decimal>("Price")
                        .HasColumnType("decimal(10,2)")
                        .HasColumnName("price");

                    b.Property<int>("categoryId")
                        .HasColumnType("int")
                        .HasColumnName("categoryId");

                    b.Property<bool>("isDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("isDeleted");

                    b.HasKey("Id");

                    b.HasIndex("categoryId");

                    b.ToTable("Product", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.ProductVariation", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<int>("ColorId")
                        .HasColumnType("int")
                        .HasColumnName("colorId");

                    b.Property<string>("MainProductVariationPhoto")
                        .IsRequired()
                        .HasMaxLength(500)
                        .HasColumnType("nvarchar(500)")
                        .HasColumnName("mainProductVariationPhoto");

                    b.Property<int>("ProductId")
                        .HasColumnType("int")
                        .HasColumnName("productId");

                    b.Property<int>("QuantityInStock")
                        .HasColumnType("int")
                        .HasColumnName("quantityInStock");

                    b.Property<byte[]>("RowVersion")
                        .IsConcurrencyToken()
                        .IsRequired()
                        .ValueGeneratedOnAddOrUpdate()
                        .HasColumnType("rowversion")
                        .HasColumnName("rowVersionn");

                    b.Property<int>("SizeId")
                        .HasColumnType("int")
                        .HasColumnName("sizeId");

                    b.Property<string>("Sku")
                        .HasMaxLength(100)
                        .HasColumnType("nvarchar(100)")
                        .HasColumnName("SKU");

                    b.Property<bool>("isDeleted")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bit")
                        .HasDefaultValue(false)
                        .HasColumnName("isDeleted");

                    b.HasKey("Id");

                    b.HasIndex("ColorId");

                    b.HasIndex("ProductId");

                    b.HasIndex("SizeId");

                    b.ToTable("ProductVariation", (string)null);
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Size", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    SqlServerPropertyBuilderExtensions.UseIdentityColumn(b.Property<int>("Id"), 1L, 1);

                    b.Property<string>("Value")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("nvarchar(50)")
                        .HasColumnName("value");

                    b.HasKey("Id");

                    b.ToTable("Size", (string)null);
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityRoleClaim<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserClaim<string>", b =>
                {
                    b.HasOne("SimpleECommerce.DataAndContext.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserLogin<string>", b =>
                {
                    b.HasOne("SimpleECommerce.DataAndContext.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserRole<string>", b =>
                {
                    b.HasOne("Microsoft.AspNetCore.Identity.IdentityRole", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SimpleECommerce.DataAndContext.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("Microsoft.AspNetCore.Identity.IdentityUserToken<string>", b =>
                {
                    b.HasOne("SimpleECommerce.DataAndContext.Models.ApplicationUser", null)
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.CartRow", b =>
                {
                    b.HasOne("SimpleECommerce.DataAndContext.ModelsForEommerce.ProductVariation", "ProductVariation")
                        .WithMany("CartRows")
                        .HasForeignKey("ProductVariationId")
                        .IsRequired()
                        .HasConstraintName("FK__CartRow__product__46E78A0C");

                    b.Navigation("ProductVariation");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Order", b =>
                {
                    b.HasOne("SimpleECommerce.DataAndContext.ModelsForEommerce.Address", "Address")
                        .WithMany("Orders")
                        .HasForeignKey("AddressId")
                        .IsRequired()
                        .HasConstraintName("FK__Orders__addressI__49C3F6B7");

                    b.Navigation("Address");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.OrderRow", b =>
                {
                    b.HasOne("SimpleECommerce.DataAndContext.ModelsForEommerce.Order", "Order")
                        .WithMany("OrderRows")
                        .HasForeignKey("OrderId")
                        .IsRequired()
                        .HasConstraintName("FK__OrderRow__orderI__4CA06362");

                    b.HasOne("SimpleECommerce.DataAndContext.ModelsForEommerce.ProductVariation", "ProductVariation")
                        .WithMany("OrderRows")
                        .HasForeignKey("ProductVariationId")
                        .IsRequired()
                        .HasConstraintName("FK__OrderRow__produc__4D94879B");

                    b.Navigation("Order");

                    b.Navigation("ProductVariation");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Photo", b =>
                {
                    b.HasOne("SimpleECommerce.DataAndContext.ModelsForEommerce.ProductVariation", "ProductVariation")
                        .WithMany("Photos")
                        .HasForeignKey("ProductVariationId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired()
                        .HasConstraintName("FK_Photo_ProductVariation");

                    b.Navigation("ProductVariation");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Product", b =>
                {
                    b.HasOne("SimpleECommerce.DataAndContext.ModelsForEommerce.Category", "Category")
                        .WithMany("Products")
                        .HasForeignKey("categoryId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired()
                        .HasConstraintName("FK_Product_Category");

                    b.Navigation("Category");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.ProductVariation", b =>
                {
                    b.HasOne("SimpleECommerce.DataAndContext.ModelsForEommerce.Color", "Color")
                        .WithMany("ProductVariations")
                        .HasForeignKey("ColorId")
                        .IsRequired()
                        .HasConstraintName("FK__ProductVa__color__3E52440B");

                    b.HasOne("SimpleECommerce.DataAndContext.ModelsForEommerce.Product", "Product")
                        .WithMany("ProductVariations")
                        .HasForeignKey("ProductId")
                        .IsRequired()
                        .HasConstraintName("FK__ProductVa__produ__3D5E1FD2");

                    b.HasOne("SimpleECommerce.DataAndContext.ModelsForEommerce.Size", "Size")
                        .WithMany("ProductVariations")
                        .HasForeignKey("SizeId")
                        .IsRequired()
                        .HasConstraintName("FK__ProductVa__sizeI__3F466844");

                    b.Navigation("Color");

                    b.Navigation("Product");

                    b.Navigation("Size");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Address", b =>
                {
                    b.Navigation("Orders");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Category", b =>
                {
                    b.Navigation("Products");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Color", b =>
                {
                    b.Navigation("ProductVariations");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Order", b =>
                {
                    b.Navigation("OrderRows");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Product", b =>
                {
                    b.Navigation("ProductVariations");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.ProductVariation", b =>
                {
                    b.Navigation("CartRows");

                    b.Navigation("OrderRows");

                    b.Navigation("Photos");
                });

            modelBuilder.Entity("SimpleECommerce.DataAndContext.ModelsForEommerce.Size", b =>
                {
                    b.Navigation("ProductVariations");
                });
#pragma warning restore 612, 618
        }
    }
}
