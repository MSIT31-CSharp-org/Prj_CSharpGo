using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class WildnessCampingContext : DbContext
    {
        public WildnessCampingContext()
        {
        }

        public WildnessCampingContext(DbContextOptions<WildnessCampingContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Recipe> Recipes { get; set; }

//        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
//        {
//            if (!optionsBuilder.IsConfigured)
//            {
//#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
//                optionsBuilder.UseSqlServer("Server=218.161.74.208;Database=WildnessCamping;uid=pudb_edit;pwd=Opland0819");
//            }
//        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipe");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.CookingTime)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Img)
                    .HasMaxLength(200)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedTime).HasColumnType("datetime");

                entity.Property(e => e.Preparation).HasMaxLength(200);

                entity.Property(e => e.PreparationTime)
                    .HasMaxLength(10)
                    .IsFixedLength(true);

                entity.Property(e => e.PublishTime).HasColumnType("datetime");

                entity.Property(e => e.RecipeName).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.Step).HasMaxLength(500);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Yield)
                    .HasMaxLength(10)
                    .IsFixedLength(true);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
