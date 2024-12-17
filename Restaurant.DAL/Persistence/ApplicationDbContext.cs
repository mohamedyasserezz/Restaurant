using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Restaurant.DAL.Entites.Ontolgy;
using System.Reflection;

namespace Restaurant.DAL.Persistence
{
    public class ApplicationDbContext(DbContextOptions options) :
        IdentityDbContext<Customer>(options)
    {

        public DbSet<RdfIndividuals> Individuals { get; set; }
        public DbSet<RdfTriples> Triples { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            base.OnModelCreating(modelBuilder);
        }
    }
}
