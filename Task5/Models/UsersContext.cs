using System;
using Microsoft.EntityFrameworkCore;

namespace Task5.Models
{
    public class UsersContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public UsersContext(DbContextOptions<UsersContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            string adminRoleName = "admin";
            string userRoleName = "user";

            Role adminRole = new Role { Id = 1, Name = adminRoleName };
            Role userRole = new Role { Id = 2, Name = userRoleName };
            User adminUser = new User { Id = 1, Email = "admin@mail.ru", Password = "1234", RoleId = adminRole.Id, Status="Не в сети"};

            modelBuilder.Entity<Role>().HasData(new Role[] { adminRole, userRole });
            modelBuilder.Entity<User>().HasData(new User[] { adminUser });
            base.OnModelCreating(modelBuilder);
        }
    }
}
