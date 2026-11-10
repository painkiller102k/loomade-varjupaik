using Microsoft.EntityFrameworkCore;

namespace Varjupaik.Services.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
}
