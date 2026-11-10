using Moq;
using Varjupaik.Services;
using Varjupaik.Services.Dtos;
using Varjupaik.Services.Models;
using Varjupaik.Services.Repositories;

namespace Varjupaik.Tests;

// AuthService testid, kus repositoorium on asendatud mockiga (Moq)
public class AuthServiceTests
{
    [Fact]
    public async Task Register_NewUser_GetsVisitorRoleAndHashedPassword()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.EmailExistsAsync("mari@test.ee")).ReturnsAsync(false);
        var service = new AuthService(repo.Object);

        var user = await service.RegisterAsync(new RegisterDto { Email = "Mari@Test.ee", Name = "Mari", Password = "mari123" });

        Assert.NotNull(user);
        Assert.Equal(Roles.Visitor, user.Role);
        Assert.Equal("mari@test.ee", user.Email);
        Assert.NotEqual("mari123", user.PasswordHash);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Once);
    }

    [Fact]
    public async Task Register_EmailAlreadyExists_ReturnsNull()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.EmailExistsAsync("mari@test.ee")).ReturnsAsync(true);
        var service = new AuthService(repo.Object);

        var user = await service.RegisterAsync(new RegisterDto { Email = "mari@test.ee", Name = "Mari", Password = "mari123" });

        Assert.Null(user);
        repo.Verify(r => r.AddAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Login_CorrectAndWrongPassword()
    {
        // Loome kasutaja läbi teenuse, et parool oleks õigesti räsitud
        User? saved = null;
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        repo.Setup(r => r.AddAsync(It.IsAny<User>())).Callback<User>(u => saved = u).Returns(Task.CompletedTask);
        repo.Setup(r => r.GetByEmailAsync("kati@test.ee")).ReturnsAsync(() => saved);
        var service = new AuthService(repo.Object);
        await service.RegisterAsync(new RegisterDto { Email = "kati@test.ee", Name = "Kati", Password = "kati123" });

        var ok = await service.LoginAsync(new LoginDto { Email = "kati@test.ee", Password = "kati123" });
        var wrong = await service.LoginAsync(new LoginDto { Email = "kati@test.ee", Password = "vale" });

        Assert.NotNull(ok);
        Assert.Null(wrong);
    }

    [Fact]
    public async Task Login_UnknownEmail_ReturnsNull()
    {
        var repo = new Mock<IUserRepository>();
        repo.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);
        var service = new AuthService(repo.Object);

        var user = await service.LoginAsync(new LoginDto { Email = "pole@test.ee", Password = "midagi" });

        Assert.Null(user);
    }
}
