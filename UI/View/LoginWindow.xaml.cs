using MSLauncher.UI.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using MSLauncher.UI.Utils;

namespace MSLauncher.UI.View
{
    public partial class LoginWindow : Window
    {
        public LoginWindow(LoginViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;

            PasswordBoxHidden.PasswordChanged += (s, e) =>
            {
                if (DataContext is LoginViewModel vm)
                {
                    vm.Password = PasswordBoxHidden.Password;
                }
            };

            PasswordBoxHidden.PasswordChanged += (s, e) =>
            {
                PasswordBoxHelper.SetPasswordLength(PasswordBoxHidden, PasswordBoxHidden.Password.Length);
            };
        }

        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}