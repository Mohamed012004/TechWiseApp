using Microsoft.EntityFrameworkCore;
using TechWise.Domains.Entities.Store;
using TechWise.Domains.Entities.Support;

namespace TechWise.Persistence.Store
{
    public class StoreDbContext(DbContextOptions<StoreDbContext> options)
        : DbContext(options)
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductSpec> ProductSpecs { get; set; }
        public DbSet<RecommendedProduct> RecommendedProducts { get; set; }
        public DbSet<SavedItem> SavedItems { get; set; }


        public DbSet<Cart> Carts { get; set; }
        public DbSet<CartItem> CartItems { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }

        public DbSet<Review> Reviews { get; set; }


        public DbSet<ContactMessage> ContactMessages { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }


        public DbSet<Notification> Notifications { get; set; }
        public DbSet<NotificationSettings> NotificationSettings { get; set; }
        public DbSet<MarketAlert> MarketAlerts { get; set; }
        public DbSet<MarketAlertProduct> MarketAlertProducts { get; set; }



        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Product>(entity =>
            {
                entity.ToTable("products", t => t.ExcludeFromMigrations());

                entity.Property(p => p.Id).HasColumnName("id");
                entity.Property(p => p.Title).HasColumnName("title");
                entity.Property(p => p.Brand).HasColumnName("brand");
                entity.Property(p => p.Category).HasColumnName("category");
                entity.Property(p => p.Price).HasColumnName("price").HasColumnType("numeric(10,2)");
                entity.Property(p => p.WasPrice).HasColumnName("was_price").HasColumnType("numeric(10,2)");
                entity.Property(p => p.InStock).HasColumnName("in_stock");
                entity.Property(p => p.RatingAvg).HasColumnName("rating_avg");
                entity.Property(p => p.RatingCount).HasColumnName("rating_count");
                entity.Property(p => p.ImageUrl).HasColumnName("image_url");
                entity.Property(p => p.ProductUrl).HasColumnName("url");
                entity.Property(p => p.CreatedAt).HasColumnName("created_at");

                entity.HasKey(p => p.Id);

            });



            modelBuilder.Entity<ProductSpec>(entity =>
            {
                entity.ToTable("product_specs", t => t.ExcludeFromMigrations());

                entity.Property(ps => ps.Id).HasColumnName("id");

                entity.Property(ps => ps.SpecKey).HasColumnName("spec_key");
                entity.Property(ps => ps.SpecValue).HasColumnName("spec_value");
                entity.Property(ps => ps.ProductId).HasColumnName("product_id");

                entity.HasOne(ps => ps.Product)
                      .WithMany(p => p.Specs)
                      .HasForeignKey(ps => ps.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<RecommendedProduct>(entity =>
            {
                entity.ToTable("recommended_products", t => t.ExcludeFromMigrations());

                entity.Property(r => r.Id).HasColumnName("id");

                entity.Property(r => r.UserId).HasColumnName("user_id");
                entity.Property(r => r.ProductId).HasColumnName("product_id");
                entity.Property(r => r.UserReqNumber).HasColumnName("user_req_number");
                entity.Property(r => r.OperationType).HasColumnName("operation_type");
                entity.Property(r => r.ProductName).HasColumnName("product_name");

                entity.Property(r => r.Brand).HasColumnName("brand");
                entity.Property(r => r.Category).HasColumnName("category");
                entity.Property(r => r.Purpose).HasColumnName("purpose");
                entity.Property(r => r.Budget).HasColumnName("budget");
                entity.Property(r => r.Reason).HasColumnName("reason");

                entity.HasOne(r => r.Product)
                      .WithMany()
                      .HasForeignKey(r => r.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


            modelBuilder.Entity<SavedItem>(entity =>
            {
                entity.ToTable("SavedItems");
                entity.HasOne(s => s.Product)
                      .WithMany()
                      .HasForeignKey(s => s.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(s => new { s.UserId, s.ProductId }).IsUnique();
            });




            modelBuilder.Entity<Cart>(entity =>
            {
                entity.ToTable("Carts");
                entity.HasIndex(c => c.UserId).IsUnique();
            });

            modelBuilder.Entity<CartItem>(entity =>
            {
                entity.ToTable("CartItems");
                entity.HasOne(ci => ci.Cart)
                      .WithMany(c => c.Items)
                      .HasForeignKey(ci => ci.CartId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(ci => ci.Product)
                      .WithMany()
                      .HasForeignKey(ci => ci.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(ci => new { ci.CartId, ci.ProductId }).IsUnique();
            });

            modelBuilder.Entity<Order>(entity =>
            {
                entity.ToTable("Orders");
                entity.Property(o => o.Subtotal).HasColumnType("decimal(10,2)");
                entity.Property(o => o.ShippingCost).HasColumnType("decimal(10,2)");
                entity.Property(o => o.Tax).HasColumnType("decimal(10,2)");
                entity.Property(o => o.Total).HasColumnType("decimal(10,2)");
            });

            modelBuilder.Entity<OrderItem>(entity =>
            {
                entity.ToTable("OrderItems");
                entity.Property(oi => oi.Price).HasColumnType("decimal(10,2)");
                entity.HasOne(oi => oi.Order)
                      .WithMany(o => o.Items)
                      .HasForeignKey(oi => oi.OrderId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(oi => oi.Product)
                      .WithMany()
                      .HasForeignKey(oi => oi.ProductId)
                      .OnDelete(DeleteBehavior.NoAction);
            });


            modelBuilder.Entity<Review>(entity =>
            {
                entity.ToTable("Reviews");
                entity.HasOne(r => r.Product)
                      .WithMany()
                      .HasForeignKey(r => r.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasIndex(r => new { r.UserId, r.ProductId }).IsUnique();

                entity.Property(r => r.Rating)
                      .IsRequired();
            });

            modelBuilder.Entity<ContactMessage>(entity =>
            {
                entity.ToTable("ContactMessages");
            });

            modelBuilder.Entity<Feedback>(entity =>
            {
                entity.ToTable("Feedbacks");
            });




            modelBuilder.Entity<Notification>(entity =>
            {
                entity.ToTable("Notifications");

                entity.HasOne(n => n.Product)
                      .WithMany()
                      .HasForeignKey(n => n.ProductId)
                      .OnDelete(DeleteBehavior.SetNull);

                // only FK - without Navigation Property
                entity.Property(n => n.OrderId)
                      .IsRequired(false);
            });

            modelBuilder.Entity<NotificationSettings>(entity =>
            {
                entity.ToTable("NotificationSettings");
                entity.HasIndex(ns => ns.UserId).IsUnique();
            });

            modelBuilder.Entity<MarketAlert>(entity =>
            {
                entity.ToTable("MarketAlerts");
            });

            modelBuilder.Entity<MarketAlertProduct>(entity =>
            {
                entity.ToTable("MarketAlertProducts");
                entity.HasOne(mp => mp.MarketAlert)
                      .WithMany(ma => ma.Products)
                      .HasForeignKey(mp => mp.MarketAlertId)
                      .OnDelete(DeleteBehavior.Cascade);

                entity.HasOne(mp => mp.Product)
                      .WithMany()
                      .HasForeignKey(mp => mp.ProductId)
                      .OnDelete(DeleteBehavior.Cascade);
            });


        }
    }
}