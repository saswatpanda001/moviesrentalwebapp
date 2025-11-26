using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace MoviesRental.Models;

public partial class MovieRentalContext : DbContext
{
    public MovieRentalContext()
    {
    }

    public MovieRentalContext(DbContextOptions<MovieRentalContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Rental> Rentals { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }
    public virtual DbSet<Cart> Carts { get; set; }
    public virtual DbSet<CartItem> CartItems { get; set; }



    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            optionsBuilder.UseSqlServer("DefaultConnection");
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.MovieId).HasName("PK__Movies__4BD2941A6368F0F4");

            entity.Property(e => e.Genre).HasMaxLength(50);
            entity.Property(e => e.RentalPrice).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.Stock).HasDefaultValue(1);
            entity.Property(e => e.Title).HasMaxLength(200);

            entity.HasCheckConstraint("CK_Movies_Title_MinLength", "LEN([Title]) >= 4");
            entity.HasCheckConstraint("CK_Movies_Genre_MinLength", "LEN([Genre]) >= 4");
            entity.HasCheckConstraint("CK_Movies_Desc_MinLength", "LEN([Description]) >= 6");

            entity.HasCheckConstraint("CK_Movies_Stock_NonNegative", "[Stock] >= 0");

            entity.HasCheckConstraint(
                "CK_Movies_ReleaseYear_Range",
                "[ReleaseYear] >= 1801 AND [ReleaseYear] <= 2025"
            );

            entity.HasCheckConstraint(
                "CK_Movies_RentalPrice_Positive",
                "[RentalPrice] > 0"
            );

        });

        modelBuilder.Entity<Rental>(entity =>
        {
            entity.HasKey(e => e.RentalId).HasName("PK__Rentals__970059430E888F41");

            entity.Property(e => e.DueDate).HasColumnType("datetime");
            entity.Property(e => e.Price).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.RentedOn)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime");
            entity.Property(e => e.ReturnedOn).HasColumnType("datetime");

            entity.HasOne(d => d.Movie).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.MovieId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rentals__MovieId__403A8C7D");

            entity.HasOne(d => d.User).WithMany(p => p.Rentals)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Rentals__UserId__3F466844");

            // Add relationship to Payment
            entity.HasOne(d => d.Payment)
                .WithOne(p => p.Rental)
                .HasForeignKey<Payment>(p => p.RentalId) // Payment has RentalId as FK
                .OnDelete(DeleteBehavior.Cascade);


            entity.HasCheckConstraint(
                    "CK_Rentals_Price_Positive",
                    "[Price] > 0"
                );

            // DueDate must be >= RentedOn
            entity.HasCheckConstraint(
                "CK_Rentals_DueDate_After_RentedOn",
                "[DueDate] > [RentedOn]"
            );

            // ReturnedOn must be >= RentedOn (only if not null)
            entity.HasCheckConstraint(
                "CK_Rentals_ReturnedOn_After_RentedOn",
                "[ReturnedOn] IS NULL OR [ReturnedOn] >= [RentedOn]"
            );

            modelBuilder.Entity<Payment>(entity =>
            {
                entity.HasKey(e => e.PaymentId).HasName("PK__Payments__PaymentId");

                entity.Property(e => e.Amount).HasColumnType("decimal(10, 2)");
                entity.Property(e => e.PaymentMethod).HasMaxLength(50);
                entity.Property(e => e.Status).HasMaxLength(20).HasDefaultValue("Completed");
                entity.Property(e => e.PaidOn)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Rental)
                  .WithOne(r => r.Payment)
                  .HasForeignKey<Payment>(p => p.RentalId)
                  .OnDelete(DeleteBehavior.Cascade);

                // 🔥 DB-Level CHECK constraints
                entity.HasCheckConstraint(
                    "CK_Payments_Amount_Positive",
                    "[Amount] > 0"
                );

                entity.HasCheckConstraint(
                    "CK_Payments_PaymentMethod_MinLength",
                    "LEN([PaymentMethod]) >= 3"
                );

                entity.HasCheckConstraint(
                    "CK_Payments_Status_MinLength",
                    "LEN([Status]) >= 3"
                );


                // Remove the Rental relationship since we now have collection
            });




            modelBuilder.Entity<Review>(entity =>
            {
                entity.HasKey(e => e.ReviewId).HasName("PK__Reviews__74BC79CEB4AE0A62");

                entity.Property(e => e.CreatedAt)
                    .HasDefaultValueSql("(getdate())")
                    .HasColumnType("datetime");

                entity.HasOne(d => d.Movie).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Reviews__MovieId__45F365D3");

                entity.HasOne(d => d.User).WithMany(p => p.Reviews)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Reviews__UserId__44FF419A");

                entity.HasCheckConstraint(
                    "CK_Reviews_Rating_Range",
                    "[Rating] >= 1 AND [Rating] <= 5"
                );

                entity.HasCheckConstraint(
                    "CK_Reviews_Comment_MinLength",
                    "LEN([Comment]) >= 5 OR [Comment] IS NULL"
                );



            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.UserId)
                      .HasName("PK__Users__1788CC4C0C6F1E16");

                // Unique email
                entity.HasIndex(e => e.Email)
                      .IsUnique()
                      .HasDatabaseName("UQ_Users_Email");

                // Unique phone (if required)
                entity.HasIndex(e => e.Phone)
                      .IsUnique()
                      .HasDatabaseName("UQ_Users_Phone");


                entity.HasIndex(e => e.Name)
                      .IsUnique()
                      .HasDatabaseName("UQ_Users_Name");

                entity.Property(e => e.CreatedAt)
                      .HasDefaultValueSql("(getdate())")
                      .HasColumnType("datetime");

                entity.Property(e => e.Email)
                      .HasMaxLength(100);

                entity.Property(e => e.Name)
                      .HasMaxLength(100);

                entity.Property(e => e.Phone)
                      .HasMaxLength(10);
                entity.HasCheckConstraint("CK_Users_Name_MinLength", "LEN([Name]) >= 4");
                entity.HasCheckConstraint("CK_Users_Email_MinLength", "LEN([Email]) >= 4");
                entity.HasCheckConstraint("CK_Users_Phone_10Digits", "LEN([Phone]) = 10 AND [Phone] NOT LIKE '%[^0-9]%'");
                entity.HasCheckConstraint("CK_Users_Role_Enum", "[Role] IN ('User','Admin')");

                entity.HasCheckConstraint(
                    "CK_Users_Email_Valid",
                    "[Email] LIKE '%_@__%.__%'");

            });







            modelBuilder.Entity<Cart>(entity =>
            {
                entity.HasKey(e => e.CartId);
                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.HasKey(e => e.CartItemId);
                entity.HasOne(d => d.Cart)
                    .WithMany(p => p.CartItems)
                    .HasForeignKey(d => d.CartId)
                    .OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(d => d.Movie)
                    .WithMany()
                    .HasForeignKey(d => d.MovieId)
                    .OnDelete(DeleteBehavior.Cascade);

                entity.HasCheckConstraint(
                    "CK_CartItem_Quantity_Range",
                    "[Quantity] >= 1 AND [Quantity] <= 10"
                );

                entity.HasCheckConstraint(
                    "CK_CartItem_RentalDays_Range",
                    "[RentalDays] >= 1 AND [RentalDays] <= 30"
                );
            });






            OnModelCreatingPartial(modelBuilder);
        });
    }


    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
