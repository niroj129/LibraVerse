using LibraVerse.Enums;
using LibraVerse.Helper;
using LibraVerse.Models;
using LibraVerse.Models.Base;
using LibraVerse.Services.Interface;
using Microsoft.EntityFrameworkCore;

namespace LibraVerse.Persistence;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IUserService userService) : DbContext(options)
{
    public DbSet<Announcement> Announcements => Set<Announcement>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();
    public DbSet<BookAuthors> BookAuthors => Set<BookAuthors>();
    public DbSet<Cart> Carts => Set<Cart>();
    public DbSet<Discount> Discounts => Set<Discount>();
    public DbSet<Format> Formats => Set<Format>();
    public DbSet<Order> Orders => Set<Order>();
    public DbSet<OrderDetails> OrderDetails => Set<OrderDetails>();
    public DbSet<Publication> Publications => Set<Publication>();
    public DbSet<Review> Reviews => Set<Review>();
    public DbSet<User> Users => Set<User>();
    public DbSet<Wishlist> Wishlists => Set<Wishlist>();

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }

    public override int SaveChanges()
    {
        var dateTime = DateTime.Now;
        var userId = userService.UserId ?? Guid.Empty;

        foreach (var entry in ChangeTracker.Entries<BaseEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedBy = userId;
                    entry.Entity.CreatedDate = dateTime;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedBy = userId;
                    entry.Entity.UpdatedDate = dateTime;
                    break;
                case EntityState.Deleted:
                case EntityState.Detached:
                case EntityState.Unchanged:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        return base.SaveChanges();
    }
    
    public void SeedAdmin()
    {
        if (!Users.Any(u => u.Email == "admin@libraverse.com"))
        {
            const string password = "@ff!N1ty";
            
            var admin = new User
            {
                Id = Guid.NewGuid(),
                Name = "Admin",
                Email = "admin@libraverse.com",
                Password = password.Hash(),
                IsActive = true,
                Address = "Stateless",
                City = "Heaventh",
                PhoneNumber = "+977 9800000000",
                Role = Role.Admin,
                State = "Somewhere in Kathmandu"
            };

            Users.Add(admin);
            
            SaveChanges();
        }
    }
}