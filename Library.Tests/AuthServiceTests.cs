using FluentAssertions;
using Library.Application.Dtos.Auth;
using Library.Infrastructure.Services;
using Library.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Xunit;

namespace Library.Tests;

public class AuthServiceTests
{
    private static IConfiguration BuildTestConfig()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Jwt:Key"] = "4yvH1kkrzwfcSR4hChI8WNq3K0GzoawiaRGOrdvb",
                ["Jwt:Issuer"] = "Library.Api",
                ["Jwt:Audience"] = "Library.Client",
                ["Jwt:ExpiresMinutes"] = "60"
            })
            .Build();
    }

    [Fact]
    public async Task RegisterAsync_ReturnsTrue_ForNewUser()
    {
        using var db = new SqliteInMemoryDb();
        var auth = new AuthService(db.DbContext, BuildTestConfig());

        var ok = await auth.RegisterAsync(new RegisterDto("admin", "Passw0rd!", "Admin"));

        ok.Should().BeTrue();
    }

    [Fact]
    public async Task RegisterAsync_ReturnsFalse_ForDuplicateUsername()
    {
        using var db = new SqliteInMemoryDb();
        var auth = new AuthService(db.DbContext, BuildTestConfig());

        var dto = new RegisterDto("admin", "Passw0rd!", "Admin");

        (await auth.RegisterAsync(dto)).Should().BeTrue();
        (await auth.RegisterAsync(dto)).Should().BeFalse();
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_ForUnknownUser()
    {
        using var db = new SqliteInMemoryDb();
        var auth = new AuthService(db.DbContext, BuildTestConfig());

        var result = await auth.LoginAsync(new LoginDto("missing", "Passw0rd!"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ReturnsNull_ForWrongPassword()
    {
        using var db = new SqliteInMemoryDb();
        var auth = new AuthService(db.DbContext, BuildTestConfig());

        await auth.RegisterAsync(new RegisterDto("admin", "Passw0rd!", "Admin"));

        var result = await auth.LoginAsync(new LoginDto("admin", "WrongPassword"));

        result.Should().BeNull();
    }

    [Fact]
    public async Task LoginAsync_ReturnsToken_ForValidCredentials()
    {
        using var db = new SqliteInMemoryDb();
        var auth = new AuthService(db.DbContext, BuildTestConfig());

        await auth.RegisterAsync(new RegisterDto("admin", "Passw0rd!", "Admin"));

        var result = await auth.LoginAsync(new LoginDto("admin", "Passw0rd!"));

        result.Should().NotBeNull();
        result!.TokenType.Should().Be("Bearer");
        result.Username.Should().Be("admin");
        result.Role.Should().Be("Admin");
        result.AccessToken.Should().NotBeNullOrWhiteSpace();
        result.ExpiresInSeconds.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task LoginAsync_Throws_WhenJwtKeyMissing()
    {
        using var db = new SqliteInMemoryDb();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>()) // provider exists but no Jwt:Key
            .Build();

        var auth = new AuthService(db.DbContext, config);

        await auth.RegisterAsync(new RegisterDto("admin", "Passw0rd!", "Admin"));

        Func<Task> act = async () =>
        {
            await auth.LoginAsync(new LoginDto("admin", "Passw0rd!"));
        };

        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("Jwt:Key missing from configuration.");
    }
}