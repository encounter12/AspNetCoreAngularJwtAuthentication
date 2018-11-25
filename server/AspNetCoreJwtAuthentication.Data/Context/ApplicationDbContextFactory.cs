using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AspNetCoreJwtAuthentication.Data.Context
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

            optionsBuilder
                .UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=AspNetCoreJwtAuthentication;Trusted_Connection=True;Connect Timeout=100");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
