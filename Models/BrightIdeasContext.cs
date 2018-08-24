using Microsoft.EntityFrameworkCore;
 
namespace BrightIdeas.Models
{
    public class BrightIdeasContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public BrightIdeasContext(DbContextOptions<BrightIdeasContext> options) : base(options) { }
        public DbSet<User> users { get; set; }
        public DbSet<Post> posts { get; set; }
        public DbSet<Like> likes { get; set; }
    }
}