using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Exceptions;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Tests;

public class AuthServiceTests
{
    // Test 1 (mock): uus kasutaja saab rolli Visitor ja parool salvestatakse räsina
    [Fact]
    public async Task Register_NewUser_GetsVisitorRoleAndHashedPassword()
    {
        var users = new Mock<IUserRepository>();
        users.Setup(r => r.EmailExistsAsync("mari@test.ee")).ReturnsAsync(false);
        var service = new AuthService(users.Object, NullLogger<AuthService>.Instance);

        var user = await service.RegisterAsync(new RegisterDto { Email = "Mari@Test.ee", Name = "Mari", Password = "mari123" });

        Assert.Equal(Roles.Visitor, user.Role);
        Assert.Equal("mari@test.ee", user.Email);
        Assert.NotEqual("mari123", user.PasswordHash);
        users.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    // Test 2: vale parooliga sisselogimine annab 401 (UnauthorizedException)
    [Fact]
    public async Task Login_WrongPassword_ThrowsUnauthorized()
    {
        var db = TestHelper.CreateDb();
        var service = new AuthService(new UserRepository(db), NullLogger<AuthService>.Instance);
        await service.RegisterAsync(new RegisterDto { Email = "kati@test.ee", Name = "Kati", Password = "kati123" });

        var ok = await service.LoginAsync(new LoginDto { Email = "kati@test.ee", Password = "kati123" });
        Assert.Equal("kati@test.ee", ok.Email);

        await Assert.ThrowsAsync<UnauthorizedException>(() =>
            service.LoginAsync(new LoginDto { Email = "kati@test.ee", Password = "vale" }));
    }
}
