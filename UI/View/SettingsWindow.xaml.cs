using MSLauncher.UI.ViewModels;
using System;
using System.Windows;
using System.Windows.Input;

namespace MSLauncher.UI.View
{
    public partial class SettingsWindow : Window
    {
        public SettingsWindow()
        {
            InitializeComponent();
            this.DataContextChanged += OnDataContextChanged;
        }

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is SettingsViewModel vm)
            {
                vm.RequestClose += (s, args) =>
                {
                    this.DialogResult = args;
                    this.Close();
                };
            }
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
            this.Close();
        }
    }
}