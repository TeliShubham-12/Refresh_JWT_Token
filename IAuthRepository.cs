// IAuthRepository.cs
using System.Threading.Tasks;
using Api.Entities;

namespace Api.Repositories.Interfaces
{
    public interface IAuthRepository
    {
        Task<User> GetUserAsync(string username, string password);
        Task AddRefreshTokenAsync(string username, string refreshToken);
        Task<User> GetUserByRefreshTokenAsync(string username, string refreshToken);
    }
}
