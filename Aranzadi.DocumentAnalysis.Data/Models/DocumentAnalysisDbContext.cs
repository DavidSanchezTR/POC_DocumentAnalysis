using Microsoft.EntityFrameworkCore;

namespace Aranzadi.DocumentAnalysis.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) 
        {
            
        }
    }
}