using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace SecureApi.API.Identity.Utilities
{
    public class IdentityContextFactory : IDesignTimeDbContextFactory<AppIdentityDbContext>
    {
        public AppIdentityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppIdentityDbContext>();
            optionsBuilder.UseNpgsql("User ID=postgres;Password=nagababu;Host=localhost;Port=5432;Database=SecureApiIdentityDb;Pooling=true;");

            return new AppIdentityDbContext(optionsBuilder.Options);
        }
    }
}
