using System;
using System.Windows;
using Codeworx.Identity.Wpf.Test.Common;
using Codeworx.Identity.Wpf.Test.ViewModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Wpf.Test
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();

            var builder = new ConfigurationBuilder();
            builder.AddJsonFile("appsettings.json");

            Configuration = builder.Build();

            var services = new ServiceCollection();
            services.AddHttpClient();
            services.AddSingleton<IDialogHandler>(new WpfDialogHandler());
            services.Configure<LoginOptions>(Configuration.GetSection("Login"));
            services.AddScoped<LoginViewModel>();

            ServiceProvider = services.BuildServiceProvider();

            this.MainWindow = new MainWindow();
            this.MainWindow.Loaded += MainWindow_Loaded;
            this.MainWindow.Show();
        }

        public IConfigurationRoot Configuration { get; }

        public ServiceProvider ServiceProvider { get; }

        public async void LoginAsync(string tenant = null)
        {
            using (var scope = ServiceProvider.CreateScope())
            {
                var viewModel = scope.ServiceProvider.GetRequiredService<LoginViewModel>();

                this.MainWindow.Content = viewModel;
                try
                {
                    ISessionInfo loginResult;

                    loginResult = await viewModel.LoginAsync(tenant);

                    this.MainWindow.Content = new HelloViewModel(loginResult);
                }
                catch (Exception ex)
                {
                    this.MainWindow.Content = $"Error: {ex}";
                }
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            this.MainWindow.Loaded -= MainWindow_Loaded;
            this.LoginAsync();
        }
    }
}
