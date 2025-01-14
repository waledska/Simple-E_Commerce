using SimpleECommerce.DataAndContext.Models;
using SimpleECommerce.DataAndContext.ModelsForEommerce;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace SimpleECommerce.DataAndContext
{
    public partial class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) :
            base(options)
        {
        }
        // adding dbsets for the new tables that we will insert in this identity context 
        public virtual DbSet<Address> Addresses { get; set; } = null!;
        public virtual DbSet<CartRow> CartRows { get; set; } = null!;
        public virtual DbSet<Color> Colors { get; set; } = null!;
        public virtual DbSet<EmailOtp> emailOtps { get; set; } = null!;
        public virtual DbSet<Order> Orders { get; set; } = null!;
        public virtual DbSet<OrderRow> OrderRows { get; set; } = null!;
        public virtual DbSet<Product> Products { get; set; } = null!;
        public virtual DbSet<ProductVariation> ProductVariations { get; set; } = null!;
        public virtual DbSet<Size> Sizes { get; set; } = null!;
        public virtual DbSet<Photo> Photos { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;


        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {

            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // the old constructor that define the authentication Models and theire relations
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Address>(entity =>
            {
                entity.ToTable("Address");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.BuildingName)
                    .HasMaxLength(255)
                    .HasColumnName("buildingName");

                entity.Property(e => e.City)
                    .HasMaxLength(255)
                    .HasColumnName("city");

                entity.Property(e => e.Country)
                    .HasMaxLength(100)
                    .HasColumnName("country");

                entity.Property(e => e.FullName)
                    .HasMaxLength(255)
                    .HasColumnName("fullName");

                entity.Property(e => e.IsDefault)
                    .IsRequired()
                    .HasColumnName("isDefault");

                entity.Property(e => e.MobileNumber)
                    .HasMaxLength(20)
                    .HasColumnName("mobileNumber");

                entity.Property(e => e.StreetName)
                    .HasMaxLength(255)
                    .HasColumnName("streetName");

                entity.Property(e => e.UserId).HasColumnName("userId");
            });

            modelBuilder.Entity<CartRow>(entity =>
            {
                entity.ToTable("CartRow");

                entity.Property(e => e.Id)
                .HasMaxLength(255)
                .HasColumnName("id");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.Property(e => e.ProductVariationId).HasColumnName("productVariationId");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.ProductVariation)
                    .WithMany(p => p.CartRows)
                    .HasForeignKey(d => d.ProductVariationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CartRow__product__46E78A0C");
            });

            modelBuilder.Entity<Color>(entity =>
            {
                entity.ToTable("Color");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Value)
                    .HasMaxLength(50)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToTable("Category");

                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

                entity.Property(e => e.Value)
                    .HasMaxLength(50)
                    .IsRequired()
                    .HasColumnName("value");

                // Configure one-to-many relationship with Product
                entity.HasMany(c => c.Products) // Category has many Products
                    .WithOne(p => p.Category)  // Product has one Category
                    .HasForeignKey(p => p.categoryId) // Foreign key in Product
                    .OnDelete(DeleteBehavior.Restrict) // Restrict deletion of Category if Products exist
                    .HasConstraintName("FK_Product_Category");
            });

            modelBuilder.Entity<EmailOtp>(entity =>
            {
                entity.ToTable("emailOtps");

                entity.Property(e => e.Email).HasColumnName("email");

                entity.Property(e => e.Otp).HasColumnName("otp");

                entity.Property(e => e.ValidTo).HasColumnName("validTo");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.AddressId).HasColumnName("addressId");

                entity.Property(e => e.DateOfOrder)
                    .HasColumnType("datetime")
                    .HasColumnName("dateOfOrder");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(50)
                    .HasColumnName("orderStatus");

                entity.Property(e => e.TotalAmount)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("totalAmount");

                entity.Property(e => e.UserId).HasColumnName("userId");

                entity.HasOne(d => d.Address)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.AddressId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Orders__addressI__49C3F6B7");
            });

            modelBuilder.Entity<OrderRow>(entity =>
            {
                entity.ToTable("OrderRow");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.OrderId).HasColumnName("orderId");

                entity.Property(e => e.PriceForProduct)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("priceForProduct");

                entity.Property(e => e.ProductVariationId).HasColumnName("productVariationId");

                entity.Property(e => e.Quantity).HasColumnName("quantity");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderRows)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderRow__orderI__4CA06362");

                entity.HasOne(d => d.ProductVariation)
                    .WithMany(p => p.OrderRows)
                    .HasForeignKey(d => d.ProductVariationId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderRow__produc__4D94879B");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("Product");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description).HasColumnName("description");

                entity.Property(e => e.Name)
                    .HasMaxLength(255)
                    .HasColumnName("name");

                entity.Property(e => e.Price)
                    .HasColumnType("decimal(10, 2)")
                    .HasColumnName("price");


                // New column configuration for isDeleted
                entity.Property(e => e.isDeleted)
                    .IsRequired()
                    .HasColumnName("isDeleted")
                    .HasDefaultValue(false);

                // New column configuration for categoryId
                entity.Property(e => e.categoryId).HasColumnName("categoryId");

                // Configure foreign key relationship with Category
                entity.HasOne(p => p.Category) // Product has one Category
                    .WithMany(c => c.Products) // Category has many Products
                    .HasForeignKey(p => p.categoryId) // Foreign key in Product
                    .OnDelete(DeleteBehavior.Restrict) // Prevent deleting Category if Products exist
                    .HasConstraintName("FK_Product_Category");
            });

            modelBuilder.Entity<ProductVariation>(entity =>
            {
                entity.ToTable("ProductVariation");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.ColorId).HasColumnName("colorId");

                entity.Property(e => e.ProductId).HasColumnName("productId");

                entity.Property(e => e.QuantityInStock).HasColumnName("quantityInStock");

                entity.Property(e => e.SizeId).HasColumnName("sizeId");

                // New column configuration for MainProductVariationPhoto
                entity.Property(e => e.MainProductVariationPhoto)
                    .IsRequired() // Not nullable
                    .HasMaxLength(500)
                    .HasColumnName("mainProductVariationPhoto");

                // New column configuration for isDeleted
                entity.Property(e => e.isDeleted)
                    .IsRequired()
                    .HasColumnName("isDeleted")
                    .HasDefaultValue(false);

                entity.Property(e => e.Sku)
                    .HasMaxLength(100)
                    .HasColumnName("SKU");

                // Configure RowVersion as concurrency token
                entity.Property(e => e.RowVersion)
                    .HasColumnName("rowVersionn")
                    .IsRowVersion()
                    .IsConcurrencyToken();

                entity.HasOne(d => d.Color)
                    .WithMany(p => p.ProductVariations)
                    .HasForeignKey(d => d.ColorId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductVa__color__3E52440B");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductVariations)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductVa__produ__3D5E1FD2");

                entity.HasOne(d => d.Size)
                    .WithMany(p => p.ProductVariations)
                    .HasForeignKey(d => d.SizeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductVa__sizeI__3F466844");

                entity.HasOne(pv => pv.Product)
                    .WithMany(p => p.ProductVariations)
                    .HasForeignKey(pv => pv.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Size>(entity =>
            {
                entity.ToTable("Size");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Value)
                    .HasMaxLength(50)
                    .HasColumnName("value");
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.ToTable("Photo");

                entity.Property(e => e.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd(); // Auto-increment for SQL Server

                entity.Property(e => e.Path)
                .IsRequired() // Enforces non-null constraint
                .HasMaxLength(500)
                .HasColumnName("path");

                entity.Property(e => e.ProductVariationId)
                .HasColumnName("productVariationId");

                // Configure the relationship with ProductVariation
                entity.HasOne(d => d.ProductVariation)
                .WithMany(p => p.Photos)
                .HasForeignKey(d => d.ProductVariationId)
                .OnDelete(DeleteBehavior.Cascade) // Optional: configure delete behavior
                .HasConstraintName("FK_Photo_ProductVariation");
            });


            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
