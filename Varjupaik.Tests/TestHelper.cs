using Microsoft.EntityFrameworkCore;
using Varjupaik.Services.Data;

namespace Varjupaik.Tests;

public static class TestHelper
{
    // Uus tühi andmebaas mälus, iga test saab oma
    public static AppDbContext CreateDb()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new AppDbContext(options);
    }
}
