using Microsoft.EntityFrameworkCore;

namespace ResultPatternTesting.Entity
{
    public class DataDbContext : DbContext, IDbContext
    {
        public DbSet<Data> Datas { get; set; }
        public DataDbContext(DbContextOptions<DataDbContext> options) : base(options)
        {
            
        }
        //public DataDbContext()
        //{
            
        //}
    }

    public interface IDbContext
    {
    }
}
