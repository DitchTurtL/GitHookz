namespace GitHookz.Data.State;

public class NavMenuState
{
    public event Action? OnChange;

    public void NotifyStateChanged() => OnChange?.Invoke();
}
