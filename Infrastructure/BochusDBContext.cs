using System.Data.Entity;
using Core;

namespace Infrastructure
{
    class BochusDBContext : DbContext
    {
        public BochusDBContext() : base("Bochus") { }

        public DbSet<Item> Items { get; set; }
        public DbSet<DetailedSubject> DetailedSubjects { get; set; }
        public DbSet<Subject> Subjects { get; set; }
        public DbSet<Genre> Genres { get; set; }
        public DbSet<Location> Locations { get; set; }
    }
}
