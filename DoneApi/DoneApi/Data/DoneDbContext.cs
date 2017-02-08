using Microsoft.EntityFrameworkCore;
using DoneApi.Models;

namespace DoneApi.Data
{
    public class DoneDbContext : DbContext
    {
        public DoneDbContext(DbContextOptions<DoneDbContext> options) : base(options)
        {
        }

        public DbSet<Item> Items { get; set; }
    }
}
