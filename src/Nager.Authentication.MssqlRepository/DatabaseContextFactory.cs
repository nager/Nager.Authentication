using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Nager.Authentication.MssqlRepository
{
    /// <summary>
    /// Database Context Factory
    /// </summary>
    /// <remarks>Only required for database migration</remarks>
    public class DatabaseContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
    {
        public DatabaseContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<DatabaseContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=CourseManagement;User Id=sa;Password=Secure-Password.1234;");

            return new DatabaseContext(optionsBuilder.Options);
        }
    }
}