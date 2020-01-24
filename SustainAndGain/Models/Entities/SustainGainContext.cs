using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace SustainAndGain.Models.Entities
{
    public partial class SustainGainContext : DbContext
    {
        public SustainGainContext()
        {
        }

        public SustainGainContext(DbContextOptions<SustainGainContext> options)
            : base(options)
        {
        }

        public virtual DbSet<AspNetRoleClaims> AspNetRoleClaims { get; set; }
        public virtual DbSet<AspNetRoles> AspNetRoles { get; set; }
        public virtual DbSet<AspNetUserClaims> AspNetUserClaims { get; set; }
        public virtual DbSet<AspNetUserLogins> AspNetUserLogins { get; set; }
        public virtual DbSet<AspNetUserRoles> AspNetUserRoles { get; set; }
        public virtual DbSet<AspNetUserTokens> AspNetUserTokens { get; set; }
        public virtual DbSet<AspNetUsers> AspNetUsers { get; set; }
        public virtual DbSet<Competition> Competition { get; set; }
        public virtual DbSet<HistDataStocks> HistDataStocks { get; set; }
        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<StaticStockData> StaticStockData { get; set; }
        public virtual DbSet<UsersHistoricalTransactions> UsersHistoricalTransactions { get; set; }
        public virtual DbSet<UsersInCompetition> UsersInCompetition { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<AspNetRoleClaims>(entity =>
            {
                entity.HasIndex(e => e.RoleId);

                entity.Property(e => e.RoleId).IsRequired();

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetRoleClaims)
                    .HasForeignKey(d => d.RoleId);
            });

            modelBuilder.Entity<AspNetRoles>(entity =>
            {
                entity.HasIndex(e => e.NormalizedName)
                    .HasName("RoleNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedName] IS NOT NULL)");

                entity.Property(e => e.Name).HasMaxLength(256);

                entity.Property(e => e.NormalizedName).HasMaxLength(256);
            });

            modelBuilder.Entity<AspNetUserClaims>(entity =>
            {
                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserClaims)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserLogins>(entity =>
            {
                entity.HasKey(e => new { e.LoginProvider, e.ProviderKey });

                entity.HasIndex(e => e.UserId);

                entity.Property(e => e.UserId).IsRequired();

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserLogins)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserRoles>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });

                entity.HasIndex(e => e.RoleId);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.RoleId);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserRoles)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUserTokens>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name });

                entity.HasOne(d => d.User)
                    .WithMany(p => p.AspNetUserTokens)
                    .HasForeignKey(d => d.UserId);
            });

            modelBuilder.Entity<AspNetUsers>(entity =>
            {
                entity.HasIndex(e => e.NormalizedEmail)
                    .HasName("EmailIndex");

                entity.HasIndex(e => e.NormalizedUserName)
                    .HasName("UserNameIndex")
                    .IsUnique()
                    .HasFilter("([NormalizedUserName] IS NOT NULL)");

                entity.Property(e => e.Email).HasMaxLength(256);

                entity.Property(e => e.NormalizedEmail).HasMaxLength(256);

                entity.Property(e => e.NormalizedUserName).HasMaxLength(256);

                entity.Property(e => e.UserName).HasMaxLength(256);
            });

            modelBuilder.Entity<Competition>(entity =>
            {
                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(128);

                entity.Property(e => e.StartTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<HistDataStocks>(entity =>
            {
                entity.Property(e => e.CurrentPrice).HasColumnType("money");

                entity.Property(e => e.DateTime).HasColumnType("datetime");

                entity.Property(e => e.StockId).HasColumnName("StockID");

                entity.Property(e => e.Symbol).HasMaxLength(64);

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.HistDataStocks)
                    .HasForeignKey(d => d.StockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__HistDataS__Stock__59FA5E80");
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderValue).HasColumnType("money");

                entity.Property(e => e.TimeOfInsertion).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Comp)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.CompId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__CompId__01142BA1");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.StockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__StockId__7F2BE32F");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Order)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Order__UserId__00200768");
            });

            modelBuilder.Entity<StaticStockData>(entity =>
            {
                entity.Property(e => e.CompanyName)
                    .HasMaxLength(128)
                    .IsUnicode(false);

                entity.Property(e => e.Description)
                    .HasMaxLength(1500)
                    .IsUnicode(false);

                entity.Property(e => e.Sector)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Symbol)
                    .IsRequired()
                    .HasMaxLength(64)
                    .IsUnicode(false);
            });

            modelBuilder.Entity<UsersHistoricalTransactions>(entity =>
            {
                entity.Property(e => e.DateTimeOfTransaction).HasColumnType("datetime");

                entity.Property(e => e.TransactionPrice).HasColumnType("money");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Competition)
                    .WithMany(p => p.UsersHistoricalTransactions)
                    .HasForeignKey(d => d.CompetitionId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UsersHist__Compe__7B5B524B");

                entity.HasOne(d => d.Stock)
                    .WithMany(p => p.UsersHistoricalTransactions)
                    .HasForeignKey(d => d.StockId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UsersHist__Stock__6EF57B66");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersHistoricalTransactions)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UsersHist__UserI__6FE99F9F");
            });

            modelBuilder.Entity<UsersInCompetition>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.AvailableForInvestment).HasColumnType("money");

                entity.Property(e => e.CurrentValue).HasColumnType("money");

                entity.Property(e => e.LastUpdatedAvailableForInvestment).HasColumnType("datetime");

                entity.Property(e => e.LastUpdatedCurrentValue).HasColumnType("datetime");

                entity.Property(e => e.UserId)
                    .IsRequired()
                    .HasMaxLength(450);

                entity.HasOne(d => d.Comp)
                    .WithMany(p => p.UsersInCompetition)
                    .HasForeignKey(d => d.CompId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UsersInCo__CompI__7C4F7684");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.UsersInCompetition)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__UsersInCo__UserI__787EE5A0");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
