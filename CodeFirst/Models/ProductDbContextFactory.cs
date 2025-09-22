using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace CodeFirst.Models
{
    public class ProductDbContextFactory : IDesignTimeDbContextFactory<ProductDbContext>
    {
        public ProductDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ProductDbContext>();
            optionsBuilder.UseSqlServer(
                "Server=localhost,1433;Database=College;User Id=sa;Password=YourStrong!Passw0rd;Encrypt=True;TrustServerCertificate=True;"
            );

            return new ProductDbContext(optionsBuilder.Options);
        }
    }
}
