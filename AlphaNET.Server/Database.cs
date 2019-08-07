using Microsoft.EntityFrameworkCore;
using NLog;
using System.ComponentModel.DataAnnotations;

namespace AlphaNET.Server
{
    class UserContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        private Logger _logger;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _logger = LogManager.GetCurrentClassLogger();

            optionsBuilder.UseSqlServer(
                @"Server=(localdb)\mssqllocaldb;Database=AlphaNET;Integrated Security=True");
        }
    }

    public class User
    {
        [Key]
        public string RealIp { get; set; }
        [Required]
        public string VirtualIp { get; set; }
    }
}
