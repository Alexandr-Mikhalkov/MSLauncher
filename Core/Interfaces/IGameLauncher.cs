namespace MSLauncher.Core.Interfaces
{
    public interface IGameLauncher
    {
        Task LaunchGameAsync(string username, string version, int maxRamMb, IProgress<string> progress);
    }
}