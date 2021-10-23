using DotNetTeacherBot.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNetTeacherBot.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }

        public DbSet<Question> Questions {get; set;}
    }
}