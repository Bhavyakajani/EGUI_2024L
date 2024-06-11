using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using test.Models;

namespace test.Data
{

    public class ApplicationDBContext(DbContextOptions<ApplicationDBContext> options, IConfiguration configuration) : IdentityDbContext(options)
    {

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connString = configuration.GetConnectionString("Default");

            object value = optionsBuilder.UseSqlServer(
                connString ?? throw new InvalidOperationException("Connection string 'Default' not found"),
                x => x.UseNetTopologySuite());
        }
        public required DbSet<ApplicationUser> ApplicationUser { get; set; }
        public required DbSet<EmployeeEdit> Employees { get; set; }
        public required DbSet<TicketEdit> Tickets { get; set; }
        
    }
}
