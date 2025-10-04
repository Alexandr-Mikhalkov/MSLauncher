using System;

namespace MSLauncher.Core.Interfaces
{
    public interface ISettingsService
    {
        event Action<string> BackgroundChanged;

        int AllocatedRamMb { get; set; }
        string BackgroundImage { get; set; }

        void Load();
        void Save();
    }
}