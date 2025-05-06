using Microsoft.EntityFrameworkCore;
using RazorPage.Models;

namespace RazorPage.Data
{
    public class SchoolDbContext : DbContext
    {
        public SchoolDbContext(DbContextOptions<SchoolDbContext> options)
            : base(options)
        {
        }

        public DbSet<ClassInformationTable> Classes { get; set; }
    }
}
