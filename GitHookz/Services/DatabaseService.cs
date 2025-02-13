using Dapper;
using GitHookz.Data;
using Microsoft.Data.Sqlite;

namespace GitHookz.Services;

public class DatabaseService : IDatabaseService
{
    private string CONNECTION_STRING = "Data Source=./Data/GitHookz.db";

    private readonly ILogger<DatabaseService> _logger;

    public DatabaseService(ILogger<DatabaseService> logger)
    {
        _logger = logger;

        _logger.LogInformation("DatabaseService initialized");
        DoDBMigrations();
    }

    private void DoDBMigrations()
    {
        _logger.LogInformation("Checking DB Migrations");
        using var conn = GetConnection();
        conn.Open();

        var sql = "SELECT name FROM sqlite_master WHERE type = 'table'";
        var tables = conn.Query<string>(sql).ToList();

        if (!tables.Contains("projects"))
        {
            _logger.LogInformation("Creating projects table");
            sql = "CREATE TABLE projects (Id INTEGER PRIMARY KEY AUTOINCREMENT, RepositoryName TEXT, RepositoryUrl TEXT, OwnerId TEXT, ChannelId TEXT, ChannelName TEXT, CreatedAt DATETIME, LastUpdatedAt DATETIME, IsPublished INTEGER)";
            conn.Execute(sql);
        }

        if (!tables.Contains("users"))
        {
            _logger.LogInformation("Creating users table");
            sql = "CREATE TABLE users (Id INTEGER PRIMARY KEY AUTOINCREMENT, DiscordId TEXT, Name TEXT)";
            conn.Execute(sql);
        }

        _logger.LogInformation("DB Migrations complete");
    }

    private SqliteConnection GetConnection() => new SqliteConnection(CONNECTION_STRING);

    public void AddProject(ProjectData projectData)
    {
        projectData.RepositoryUrl = projectData.RepositoryUrl?.ToLower();

        using var conn = GetConnection();
        conn.Open();

        var sql = "INSERT INTO projects (RepositoryName, RepositoryUrl, OwnerId, ChannelId, ChannelName, CreatedAt, LastUpdatedAt, IsPublished) VALUES (@RepositoryName, @RepositoryUrl, @OwnerId, @ChannelId, @ChannelName, @CreatedAt, @LastUpdatedAt, @IsPublished)";
        conn.Execute(sql, projectData);
    }

    public ProjectData? GetProjectById(int projectId)
    {
        using var conn = GetConnection();
        conn.Open();

        var sql = "SELECT * FROM projects WHERE Id = @Id";
        return conn.QueryFirstOrDefault<ProjectData>(sql, new { Id = projectId });
    }

    public IEnumerable<ProjectData> GetAllProjects()
    {
        using var conn = GetConnection();
        conn.Open();
        var sql = "SELECT * FROM projects";
        return conn.Query<ProjectData>(sql);
    }

    public IEnumerable<ProjectData> GetProjectsByOwnerId(string ownerId)
    {
        using var conn = GetConnection();
        conn.Open();
        var sql = "SELECT * FROM projects WHERE OwnerId = @OwnerId";
        return conn.Query<ProjectData>(sql, new { OwnerId = ownerId });
    }

    public int AddUser(UserData userData)
    {
        using var conn = GetConnection();
        conn.Open();

        var sql = "INSERT INTO users (DiscordId, Name) VALUES (@DiscordId, @Name)";
        conn.Execute(sql, userData);

        var sqlRet = "SELECT last_insert_rowid()";
        return conn.ExecuteScalar<int>(sqlRet);
    }

    public UserData? GetUserById(int userId)
    {
        using var conn = GetConnection();
        conn.Open();

        var sql = "SELECT * FROM users WHERE Id = @Id";
        return conn.QueryFirstOrDefault<UserData>(sql, new { Id = userId });
    }

    public void UpdateProject(ProjectData projectData)
    {
        projectData.RepositoryUrl = projectData.RepositoryUrl?.ToLower();

        using var conn = GetConnection();
        conn.Open();

        var sql = "UPDATE projects SET RepositoryName = @RepositoryName, RepositoryUrl = @RepositoryUrl, OwnerId = @OwnerId, ChannelId = @ChannelId, IsPublished = @IsPublished WHERE Id = @Id";
        conn.Execute(sql, projectData);
    }

    public UserData GetOrAddUser(UserData userData)
    {
        using var conn = GetConnection();
        conn.Open();

        var sql = "SELECT * FROM users WHERE DiscordId = @DiscordId";
        var user = conn.QueryFirstOrDefault<UserData>(sql, new { DiscordId = userData.DiscordId });
        if (user == null)
        {
            sql = "INSERT INTO users (DiscordId, Name) VALUES (@DiscordId, @Name)";
            conn.Execute(sql, userData);
            sql = "SELECT * from users WHERE DiscordId=@DiscordId";
            user = conn.QueryFirstOrDefault<UserData>(sql, new { DiscordId = userData.DiscordId });
            return user;
        }
        return user;
    }

    public ProjectData? GetProjectByRepositoryUrl(string repositoryUrl)
    {
        repositoryUrl = repositoryUrl.ToLower();

        using var conn = GetConnection();
        conn.Open();

        var sql = "SELECT * FROM projects WHERE RepositoryUrl = @RepositoryUrl";
        return conn.QueryFirstOrDefault<ProjectData>(sql, new { RepositoryUrl = repositoryUrl });
    }
}
