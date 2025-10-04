using CmlLib.Core;
using CmlLib.Core.Auth;
using CmlLib.Core.ProcessBuilder;
using MSLauncher.Core.Interfaces;

namespace MSLauncher.Core.Services
{
    public class GameLauncher : IGameLauncher
    {
        private readonly MinecraftLauncher _launcher;
        private IProgress<string> _progress;

        public GameLauncher()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string launcherBasePath = Path.Combine(appDataPath, "MSLauncher");
            string gameDataPath = Path.Combine(launcherBasePath, "gamedata");

            Directory.CreateDirectory(gameDataPath);

            var minecraftPath = new MinecraftPath(gameDataPath);

            _launcher = new MinecraftLauncher(minecraftPath);


            _launcher.FileProgressChanged += (sender, e) =>
            {
                _progress?.Report($"{e.EventType}: {e.Name} ({e.ProgressedTasks}/{e.TotalTasks})");
            };

            _launcher.ByteProgressChanged += (sender, e) =>
            {
                _progress?.Report($"Загрузка: {e.ProgressedBytes / 1024 / 1024}MB/{e.TotalBytes / 1024 / 1024}MB");
            };
        }

        public async Task LaunchGameAsync(string username, string version, int maxRamMb, IProgress<string> progress)
        {
            this._progress = progress;

            try
            {
                progress.Report("Подготовка к запуску...");
                var process = await _launcher.InstallAndBuildProcessAsync(version, new MLaunchOption
                {
                    Session = MSession.CreateOfflineSession(username),
                    MaximumRamMb = maxRamMb
                });

                progress.Report("Запуск Minecraft...");
                process.Start();
            }
            catch (Exception ex)
            {
                progress.Report($"Ошибка: {ex.Message}");
                throw;
            }
        }
    }
}