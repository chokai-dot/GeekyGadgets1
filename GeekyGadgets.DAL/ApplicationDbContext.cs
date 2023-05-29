using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Domain.Enum;
using GeekyGadgets.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace GeekyGadgets.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

        public DbSet<Smartphone> Smartphones { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Profile> Profiles { get; set; } 
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Order> Orders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.ToTable("Users").HasKey(x => x.Id);

                builder.Property(x => x.Id).ValueGeneratedOnAdd();
                builder.Property(x => x.Email).IsRequired();
                builder.Property(x => x.Password).IsRequired().HasMaxLength(500);
                builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

                builder.HasOne(x => x.Profile)
                    .WithOne(x => x.User)
                    .HasForeignKey<User>(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);

                builder.HasOne(x => x.Basket)
                    .WithOne(x => x.User)
                    .HasPrincipalKey<User>(x => x.Id)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<Profile>(builder =>
            {
                builder.ToTable("Profiles").HasKey(x => x.Id);

                builder.Property(x => x.Id).ValueGeneratedOnAdd();
                builder.Property(x => x.Age);
                builder.Property(x => x.Address).HasMaxLength(250);
                builder.Property(x => x.UserId).IsRequired();

            });
            modelBuilder.Entity<Basket>(builder =>
            {
                builder.ToTable("Baskets").HasKey(x => x.Id);
            });

            modelBuilder.Entity<Order>(builder =>
            {
                builder.ToTable("Orders").HasKey(x => x.Id);

                builder.HasOne(r => r.Basket).WithMany(t => t.Orders)
                    .HasForeignKey(r => r.BasketId);
            });

            SeedData(modelBuilder);
 
        }

        private void SeedData(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 1,
                Name = "Raufat",
                Password = BCrypt.Net.BCrypt.HashPassword("123456Raufat"),
                Email = "raufatnurarov@gmail.com",
                Role = Role.Admin
            });

            modelBuilder.Entity<User>().HasData(new User
            {
                Id = 2,
                Name = "Diyaz",
                Password = BCrypt.Net.BCrypt.HashPassword("12345Diyaz"),
                Email = "diyaz@mail.com",
                Role = Role.User
            });

            modelBuilder.Entity<Profile>().HasData(new Profile
            {
                Id = 1,
                Age = 30,
                Address = "123 Main St",
                UserId = 1
            });

            modelBuilder.Entity<Profile>().HasData(new Profile
            {
                Id = 2,
                Age = 30,
                Address = "128 Main St",
                UserId = 2
            });

            modelBuilder.Entity<Basket>().HasData(new Basket
            {
                Id = 1,
                UserId = 1
            });

        }
    }
}
