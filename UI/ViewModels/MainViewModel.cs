using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSLauncher.Core.Interfaces;
using MSLauncher.Core.Services;
using MSLauncher.UI.View;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Windows;

namespace MSLauncher.UI.ViewModels
{
    public class VersionInfo : ObservableObject
    {
        public string VersionName { get; set; }
        public string ImageUrl { get; set; }
        public bool IsInstalled { get; set; }

    }

    public partial class MainViewModel : ObservableObject
    {
        public event Action LogoutRequested;

        private readonly IGameLauncher _gameLauncher;
        private readonly ISettingsService _settingsService;
        private readonly Dictionary<string, string> _backgroundMap;
        private const string defaultImage = "/Resources/background.png";

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _statusText = "Выберите версию для запуска";

        [ObservableProperty]
        private string _buttonText = "Играть";

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LaunchGameCommand))]
        private bool _isBusy = false;

        [ObservableProperty]
        private string _currentBackgroundImage;

        public ObservableCollection<VersionInfo> RecentVersions { get; }
        public ObservableCollection<VersionInfo> AllVersions { get; }

        [ObservableProperty]
        private VersionInfo _recentSelectedVersion;

        [ObservableProperty]
        private VersionInfo _allSelectedVersion;

        public VersionInfo SelectedVersion => RecentSelectedVersion ?? AllSelectedVersion;

        public MainViewModel(IGameLauncher gameLauncher, ISettingsService settingsService)
        {
            _gameLauncher = gameLauncher;
            _settingsService = settingsService;

            _backgroundMap = new Dictionary<string, string>
            {
                { "classic", "/Resources/classic_background.jpeg" },
                { "castle", "/Resources/castle_background.jpeg" },
                { "forest", "/Resources/forest_background.jpeg" }
            };

            _settingsService.Load();
            UpdateBackground(_settingsService.BackgroundImage);
            
            _settingsService.BackgroundChanged += UpdateBackground;

            RecentVersions = new ObservableCollection<VersionInfo>();
            AllVersions = new ObservableCollection<VersionInfo>();

            LoadVersions();
        }

        private void UpdateBackground(string image)
        {
            if (image != null && _backgroundMap.TryGetValue(image, out var imagePath))
            {
                CurrentBackgroundImage = imagePath;
            }
            else
            {
                CurrentBackgroundImage = _backgroundMap["classic"];
            }
        }

        partial void OnRecentSelectedVersionChanged(VersionInfo value)
        {
            if (value != null)
            {
                AllSelectedVersion = null;
            }

            OnPropertyChanged(nameof(SelectedVersion));
            UpdateButtonState();
            LaunchGameCommand.NotifyCanExecuteChanged();
        }

        partial void OnAllSelectedVersionChanged(VersionInfo value)
        {
            if (value != null)
            {
                RecentSelectedVersion = null;
            }

            OnPropertyChanged(nameof(SelectedVersion));
            UpdateButtonState();
            LaunchGameCommand.NotifyCanExecuteChanged();
        }

        private void UpdateButtonState()
        {
            if (SelectedVersion == null)
            {
                ButtonText = "Играть";
                StatusText = "Выберите версию для запуска";
                return;
            }

            if (SelectedVersion.IsInstalled)
            {
                ButtonText = "Играть";
                StatusText = $"Готов к запуску {SelectedVersion.VersionName}";
            }
            else
            {
                ButtonText = "Установить";
                StatusText = $"Требуется установка версии {SelectedVersion.VersionName}";
            }
        }

        private void LoadVersions()
        {
            RecentVersions.Clear();
            AllVersions.Clear();

            try
            {
                string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                string launcherBasePath = Path.Combine(appDataPath, "MSLauncher");
                string gameDataPath = Path.Combine(launcherBasePath, "gamedata");
                string versionsPath = Path.Combine(gameDataPath, "versions");


                var installedVersionNames = new HashSet<string>();

                if (Directory.Exists(versionsPath))
                {
                    var versionsDirectory = new DirectoryInfo(versionsPath);
                    var installedDirs = versionsDirectory.GetDirectories();

                    foreach (var dir in installedDirs)
                    {
                        installedVersionNames.Add(dir.Name);
                    }
                }

                if (Directory.Exists(versionsPath))
                {
                    var versionsDirectory = new DirectoryInfo(versionsPath);
                    var recentDirs = versionsDirectory.GetDirectories().OrderByDescending(dir => dir.LastWriteTime).Take(3);

                    foreach (var dir in recentDirs)
                    {
                        RecentVersions.Add(new VersionInfo
                        {
                            VersionName = dir.Name,
                            ImageUrl = defaultImage,
                            IsInstalled = true
                        });
                    }
                }

                var masterVersionList = new List<string>
                {
                    "1.20.4", "1.20.1", "1.19.2", "1.18.2", "1.17.1", "1.16.5",
                    "1.12.2", "1.8.9", "1.7.10"
                };

                foreach (var versionName in masterVersionList)
                {
                    bool isVersionInstalled = installedVersionNames.Contains(versionName);

                    AllVersions.Add(new VersionInfo
                    {
                        VersionName = versionName,
                        ImageUrl = defaultImage,
                        IsInstalled = isVersionInstalled
                    });
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Ошибка при сканировании версий: {ex.Message}";
            }
        }

        [RelayCommand(CanExecute = nameof(CanLaunch))]
        private async Task LaunchGameAsync()
        {
            IsBusy = true;
            var progress = new Progress<string>(status => StatusText = status);

            try
            {
                await _gameLauncher.LaunchGameAsync(Username, SelectedVersion.VersionName, _settingsService.AllocatedRamMb, progress);
                Application.Current.Shutdown();
            }
            catch (Exception ex)
            {
                StatusText = $"Ошибка: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanLaunch()
        {
            return !IsBusy && SelectedVersion != null;
        }

        [RelayCommand]
        private void ShowSettings()
        {
            var settingsViewModel = new SettingsViewModel(_settingsService);
            var settingsWindow = new SettingsWindow
            {
                DataContext = settingsViewModel,
                Owner = Application.Current.MainWindow
            };
            settingsWindow.ShowDialog();
        }

        [RelayCommand]
        private void Logout()
        {
            LogoutRequested?.Invoke();
        }
    }
}