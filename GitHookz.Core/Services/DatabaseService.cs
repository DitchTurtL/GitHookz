using Dapper;
using GitHookz.Core.Data;
using Microsoft.Extensions.Options;
using System.Data.SQLite;

namespace GitHookz.Core.Services;

public class DatabaseService : IDatabaseService
{
    private readonly GitHookzAppSettings _settings;

    public DatabaseService(IOptions<GitHookzAppSettings> settings)
    {
        _settings = settings.Value;
    }

    private string GetConnectionString() =>
        $"Data Source={_settings.DatabasePath};Version=3;";

    private SQLiteConnection GetConnection() =>
        new(GetConnectionString());

    public async Task<User?> GetUserAsync(int id)
    {
        await using var connection = GetConnection();
        connection.Open();
        return await connection.QueryFirstOrDefaultAsync<User>(
            "SELECT * FROM Users WHERE Id = @Id",
            new { Id = id });
    }

    public async Task AddUserAsync(User user)
    {
        await using var connection = GetConnection();
        connection.Open();
        await connection.ExecuteAsync(
            "INSERT INTO Users (DiscordId, Username) VALUES (@DiscordId, @Username)",
            user);
    }

    public async Task UpdateUserAsync(User user)
    {
        await using var connection = GetConnection();
        connection.Open();
        await connection.ExecuteAsync(
            "UPDATE Users SET DiscordId = @DiscordId, Username = @Username WHERE Id = @Id",
            user);
    }

    public async Task DeleteUserAsync(User user)
    {
        await using var connection = GetConnection();
        connection.Open();
        await connection.ExecuteAsync(
            "DELETE FROM Users WHERE Id = @Id",
            user);
    }

    public async Task<IEnumerable<User>> GetUsersAsync()
    {
        await using var connection = GetConnection();
        connection.Open();

        var query = "SELECT * FROM Users;";
        return await connection.QueryAsync<User>(query);
    }

    public async Task<Repository?> GetRepositoryAsync(int id)
    {
        await using var connection = GetConnection();
        connection.Open();
        return await connection.QueryFirstOrDefaultAsync<Repository>(
            "SELECT * FROM Repositories WHERE Id = @Id",
            new { Id = id });
    }

    public async Task AddRepositoryAsync(Repository repository)
    {
        await using var connection = GetConnection();
        connection.Open();
        await connection.ExecuteAsync(
            "INSERT INTO Repositories (Name, UserId, ChannelId) VALUES (@Name, @UserId, @ChannelId)",
            repository);
    }

    public async Task UpdateRepositoryAsync(Repository repository)
    {
        await using var connection = GetConnection();
        connection.Open();
        await connection.ExecuteAsync(
            "UPDATE Repositories SET Name = @Name, UserId = @UserId, ChannelId = @ChannelId WHERE Id = @Id",
            repository);
    }

    public async Task DeleteRepositoryAsync(Repository repository)
    {
        await using var connection = GetConnection();
        connection.Open();
        await connection.ExecuteAsync(
            "DELETE FROM Repositories WHERE Id = @Id",
            repository);
    }

    public async Task<IEnumerable<Repository>> GetRepositoriesAsync()
    {
        await using var connection = GetConnection();
        connection.Open();
        return await connection.QueryAsync<Repository>(
            "SELECT * FROM Repositories;");
    }
}