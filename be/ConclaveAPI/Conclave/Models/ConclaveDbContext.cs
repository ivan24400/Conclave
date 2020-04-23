using Microsoft.EntityFrameworkCore;

namespace Conclave.Models
{
    public class ConclaveDbContext : DbContext
    {

        public ConclaveDbContext() { }

        public ConclaveDbContext(DbContextOptions<ConclaveDbContext> context) : base(context) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=.;Initial Catalog=conclave;Integrated Security=False;Persist Security Info=False;User ID=sheep;Password=Blackface-1234");
        }
        public DbSet<UserSocial> UserSocial { get; set; }
        public DbSet<Post> Post { get; set; }
        public DbSet<PostMedia> PostMedia { get; set; }

    }

}
