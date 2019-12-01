using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace DataMigration.Models
{
    public partial class SourceDbContext : DbContext
    {
        public static string connectionString;
        public SourceDbContext() { }

        public SourceDbContext(string connStr)
        {
            connectionString = connStr;
        }

        public SourceDbContext(DbContextOptions<SourceDbContext> options)
            : base(options)
        {
        }

        public virtual DbSet<MemberAssociation> MemberAssociation { get; set; }
        public virtual DbSet<Members> Members { get; set; }
        public virtual DbSet<Skills> Skills { get; set; }
        public virtual DbSet<OldUnits> Units { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MemberAssociation>(entity =>
            {
                entity.ToTable("member_association", "osp");

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.MemberId).HasColumnName("member_id");

                entity.Property(e => e.UnitId)
                    .IsRequired()
                    .HasColumnName("unit_id")
                    .HasMaxLength(10);

                entity.HasOne(d => d.Member)
                    .WithMany(p => p.MemberAssociation)
                    .HasForeignKey(d => d.MemberId)
                    .HasConstraintName("member_association_member_id_fkey");
            });

            modelBuilder.Entity<Members>(entity =>
            {
                entity.HasKey(e => e.MemberId)
                    .HasName("members_pkey");

                entity.ToTable("members", "osp");

                entity.HasIndex(e => e.MemberId)
                    .HasName("members_new_member_id_key")
                    .IsUnique();

                entity.HasIndex(e => e.Phone)
                    .HasName("members_new_phone_key")
                    .IsUnique();

                entity.HasIndex(e => e.Username)
                    .HasName("members_new_username_key")
                    .IsUnique();

                entity.Property(e => e.MemberId)
                    .HasColumnName("member_id")
                    .HasDefaultValueSql("nextval('osp.members_new_member_id_seq'::regclass)");

                entity.Property(e => e.Password)
                    .IsRequired()
                    .HasColumnName("password")
                    .HasMaxLength(100);

                entity.Property(e => e.Phone).HasColumnName("phone");

                entity.Property(e => e.Registered).HasColumnName("registered");

                entity.Property(e => e.SkillId).HasColumnName("skill_id");

                entity.Property(e => e.Token)
                    .IsRequired()
                    .HasColumnName("token")
                    .HasMaxLength(50)
                    .HasDefaultValueSql("psp.random_salt(50)");

                entity.Property(e => e.Username)
                    .IsRequired()
                    .HasColumnName("username")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Skill)
                    .WithMany(p => p.Members)
                    .HasForeignKey(d => d.SkillId)
                    .HasConstraintName("members_new_skill_id_fkey");
            });

            modelBuilder.Entity<Skills>(entity =>
            {
                entity.ToTable("skills", "osp");

                entity.HasIndex(e => e.Skill)
                    .HasName("skills_skill_key")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Skill)
                    .IsRequired()
                    .HasColumnName("skill")
                    .HasMaxLength(20);
            });

            modelBuilder.Entity<OldUnits>(entity =>
            {
                entity.HasKey(e => e.UnitId)
                    .HasName("units_pkey");

                entity.ToTable("units");

                entity.Property(e => e.UnitId)
                    .HasColumnName("unit_id")
                    .HasMaxLength(10);

                entity.Property(e => e.Description)
                    .HasColumnName("description")
                    .HasMaxLength(200);

                entity.Property(e => e.Phone)
                    .HasColumnName("phone")
                    .HasMaxLength(15);
            });

            modelBuilder.HasSequence("members_member_id_seq", "osp").StartsAt(40000);

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
