using Microsoft.Extensions.DependencyInjection;
using PinGen.App.ViewModels;
using PinGen.App.Views;
using PinGen.ImageProcessing.Interfaces;
using PinGen.ImageProcessing.Services;
using PinGen.IO.Interfaces;
using PinGen.IO.Services;
using PinGen.Rendering.Interfaces;
using PinGen.Rendering.Services;
using PinGen.Templates.Interfaces;
using PinGen.Templates.Services;
using System;
using System.Windows;

namespace PinGen.App
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var services = new ServiceCollection();
            
            // Register services
            services.AddSingleton<IImageProcessor, ImageSharpProcessor>();
            services.AddSingleton<IImageLoader, ImageLoader>();
            services.AddSingleton<IFileSaver, FileSaver>();
            services.AddSingleton<IFontLoader, FontLoader>();
            services.AddSingleton<ITemplateProvider, HardcodedTemplateProvider>();
            services.AddSingleton<IPinRenderer, PinRenderer>();

            // Register ViewModels
            services.AddTransient<AwakenWindowViewModel>();

            Services = services.BuildServiceProvider();

            var mainWindow = new AwakenWindow();
            mainWindow.DataContext = Services.GetRequiredService<AwakenWindowViewModel>();
            mainWindow.Show();
        }
    }
}
