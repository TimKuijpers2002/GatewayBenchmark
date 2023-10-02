using GatewayBenchmark.Models;
using Microsoft.EntityFrameworkCore;

namespace GatewayBenchmark.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            
        }

        public DbSet<ToDoItem> ToDoItems => Set<ToDoItem>();
    }
}
