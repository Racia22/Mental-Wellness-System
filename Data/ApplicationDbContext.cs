using MentalWellnessSystem.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace MentalWellnessSystem.Data
{
    /// <summary>
    /// Legacy ApplicationDbContext - now we use MentalWellnessDbContext for everything
    /// This is kept for backward compatibility with Identity migrations
    /// </summary>
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
