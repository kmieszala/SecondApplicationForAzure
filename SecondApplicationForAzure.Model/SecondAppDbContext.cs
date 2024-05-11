using Microsoft.EntityFrameworkCore;
using SecondApplicationForAzure.Model.DbSets;

namespace SecondApplicationForAzure.Model;

public class SecondAppDbContext : DbContext
{
    public SecondAppDbContext(DbContextOptions<SecondAppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Student> Students { get; set; }
}