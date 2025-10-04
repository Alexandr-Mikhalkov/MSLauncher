using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MSLauncher.Core.Entities;
using MSLauncher.Core.Interfaces;

namespace MSLauncher.UI.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        private readonly IAuthService _authService;

        [ObservableProperty]
        private string _username;

        [ObservableProperty]
        private string _password;

        [ObservableProperty]
        private string _visiblePassword;

        [ObservableProperty]
        private string _statusText;

        [ObservableProperty]
        private bool _isBusy;

        [ObservableProperty]
        private bool _isPasswordVisible;

        public event Action<User> LoginSuccess;

        public LoginViewModel(IAuthService authService)
        {
            _authService = authService;
        }

        partial void OnIsPasswordVisibleChanged(bool value)
        {
            if (value)
                VisiblePassword = Password;
            else
                Password = VisiblePassword;
        }

        partial void OnVisiblePasswordChanged(string value)
        {
            if (IsPasswordVisible)
                Password = value;
        }

        partial void OnPasswordChanged(string value)
        {
            if (!IsPasswordVisible)
                VisiblePassword = value;
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
        private async Task LoginAsync()
        {
            IsBusy = true;
            StatusText = "Вход...";
            try
            {
                var passwordToUse = IsPasswordVisible ? VisiblePassword : Password;
                var user = new User { Username = Username };
                //var user = await _authService.LoginAsync(Username, passwordToUse);
                if (user != null)
                {
                    StatusText = "Успешный вход!";
                    LoginSuccess?.Invoke(user);
                }
                else
                {
                    StatusText = "Неверное имя пользователя или пароль.";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Ошибка входа: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand(CanExecute = nameof(CanExecuteCommands))]
        private async Task RegisterAsync()
        {
            IsBusy = true;
            StatusText = "Регистрация...";
            try
            {
                var passwordToUse = IsPasswordVisible ? VisiblePassword : Password;
                var success = await _authService.RegisterAsync(Username, passwordToUse);
                if (success)
                {
                    StatusText = "Регистрация успешна! Теперь вы можете войти.";
                }
                else
                {
                    StatusText = "Пользователь с таким именем уже существует.";
                }
            }
            catch (Exception ex)
            {
                StatusText = $"Ошибка регистрации: {ex.Message}";
            }
            finally
            {
                IsBusy = false;
            }
        }

        private bool CanExecuteCommands()
        {
            return true;
            // return !IsBusy && !string.IsNullOrWhiteSpace(Username) && !string.IsNullOrWhiteSpace(Password);
        }
    }
}
