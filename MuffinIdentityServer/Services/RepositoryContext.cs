using Microsoft.EntityFrameworkCore;
using MuffinIdentityServer.Models;

namespace MuffinIdentityServer.Services
{
    public class RepositoryContext : DbContext
    {
        public RepositoryContext(DbContextOptions<RepositoryContext> options) : base(options)
        {
        }

        public DbSet<UserProfile> UserProfiles { get; set; }
    }
}