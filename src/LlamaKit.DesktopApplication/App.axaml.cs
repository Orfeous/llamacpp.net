using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using LlamaKit.Configuration;
using LlamaKit.DependencyInjection;
using LlamaKit.DesktopApplication.ViewModels;
using LlamaKit.DesktopApplication.Views;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Serilog;

namespace LlamaKit.DesktopApplication;

public partial class App : Application
{
    private readonly IConfigurationRoot configuration;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // Line below is needed to remove Avalonia data validation.
        // Without this line you will get duplicate validations from both Avalonia and CT

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainViewModel()
            };
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = new MainViewModel()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    public App()
    {
        this.configuration = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddUserSecrets<App>(true)
            .Build();


        Services = ConfigureServices(configuration);
    }

    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    /// <param name="configuration"></param>
    public static IServiceProvider ConfigureServices(IConfiguration configuration)
    {
        var services = new ServiceCollection();
        services.AddSerilog();


        services.AddLogging();


        // add file provider 

        var c = new LlamaKitOptions();
        configuration.GetSection(LlamaKitOptions.LlamaKit).Bind(c);


        services.AddSingleton<IFileProvider>(provider => new PhysicalFileProvider(c.ModelDirectory));

        // add llama 
        services.AddLlama(configuration);

        return services.BuildServiceProvider();
    }
}