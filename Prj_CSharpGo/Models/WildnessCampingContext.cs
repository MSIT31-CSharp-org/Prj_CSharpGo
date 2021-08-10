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

        public virtual DbSet<Approval> Approvals { get; set; }
        public virtual DbSet<Association> Associations { get; set; }
        public virtual DbSet<Camp> Camps { get; set; }
        public virtual DbSet<CampImg> CampImgs { get; set; }
        public virtual DbSet<CampOrder> CampOrders { get; set; }
        public virtual DbSet<CategoriesTypeI> CategoriesTypeIs { get; set; }
        public virtual DbSet<CategoriesTypeIi> CategoriesTypeIis { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Demo> Demos { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderDetail> OrderDetails { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<ProductImg> ProductImgs { get; set; }
        public virtual DbSet<Recipe> Recipes { get; set; }
        public virtual DbSet<RecipeImg> RecipeImgs { get; set; }
        public virtual DbSet<ShoppingCart> ShoppingCarts { get; set; }
        public virtual DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Chinese_Taiwan_Stroke_CI_AS");

            modelBuilder.Entity<Approval>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Approval");

                entity.Property(e => e.ApprovalAfter).HasMaxLength(50);

                entity.Property(e => e.ApprovalColumn).HasMaxLength(50);

                entity.Property(e => e.ApprovalDate).HasColumnType("datetime");

                entity.Property(e => e.ApprovalPk)
                    .HasMaxLength(50)
                    .HasColumnName("ApprovalPK");

                entity.Property(e => e.ApprovalSheet).HasMaxLength(50);

                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.HasOne(d => d.Employee)
                    .WithMany()
                    .HasForeignKey(d => d.EmployeeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Approval__Employ__160F4887");
            });

            modelBuilder.Entity<Association>(entity =>
            {
                entity.HasKey(e => e.Sequence);

                entity.ToTable("Association");

                entity.HasIndex(e => e.RecipeId, "IX_Association");

                entity.HasIndex(e => e.ProductId, "IX_Association_2");

                entity.Property(e => e.Description).HasMaxLength(200);

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("ProductID");

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.Associations)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Associati__Produ__48CFD27E");

                entity.HasOne(d => d.Recipe)
                    .WithMany(p => p.Associations)
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Associati__Recip__47DBAE45");
            });

            modelBuilder.Entity<Camp>(entity =>
            {
                entity.ToTable("Camp");

                entity.Property(e => e.CampId)
                    .ValueGeneratedNever()
                    .HasColumnName("CampID");

                entity.Property(e => e.Ahref)
                    .HasMaxLength(20)
                    .HasColumnName("ahref");

                entity.Property(e => e.Approval)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.CampName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.CampSize).HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(100);

                entity.Property(e => e.Img).HasMaxLength(10);
            });

            modelBuilder.Entity<CampImg>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CampImg");

                entity.Property(e => e.CampId).HasColumnName("CampID");

                entity.Property(e => e.Img).HasMaxLength(200);

                entity.HasOne(d => d.Camp)
                    .WithMany()
                    .HasForeignKey(d => d.CampId)
                    .HasConstraintName("FK__CampImg__CampID__0A338187");
            });

            modelBuilder.Entity<CampOrder>(entity =>
            {
                entity.ToTable("CampOrder");

                entity.Property(e => e.CampOrderId).HasColumnName("CampOrderID");

                entity.Property(e => e.Approval).HasMaxLength(2);

                entity.Property(e => e.CampId).HasColumnName("CampID");

                entity.Property(e => e.CampStatus)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.EndDay).HasColumnType("datetime");

                entity.Property(e => e.OrderDay).HasColumnType("datetime");

                entity.Property(e => e.OrderStatus)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.PayMethod)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.StartDay).HasColumnType("datetime");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Camp)
                    .WithMany(p => p.CampOrders)
                    .HasForeignKey(d => d.CampId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CampOrder__CampI__0B27A5C0");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.CampOrders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__CampOrder__UserI__0C1BC9F9");
            });

            modelBuilder.Entity<CategoriesTypeI>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CategoriesTypeI");

                entity.Property(e => e.CategoryId)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("CategoryID")
                    .IsFixedLength(true);

                entity.Property(e => e.CategoryTypeI)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.TypeIname)
                    .HasMaxLength(20)
                    .HasColumnName("TypeIName");
            });

            modelBuilder.Entity<CategoriesTypeIi>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("CategoriesTypeII");

                entity.Property(e => e.CategoryTypeI)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.CategoryTypeIi).HasColumnName("CategoryTypeII");
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.CategoryId)
                    .HasMaxLength(2)
                    .HasColumnName("CategoryID")
                    .IsFixedLength(true);

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(20);

                entity.Property(e => e.Description).HasMaxLength(100);
            });

            modelBuilder.Entity<Demo>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("Demo");

                entity.Property(e => e.C)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("c")
                    .IsFixedLength(true);

                entity.Property(e => e.Id)
                    .ValueGeneratedOnAdd()
                    .HasColumnName("id");

                entity.Property(e => e.Nc)
                    .HasMaxLength(10)
                    .HasColumnName("nc")
                    .IsFixedLength(true);

                entity.Property(e => e.Nv)
                    .HasMaxLength(10)
                    .HasColumnName("nv");

                entity.Property(e => e.V)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .HasColumnName("v");
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.EmployeeId).HasColumnName("EmployeeID");

                entity.Property(e => e.EmployeeEmail).HasMaxLength(50);

                entity.Property(e => e.EmployeeName)
                    .HasMaxLength(20)
                    .HasColumnName("employeeName");

                entity.Property(e => e.EmployeePassword)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Approval)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.OrderDate).HasColumnType("datetime");

                entity.Property(e => e.PayMethod)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.UserName).HasMaxLength(20);

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Orders)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Orders__UserID__35BCFE0A");
            });

            modelBuilder.Entity<OrderDetail>(entity =>
            {
                entity.HasKey(e => e.Odpk);

                entity.ToTable("OrderDetail");

                entity.Property(e => e.Odpk).HasColumnName("ODPK");

                entity.Property(e => e.Approval)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.Commets).HasMaxLength(200);

                entity.Property(e => e.OrderId).HasColumnName("OrderID");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("ProductID");

                entity.HasOne(d => d.Order)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.OrderId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Order__37A5467C");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.OrderDetails)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__OrderDeta__Produ__38996AB5");
            });

            modelBuilder.Entity<Product>(entity =>
            {
                entity.Property(e => e.ProductId)
                    .HasMaxLength(10)
                    .HasColumnName("ProductID");

                entity.Property(e => e.Approval)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.CategoryId)
                    .IsRequired()
                    .HasMaxLength(2)
                    .HasColumnName("CategoryID")
                    .IsFixedLength(true);

                entity.Property(e => e.CategoryType)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.ProductDescription).HasMaxLength(200);

                entity.Property(e => e.ProductName).HasMaxLength(50);

                entity.Property(e => e.Specification).HasMaxLength(20);

                entity.Property(e => e.Status)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.Products)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Products__Catego__30C33EC3");
            });

            modelBuilder.Entity<ProductImg>(entity =>
            {
                entity.HasKey(e => e.Img);

                entity.ToTable("ProductImg");

                entity.Property(e => e.Img).HasMaxLength(200);

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("ProductID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ProductImgs)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ProductIm__Produ__19DFD96B");
            });

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

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Recipes)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__Recipe__UserID__4316F928");
            });

            modelBuilder.Entity<RecipeImg>(entity =>
            {
                entity.HasNoKey();

                entity.ToTable("RecipeImg");

                entity.Property(e => e.Img).HasMaxLength(200);

                entity.Property(e => e.Message).HasMaxLength(200);

                entity.Property(e => e.RecipeId).HasColumnName("RecipeID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Recipe)
                    .WithMany()
                    .HasForeignKey(d => d.RecipeId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RecipeImg__Recip__44FF419A");

                entity.HasOne(d => d.User)
                    .WithMany()
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__RecipeImg__UserI__45F365D3");
            });

            modelBuilder.Entity<ShoppingCart>(entity =>
            {
                entity.HasKey(e => e.ShCartId);

                entity.ToTable("ShoppingCart");

                entity.Property(e => e.ShCartId).HasColumnName("ShCartID");

                entity.Property(e => e.ProductId)
                    .IsRequired()
                    .HasMaxLength(10)
                    .HasColumnName("ProductID");

                entity.Property(e => e.ProductName).HasMaxLength(50);

                entity.Property(e => e.Status)
                    .HasMaxLength(2)
                    .IsFixedLength(true);

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Product)
                    .WithMany(p => p.ShoppingCarts)
                    .HasForeignKey(d => d.ProductId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShoppingC__Produ__32E0915F");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ShoppingCarts)
                    .HasForeignKey(d => d.UserId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK__ShoppingC__UserI__31EC6D26");
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "Email");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.Property(e => e.Address).HasMaxLength(50);

                entity.Property(e => e.Birthday).HasColumnType("datetime");

                entity.Property(e => e.ConfirmPassword).HasMaxLength(50);

                entity.Property(e => e.DiscountCode).HasMaxLength(8);

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Img).HasMaxLength(50);

                entity.Property(e => e.Phone).HasMaxLength(10);

                entity.Property(e => e.Region).HasMaxLength(10);

                entity.Property(e => e.UpdateDate).HasColumnType("datetime");

                entity.Property(e => e.UserAccount)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.UserName).HasMaxLength(20);

                entity.Property(e => e.UserPassword)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.UserStatus)
                    .HasMaxLength(2)
                    .IsFixedLength(true);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
