using CommunityToolkit.Mvvm.ComponentModel;

namespace MSLauncher.UI.Models
{
    public partial class BackgroundImageInfo : ObservableObject
    {
        public string VersionName { get; set; }
        public string ImageUrl { get; set; }
        public string Image { get; set; }
    }
}