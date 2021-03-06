using EventsCreator.EfStuff.DbModel;
using Microsoft.EntityFrameworkCore;

namespace EventsCreator.EfStuff
{
    public class WebContext : DbContext
    {
        public DbSet<Event> Events { get; set; }
        public DbSet<User> Users { get; set; }
        public WebContext(DbContextOptions options) : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().HasMany(u => u.CreatedEvents).WithOne(e => e.Creator);
            modelBuilder.Entity<User>().HasMany(u => u.InvitedEvents).WithMany(e => e.Participants);
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=usersdb;Username=postgres;Password=prostoweb20");
            base.OnConfiguring(optionsBuilder);
        }
    }
}
