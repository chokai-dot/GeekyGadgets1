
using GeekyGadgets.Domain.Entity;
using GeekyGadgets.Domain.Enum;
using GeekyGadgets.Domain.Helpers;
using Microsoft.EntityFrameworkCore;

namespace Automarket.DAL
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }

       
        public DbSet<Smartphone> Smartphones { get; set; }

        public DbSet<User> Users { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(builder =>
            {
                builder.ToTable("Users").HasKey(x => x.Id);

                builder.HasData(new User
                {
                    Id = 1,
                    Name = "Raufat",
                    Password = HashPasswordHelper.HashPassowrd("123456"),
                    Email = "raufatnurarov@gmail.com",
                    Role = Role.Admin
                });

                builder.Property(x => x.Id).ValueGeneratedOnAdd();
                builder.Property(x => x.Email).IsRequired();
                builder.Property(x => x.Password).IsRequired();
                builder.Property(x => x.Name).HasMaxLength(100).IsRequired();

            });
        }
    }
}
