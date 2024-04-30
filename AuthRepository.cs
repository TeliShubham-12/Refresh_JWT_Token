// AuthRepository.cs
using Api.Entities;
using Api.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace Api.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;

        public AuthRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserAsync(string username, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username && u.Password == password);
        }

        public async Task AddRefreshTokenAsync(string username, string refreshToken)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            if (user != null)
            {
                _context.RefreshTokens.Add(new RefreshToken { UserId = user.UserId, Token = refreshToken });
                await _context.SaveChangesAsync();
            }
        }

        public async Task<User> GetUserByRefreshTokenAsync(string username, string refreshToken)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username &&
                                                                _context.RefreshTokens.Any(rt => rt.UserId == u.UserId && rt.Token == refreshToken));
        }
    }
}
