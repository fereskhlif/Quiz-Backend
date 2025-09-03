using Microsoft.EntityFrameworkCore;
using Quizzz.Models;

namespace Quizzz
{
    public class CollegeDBContext : DbContext
    {
        public CollegeDBContext(DbContextOptions<CollegeDBContext> options) : base(options) 
        {
        
                
        }
        public DbSet<Utilisateur> Utilisateur { get; set; }
        public DbSet<Test> Test { get; set; }
        public DbSet<Question> Question { get; set; }
        public DbSet<Section> Section { get; set; }
    }
}
