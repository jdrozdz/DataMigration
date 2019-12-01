using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataMigration.Models
{
    public partial class HostDbContext : DbContext
    {
        public static string connectionString;

        public HostDbContext() { }
        public HostDbContext(string connStr)
        {
            connectionString = connStr;
        }

        public HostDbContext(DbContextOptions<HostDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Account> Account { get; set; }
        public virtual DbSet<User> User { get; set; }
        public virtual DbSet<UserAssociation> UserAssociation { get; set; }
        public virtual DbSet<Units> Units { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Account>(entity =>
            {
                entity.ToTable("account");

                entity.HasIndex(e => e.Hid)
                    .HasName("account_hid_key")
                    .IsUnique();

                entity.HasIndex(e => e.UserIdId)
                    .HasName("uniq_7d3656a49d86650f")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

                entity.Property(e => e.Hid)
                    .HasColumnName("hid")
                    .HasMaxLength(255);

                entity.Property(e => e.Settings)
                    .IsRequired()
                    .HasColumnName("settings")
                    .HasColumnType("json")
                    .HasDefaultValueSql(@"'{
  ""vibrationEnabled"": true,
  ""soundsEnabled"": true,
  ""notificationsEnabled"": true,
  ""callTime"": 3,
  ""stayLogin"": true,
  ""status"": ""AVAILABLE"",
  ""displayName"": ""Strażak""
}'::json");

                entity.Property(e => e.UserIdId).HasColumnName("user_id_id");

                entity.HasOne(d => d.UserId)
                    .WithOne(p => p.Account)
                    .HasForeignKey<Account>(d => d.UserIdId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("fk_7d3656a49d86650f");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.ToTable("user");

                entity.HasIndex(e => e.Phone)
                    .HasName("uniq_8d93d649444f97dd")
                    .IsUnique();

                entity.Property(e => e.Id)
                    .HasColumnName("id")
                    .ValueGeneratedOnAdd();

                entity.Property(e => e.Displayname)
                    .HasColumnName("displayname")
                    .HasMaxLength(30);

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(255);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasMaxLength(180);

                entity.Property(e => e.Roles)
                    .IsRequired()
                    .HasColumnName("roles")
                    .HasColumnType("json")
                    .HasDefaultValueSql("'[]'::json");

                entity.Property(e => e.Skill)
                    .IsRequired()
                    .HasColumnName("skill")
                    .HasMaxLength(30)
                    .HasDefaultValueSql("'brak wyszkolenia'::character varying");
            });

            modelBuilder.Entity<UserAssociation>(entity =>
            {
                entity.ToTable("user_association");

                entity.HasIndex(e => new { e.UnitId, e.UserId })
                    .HasName("user_association_uk")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id").ValueGeneratedOnAdd();

                entity.Property(e => e.IdUser).HasColumnName("id_user");

                entity.Property(e => e.UnitId).HasColumnName("unit_id");

                entity.Property(e => e.UserId).HasColumnName("user_id");

                entity.HasOne(d => d.IdUserNavigation)
                    .WithMany(p => p.UserAssociation)
                    .HasForeignKey(d => d.IdUser)
                    .OnDelete(DeleteBehavior.SetNull)
                    .HasConstraintName("user_fk");
            });

            modelBuilder.Entity<Units>(entity =>
            {
                entity.ToTable("units");

                entity.HasIndex(e => e.UnitId)
                    .HasName("units_unit_id_key")
                    .IsUnique();

                entity.HasIndex(e => new { e.UnitId, e.Phone })
                    .HasName("uniq_e9b07449f8bd700d")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .IsRequired()
                    .HasColumnName("description")
                    .HasMaxLength(255);

                entity.Property(e => e.Phone)
                    .IsRequired()
                    .HasColumnName("phone")
                    .HasMaxLength(255);

                entity.Property(e => e.UnitId)
                    .IsRequired()
                    .HasColumnName("unit_id")
                    .HasMaxLength(255);
            });

            modelBuilder.HasSequence("account_id_seq");

            modelBuilder.HasSequence("confirmed_id_seq").StartsAt(100);

            modelBuilder.HasSequence("incidents_id_seq");

            modelBuilder.HasSequence("log_id_seq");

            modelBuilder.HasSequence("units_id_seq").HasMax(2147483647);

            modelBuilder.HasSequence("user_association_id_seq");

            modelBuilder.UseIdentityColumns();

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
