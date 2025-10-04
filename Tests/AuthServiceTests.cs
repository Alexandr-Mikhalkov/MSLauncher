using Microsoft.EntityFrameworkCore;
using MSLauncher.Core.Data;
using MSLauncher.Core.Entities;
using MSLauncher.Core.Services;
using System.Threading.Tasks;
using Xunit;

namespace MSLauncher.Tests
{
    public class AuthServiceTests
    {
        private readonly AppDbContext _context;
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            var options = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: System.Guid.NewGuid().ToString())
                .Options;

            _context = new AppDbContext(options);

            _authService = new AuthService(_context);
        }

        [Fact]
        public async Task LoginAsync_1()
        {
            var username = "testuser";
            var password = "password123";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password);
            _context.Users.Add(new User { Username = username, PasswordHash = hashedPassword });
            await _context.SaveChangesAsync();

            var result = await _authService.LoginAsync(username, password);

            Assert.NotNull(result); 
            Assert.Equal(username, result.Username);
        }

        [Fact]
        public async Task LoginAsync_2()
        {
            var username = "testuser";
            var correctPassword = "password123";
            var wrongPassword = "wrongpassword";
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(correctPassword);
            _context.Users.Add(new User { Username = username, PasswordHash = hashedPassword });
            await _context.SaveChangesAsync();

            var result = await _authService.LoginAsync(username, wrongPassword);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_3()
        {
            var result = await _authService.LoginAsync("nonexistent", "any_password");

            Assert.Null(result);
        }

        [Fact]
        public async Task RegisterAsync_4()
        {
            var username = "newuser";
            var password = "newpassword";

            var result = await _authService.RegisterAsync(username, password);

            Assert.True(result);

            var userInDb = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            Assert.NotNull(userInDb);
            Assert.True(BCrypt.Net.BCrypt.Verify(password, userInDb.PasswordHash));
        }

        [Fact]
        public async Task RegisterAsync_5()
        {
            var username = "existinguser";
            _context.Users.Add(new User { Username = username, PasswordHash = "somehash" });
            await _context.SaveChangesAsync();

            var result = await _authService.RegisterAsync(username, "any_password");

            Assert.False(result);

            var userCount = await _context.Users.CountAsync();
            Assert.Equal(1, userCount);
        }
    }
}