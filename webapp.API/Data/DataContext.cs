using Microsoft.EntityFrameworkCore;
using webapp.API.Models;

namespace webapp.API.Data
{

    //will let the app thinks this class as service
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {}

        //Values represent the table name 
        public DbSet<Value> Values { get; set; }

        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }

        public DbSet<Add>  Adds { get; set; }

        protected override void OnModelCreating(ModelBuilder builder){
            builder.Entity<Add>()
                .HasKey(k => new {k.AdderId, k.AddedId});
            builder.Entity<Add>()
                .HasOne(u => u.Added)
                .WithMany(u => u.Adders)
                .HasForeignKey(u => u.AddedId)
                .OnDelete(DeleteBehavior.Restrict);
             builder.Entity<Add>()
                .HasOne(u => u.Adder)
                .WithMany(u => u.Addeds)
                .HasForeignKey(u => u.AdderId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}