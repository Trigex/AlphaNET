using Microsoft.EntityFrameworkCore;
using NLog;
using System.ComponentModel.DataAnnotations;
using System.Linq;

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
                AlphaServer.Config.Db.ConnectionString);
        }
    }

    public static class QueryUserContext
    {
        public static User GetUser(string realIp)
        {
            using (var db = new UserContext())
            {
                return db.Users.FirstOrDefault(u => u.RealIp == realIp);
            }
        }

        public static int GetVirtualIpCount(string virtualIp)
        {
            using (var db = new UserContext())
            {
                return db.Users.Where(u => u.VirtualIp == virtualIp).ToList().Count;
            }
        }

        public static void AddUser(User user)
        {
            using (var db = new UserContext())
            {
                db.Users.Add(user);
                db.SaveChanges();
            }
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
