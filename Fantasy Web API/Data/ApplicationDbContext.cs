using Microsoft.EntityFrameworkCore;
using Fantasy_Web_API.Models;

namespace Fantasy_Web_API.Data
{
    public class ApplicationDbContext : DbContext
    {
        public DbSet<Item> Items { get; set; }
        public DbSet<UserModel> Users { get; set; }


        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }
    }
}