using Calendar.BE.Models;
using Microsoft.EntityFrameworkCore;

namespace Calendar.BE.Dal
{
    public class DataContext : DbContext
    {
        public DataContext() { }
        public DataContext(DbContextOptions<DataContext> options) :
         base(options)
        {
        }
        public virtual DbSet<Account> Accounts { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfiguration(new AccountConfiguration());
        }
    }
}
