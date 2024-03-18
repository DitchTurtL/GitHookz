using Dapper;
using GitHookz.Data.Models;
using Serilog;
using System.Data.SQLite;

namespace GitHookz.Services;

public class DatabaseService : IDatabaseService
{
    private readonly IConfiguration _configuration;
    private List<WebHookData> webHookDatas = new List<WebHookData>();
    private List<string> bannedRepos = new List<string>();


    private string _connectionString;
    private string GetConnectionString() => _connectionString ??= _configuration.GetConnectionString("DefaultConnection") ?? "Missing Connection String";

    public DatabaseService(IConfiguration configuration)
    {
        _configuration = configuration;

        Log.Information("Loading Repos...");
        bannedRepos = GetBannedRepos();
        webHookDatas = GetAllWebHooks();
        Log.Information($"{0} Repos Loaded.", webHookDatas.Count);
    }

    public bool AddWebHookData(WebHookData data)
    {
        // Block existing repo/recipient combinations
        if (webHookDatas.Any(x => x.RepoFullname == data.RepoFullname && x.RecipientId == data.RecipientId))
            return false;

        // Block banned repos
        if (bannedRepos.Contains(data.RepoFullname))
            return false;

        using var connection = new SQLiteConnection(GetConnectionString());

        var query = "INSERT INTO webhook_data (Type, GuildId, RecipientId, RecipientName, RepoFullname, AddedBy, AddedAt, AddedById) " +
            "VALUES (@Type, @GuildId, @RecipientId, @RecipientName, @RepoFullname, @AddedBy, @AddedAt, @AddedById)";
        
        connection.Open();
        var result = connection.ExecuteScalarAsync<int>(query, data);

        webHookDatas.Add(data);
        return true;
    }

    public List<WebHookData> GetWebHookDataByRepoFullName(string repoName)
    {
        using var connection = new SQLiteConnection(GetConnectionString());
        connection.Open();

        var query = "SELECT * FROM webhook_data WHERE RepoFullname = @RepoFullname";
        var result = connection.QueryAsync<WebHookData>(query, new { RepoFullname = repoName });

        return result.Result.ToList();
    }

    public List<WebHookData> GetAllWebHooks()
    {
        using var connection = new SQLiteConnection(GetConnectionString());
        connection.Open();

        var query = "SELECT * FROM webhook_data";
        var result = connection.QueryAsync<WebHookData>(query);

        return result.Result.ToList();
    }

    private List<string> GetBannedRepos()
    {
        using var connection = new SQLiteConnection(GetConnectionString());
        connection.Open();

        var query = "SELECT RepoFullname FROM banned_repos";
        var result = connection.Query<string>(query);

        return result.ToList();
    }
}