namespace GitHookz.Data;

public static class StringConstants
{

    public const string APP_NAME = "GitHookz";
    public const string DEFAULT_WEBHOOK_ENDPOINT = "/webhook";
    public const string DEFAULT_EXTERNAL_URL = "http://localhost:5048/";

    public static readonly string BASE_DATA_PATH = $"{AppDomain.CurrentDomain.BaseDirectory}\\GitHookz\\Data";
    public static readonly string WEBHOOK_DATA_FILE = $"{BASE_DATA_PATH}\\webhooks.dat";

    public static string ToTitleCase(string str)
    {
        var cultureInfo = System.Threading.Thread.CurrentThread.CurrentCulture;
        return cultureInfo.TextInfo.ToTitleCase(str.ToLower());
    }
}
