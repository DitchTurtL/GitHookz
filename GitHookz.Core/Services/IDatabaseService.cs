using GitHookz.Core.Data;

namespace GitHookz.Core.Services;

public interface IDatabaseService
{
    Task<User?> GetUserAsync(int id);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(User user);
    Task<IEnumerable<User>> GetUsersAsync();

    Task<Repository?> GetRepositoryAsync(int id);
    Task AddRepositoryAsync(Repository repository);
    Task UpdateRepositoryAsync(Repository repository);
    Task DeleteRepositoryAsync(Repository repository);
    Task<IEnumerable<Repository>> GetRepositoriesAsync();
}