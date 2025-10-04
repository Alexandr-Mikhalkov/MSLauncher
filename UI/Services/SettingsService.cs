using MSLauncher.Core.Interfaces;
using System;
using MSLauncher.UI.Properties;

namespace MSLauncher.UI.Services
{
    public class SettingsService : ISettingsService
    {
        public event Action<string> BackgroundChanged;

        public int AllocatedRamMb { get; set; }

        private string _backgroundImage;
        public string BackgroundImage
        {
            get => _backgroundImage;
            set
            {
                if (_backgroundImage != value)
                {
                    _backgroundImage = value;
                    BackgroundChanged?.Invoke(_backgroundImage);
                }
            }
        }

        public void Load()
        {
            AllocatedRamMb = Settings.Default.AllocatedRamMb;
            BackgroundImage = Settings.Default.BackgroundImage;
        }

        public void Save()
        {
            Settings.Default.AllocatedRamMb = this.AllocatedRamMb;
            Settings.Default.BackgroundImage = this.BackgroundImage;
            Settings.Default.Save();
        }
    }
}