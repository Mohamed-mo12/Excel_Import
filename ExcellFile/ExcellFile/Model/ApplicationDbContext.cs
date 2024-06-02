using Microsoft.EntityFrameworkCore;

namespace ExcellFile.Model
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext()
        {
            
        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options ):base(options)
        {
                
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Coustomer> Coustomers { get; set; }
        public DbSet<Employee> Employees { get; set; }
    }
}
