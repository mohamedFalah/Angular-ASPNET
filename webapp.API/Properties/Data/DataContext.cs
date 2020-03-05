using Microsoft.EntityFrameworkCore;
using webapp.API.Controllers.Models;

namespace webapp.API.Properties.Data
{

    //will let the app thinks this class as service
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {}

        //Values represent the table name 
        public DbSet<Value> Values { get; set; }
    }
}