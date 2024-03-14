namespace GitHookz.Data;

public class StringConstants
{
    public const string APP_NAME = "GitHookz";

    public static readonly string BASE_DATA_PATH = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}\\GitHookz\\Data";
    public static readonly string WEBHOOK_DATA_FILE = $"{BASE_DATA_PATH}\\webhooks.dat";

}
