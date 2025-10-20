using Application.Features.TransportFeature.TransportInfo;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Persistence.Context
{
    public partial class DataContext : DbContext
    {
        public DataContext()
        {
        }

        public DataContext(DbContextOptions<DataContext> options) : base(options)
        {
            Database.SetCommandTimeout(TimeSpan.FromSeconds(300));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            modelBuilder.Entity<TransportInfoResponse>()
                .HasNoKey();
        }


        public DbSet<IntegrationAPIUser> IntegrationAPIUser { get; set; }



        public DbSet<Employee> Employee { get; set; }


    }


}
