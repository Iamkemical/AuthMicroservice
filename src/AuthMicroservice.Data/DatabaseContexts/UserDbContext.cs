using AuthMicroservice.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuthMicroservice.Data.DatabaseContexts
{
    public class UserDbContext : DbContext
    {
        public UserDbContext(DbContextOptions<UserDbContext> options)
        :base(options)
        {
            
        }
        public DbSet<User> Users {get; set;}
    }
}