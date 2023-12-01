using System.Reflection.Emit;
using asm.Areas.Identity.Data;
using asm.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace asm.Areas.Identity.Data;

public class asmIdentityDbContext : IdentityDbContext<User>
{
    public DbSet<asm.Models.Product> Products { get; set; }
    public DbSet<asm.Models.Category> Categories { get; set; }
    public DbSet<asm.Models.Size> Sizes { get; set; }
    public DbSet<asm.Models.ProductSize> ProductSizes { get; set; }
    public DbSet<asm.Models.Cart> Carts { get; set; }
    public DbSet<asm.Models.CartItem> CartItems { get; set; }
    public DbSet<asm.Models.Order> Orders {get; set;}
    public DbSet<asm.Models.OrderDetail> OrderDetails {get; set;}


    public asmIdentityDbContext(DbContextOptions<asmIdentityDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<Product>()
            .Property(p => p.Images)
            .HasConversion(
                v => string.Join(',', v),
                v => v.Split(',', StringSplitOptions.RemoveEmptyEntries).ToList()
            )
            .Metadata
            .SetValueComparer(new ValueComparer<List<string>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList()
            ));
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        // Xử lý quan hệ giữa Cart và CartItem
        builder.Entity<Cart>()
            .HasMany(c => c.CartItems)
            .WithOne()
            .HasForeignKey(ci => ci.CartId);

        builder.Entity<CartItem>()
            .HasOne(ci => ci.Cart)
            .WithMany(c => c.CartItems)
            .HasForeignKey(ci => ci.CartId)
            .IsRequired();
        //Order, Order details and User
        builder.Entity<Order>()
            .HasOne(o => o.User)
            .WithMany(u => u.Orders)
            .HasForeignKey(o => o.UserID);

        builder.Entity<OrderDetail>()
            .HasKey(od => new { od.OrderID, od.ProductID });

        builder.Entity<OrderDetail>()
            .HasOne(od => od.Order)
            .WithMany(o => o.OrderDetails)
            .HasForeignKey(od => od.OrderID);

        builder.Entity<OrderDetail>()
            .HasOne(od => od.Product)
            .WithMany(p => p.OrderDetails)
            .HasForeignKey(od => od.ProductID);

    }
}
