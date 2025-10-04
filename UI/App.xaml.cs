using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MSLauncher.Core.Data;
using MSLauncher.Core.Entities;
using MSLauncher.Core.Interfaces;
using MSLauncher.Core.Services;
using MSLauncher.UI.Services;
using MSLauncher.UI.View;
using MSLauncher.UI.ViewModels;
using System;
using System.Windows;

namespace UI
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            string connectionString = "Host=localhost;Port=5432;Database=mslauncher_db;Username=sash2411;Password=24012006";

            services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

            services.AddSingleton<IGameLauncher, GameLauncher>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddSingleton<ISettingsService, SettingsService>();

            services.AddTransient<LoginViewModel>();
            services.AddTransient<MainViewModel>();

            services.AddTransient<LoginWindow>();
            services.AddTransient<MainWindow>();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var dbContext = ServiceProvider.GetRequiredService<AppDbContext>();
            dbContext.Database.Migrate();

            ShowLoginWindow();
        }

        private void ShowLoginWindow()
        {
            var loginWindow = ServiceProvider.GetRequiredService<LoginWindow>();
            var loginViewModel = ServiceProvider.GetRequiredService<LoginViewModel>();

            loginViewModel.LoginSuccess += (user) =>
            {
                ShowMainWindow(user);
                loginWindow.Close();
            };

            loginWindow.DataContext = loginViewModel;
            loginWindow.Show();
        }


        private void ShowMainWindow(User user)
        {
            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            var mainViewModel = ServiceProvider.GetRequiredService<MainViewModel>();

            mainViewModel.LogoutRequested += () =>
            {
                ShowLoginWindow();
                mainWindow.Close();
            };

            mainViewModel.Username = user.Username;
            mainWindow.DataContext = mainViewModel;
            mainWindow.Show();
        }
    }
}