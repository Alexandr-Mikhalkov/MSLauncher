using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSLauncher.Core.Interfaces;
using MSLauncher.UI.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Windows;
using MSLauncher.UI.Utils;

namespace MSLauncher.UI.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        public event EventHandler<bool> RequestClose;
        private readonly ISettingsService _settingsService;

        [ObservableProperty]
        private double _allocatedRamMb;

        [ObservableProperty]
        private BackgroundImageInfo _selectedBackground;

        public ObservableCollection<BackgroundImageInfo> BackgroundImages { get; }

        public SettingsViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;
         
            BackgroundImages = new ObservableCollection<BackgroundImageInfo>
            {
                new BackgroundImageInfo { VersionName = "Классика", ImageUrl = "/Resources/classic_background.jpeg", Image = "classic" },
                new BackgroundImageInfo { VersionName = "Замок", ImageUrl = "/Resources/castle_background.jpeg", Image = "castle" },
                new BackgroundImageInfo { VersionName = "Лес", ImageUrl = "/Resources/forest_background.jpeg", Image = "forest" }
            };

            LoadSettings();
        }

        private void LoadSettings()
        {
            AllocatedRamMb = _settingsService.AllocatedRamMb;
            var savedId = _settingsService.BackgroundImage;
            SelectedBackground = BackgroundImages.FirstOrDefault(bg => bg.Image == savedId) ?? BackgroundImages[0];
        }

        [RelayCommand]
        private void Save()
        {
            _settingsService.AllocatedRamMb = (int)this.AllocatedRamMb;
            _settingsService.BackgroundImage = this.SelectedBackground?.Image ?? "classic";
            _settingsService.Save();

            RequestClose?.Invoke(this, true);
        }

        [RelayCommand]
        private void Close()
        {
            RequestClose?.Invoke(this, false);
        }

        [RelayCommand]
        private void OpenGameFolder()
        {
            string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            string launcherBasePath = Path.Combine(appDataPath, "MSLauncher");
            string gameDataPath = Path.Combine(launcherBasePath, "gamedata");

            Directory.CreateDirectory(gameDataPath);

            Process.Start("explorer.exe", gameDataPath);
        }

        [RelayCommand]
        private void Uninstall()
        {
            var message = "Вы уверены, что хотите удалить все данные игры?\n\n" +
                          "Будет удалена папка 'gamedata', содержащая:\n" +
                          "- Все версии игры\n" +
                          "- Сохранения, миры, моды и ресурс-паки\n\n" +
                          "Это действие необратимо!";

            var result = MessageBoxWindow.Show(message, "Подтверждение удаления", MessageBoxButton.YesNo, MessageIcon.Warning);

            if (result == MessageBoxResult.Yes)
            {
                RequestClose?.Invoke(this, false);

                Application.Current.Dispatcher.InvokeAsync(async () =>
                {
                    await Task.Delay(100);

                    try
                    {
                        string appDataPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                        string launcherBasePath = Path.Combine(appDataPath, "MSLauncher");
                        string gameDataPath = Path.Combine(launcherBasePath, "gamedata");

                        if (Directory.Exists(gameDataPath))
                        {
                            Directory.Delete(gameDataPath, true);
                        }

                        MessageBoxWindow.Show("Все игровые данные были успешно удалены. Приложение будет закрыто.",
                                              "Удаление завершено", MessageBoxButton.OK, MessageIcon.Info);

                        Application.Current.Shutdown();
                    }
                    catch (IOException ioEx)
                    {
                        MessageBoxWindow.Show($"Произошла ошибка при удалении файлов. Возможно, игра все еще запущена.\n\nОшибка: {ioEx.Message}",
                                               "Ошибка удаления", MessageBoxButton.OK, MessageIcon.Error);
                    }
                    catch (Exception ex)
                    {
                        MessageBoxWindow.Show($"Произошла непредвиденная ошибка при удалении:\n{ex.Message}",
                                               "Ошибка удаления", MessageBoxButton.OK, MessageIcon.Error);
                    }
                });
            }
        }
    }
}