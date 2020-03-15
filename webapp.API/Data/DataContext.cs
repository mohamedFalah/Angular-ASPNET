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
    }
}