using Microsoft.EntityFrameworkCore;
using userprofile.entities.Models;
using userprofile.persistence.Configuration;

namespace userprofile.persistence
{
    public class UserProfileDbContext : DbContext
    {
        public UserProfileDbContext(DbContextOptions options)
            :base(options)
        { }

        public DbSet<AdminUserModel> AdminUserModel { get; set; }
        public DbSet<UserActivationModel> UserActivationModel { get; set; }
        public DbSet<UserModel> UserModel { get; set; }
      
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AdminUserModelConfiguration());
        }

    }
}
