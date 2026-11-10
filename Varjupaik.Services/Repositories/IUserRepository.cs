using Varjupaik.Services.Models;

namespace Varjupaik.Services.Repositories;

public interface IUserRepository
{
    Task<User?> GetByEmailAsync(string email);
    Task<bool> EmailExistsAsync(string email);
    Task AddAsync(User user);
}
