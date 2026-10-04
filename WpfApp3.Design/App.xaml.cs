using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using ProjectName.Services;
using ProjectName.Wpf.Services;
using ProjectName.Wpf.ViewModels;
using ProjectName.Wpf.Views;

namespace ProjectName.Wpf;

public partial class App : Application
{
    public static IServiceProvider Services { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        var sc = new ServiceCollection();
        ConfigureServices(sc);
        Services = sc.BuildServiceProvider();

        Services.GetRequiredService<MainWindow>().Show();
    }

    private static void ConfigureServices(IServiceCollection services)
    {
        // Services
        services.AddSingleton<IExternalCatalogService, ExternalCatalogService>();
        services.AddSingleton<ICalculationContext, CalculationContext>();
        services.AddSingleton<ICalculationService, CalculationService>();
        services.AddSingleton<IDraftStorage, LocalJsonDraftStorage>();
        services.AddSingleton<IDialogService, DialogService>();

        // ViewModels
        services.AddSingleton<MainViewModel>();

        // Views
        services.AddSingleton<MainWindow>();
    }
}