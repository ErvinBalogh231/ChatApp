using ChatApp.MVVM.View;
using ChatApp.MVVM.ViewModel;
using ChatApp.Net;
using ChatApp.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;

namespace ChatApp
{
    
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();

            services.AddSingleton<LoginViewModel>();
            services.AddSingleton<ChatViewModel>();
            services.AddSingleton<LoginView>(provider => new LoginView
            {
                DataContext = provider.GetRequiredService<LoginViewModel>()
            });
            services.AddSingleton<ChatView>(provider => new ChatView
            {
                DataContext = provider.GetRequiredService<ChatViewModel>()
            });
            services.AddSingleton<Server>();
            services.AddSingleton<LoggedInUserService>();

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var loginView = _serviceProvider.GetRequiredService<LoginView>();
            var chatView = _serviceProvider.GetRequiredService<ChatView>();
            var chatViewModel = _serviceProvider.GetRequiredService<ChatViewModel>();
            var loggedInUserService = _serviceProvider.GetRequiredService<LoggedInUserService>();

            loginView.Show();
            loginView.IsVisibleChanged += (s, ev) =>
                {
                    if (loginView.IsVisible == false && loginView.IsLoaded)
                    {
                        chatViewModel.LoggedInUser = loggedInUserService.CurrentUser; 
                        chatView.Show();
                        loginView.Close();
                    }
                };

            base.OnStartup(e);
        }

    }

}
