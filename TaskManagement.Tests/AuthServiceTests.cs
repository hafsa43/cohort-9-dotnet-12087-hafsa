using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Memory;
using Moq;
using TaskManagement.Application.DTOs;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Services;
using Xunit;


namespace TaskManagement.Tests;

public class AuthServiceTests
{
    // ── Helpers ──────────────────────────────────────────────────────────────
    private static Mock<UserManager<AppUser>> MockUserManager()
    {
        var store = new Mock<IUserStore<AppUser>>();
        return new Mock<UserManager<AppUser>>(
            store.Object, null!, null!, null!, null!, null!, null!, null!, null!);
    }

    private static IConfiguration BuildConfig() =>
        new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"]         = "TestSecretKey_MustBe32Chars_12345678",
                ["Jwt:Issuer"]      = "TestIssuer",
                ["Jwt:ExpireHours"] = "1"
            })
            .Build();

    // ── Register Tests ────────────────────────────────────────────────────────
    [Fact]
    public async Task Register_ShouldThrow_WhenEmailAlreadyExists()
    {
        // Arrange
        var userManager = MockUserManager();
        var existingUser = new AppUser { Email = "test@example.com" };

        userManager.Setup(m => m.FindByEmailAsync("test@example.com"))
                   .ReturnsAsync(existingUser);

        var service = new AuthService(userManager.Object, BuildConfig());
        var dto = new RegisterDto("John", "Doe", "test@example.com", "Password1!", "User");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => service.RegisterAsync(dto));
    }

    [Fact]
    public async Task Register_ShouldSucceed_WithValidData()
    {
        // Arrange
        var userManager = MockUserManager();

        userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                   .ReturnsAsync((AppUser?)null);

        userManager.Setup(m => m.CreateAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                   .ReturnsAsync(IdentityResult.Success);

        userManager.Setup(m => m.AddToRoleAsync(It.IsAny<AppUser>(), It.IsAny<string>()))
                   .ReturnsAsync(IdentityResult.Success);

        userManager.Setup(m => m.GetRolesAsync(It.IsAny<AppUser>()))
                   .ReturnsAsync(new List<string> { "User" });

        var service = new AuthService(userManager.Object, BuildConfig());
        var dto = new RegisterDto("Jane", "Smith", "jane@example.com", "Password1!", "User");

        // Act
        var result = await service.RegisterAsync(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("jane@example.com", result.Email);
        Assert.Equal("User", result.Role);
        Assert.NotEmpty(result.Token);
    }

    // ── Login Tests ───────────────────────────────────────────────────────────
    [Fact]
    public async Task Login_ShouldThrow_WhenUserNotFound()
    {
        var userManager = MockUserManager();
        userManager.Setup(m => m.FindByEmailAsync(It.IsAny<string>()))
                   .ReturnsAsync((AppUser?)null);

        var service = new AuthService(userManager.Object, BuildConfig());
        var dto = new LoginDto("nobody@example.com", "Password1!");

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => service.LoginAsync(dto));
    }

    [Fact]
    public async Task Login_ShouldThrow_WhenPasswordIsWrong()
    {
        var userManager = MockUserManager();
        var user = new AppUser { Email = "user@example.com", FirstName = "A", LastName = "B" };

        userManager.Setup(m => m.FindByEmailAsync("user@example.com"))
                   .ReturnsAsync(user);
        userManager.Setup(m => m.CheckPasswordAsync(user, "WrongPass"))
                   .ReturnsAsync(false);

        var service = new AuthService(userManager.Object, BuildConfig());

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            service.LoginAsync(new LoginDto("user@example.com", "WrongPass")));
    }

    [Fact]
    public async Task Login_ShouldReturn_Token_WhenCredentialsValid()
    {
        var userManager = MockUserManager();
        var user = new AppUser
        {
            Id        = Guid.NewGuid().ToString(),
            Email     = "user@example.com",
            FirstName = "Test",
            LastName  = "User"
        };

        userManager.Setup(m => m.FindByEmailAsync("user@example.com")).ReturnsAsync(user);
        userManager.Setup(m => m.CheckPasswordAsync(user, "Password1!")).ReturnsAsync(true);
        userManager.Setup(m => m.GetRolesAsync(user)).ReturnsAsync(new List<string> { "User" });

        var service = new AuthService(userManager.Object, BuildConfig());
        var result  = await service.LoginAsync(new LoginDto("user@example.com", "Password1!"));

        Assert.NotNull(result);
        Assert.Equal("user@example.com", result.Email);
        Assert.NotEmpty(result.Token);
        Assert.Equal("User", result.Role);
    }
}
