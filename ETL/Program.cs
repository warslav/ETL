using ETL.DTO;
using ETL.Service.Implementations;
using ETL.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

class Program
{
    private static ETLSettings _etlSettings = new ETLSettings();
    private static IConfiguration _configuration;
    static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

        _configuration = builder.Build();

        ConfigurationBinder.Bind(_configuration.GetSection("ETLSettings"), _etlSettings);

        var serviceCollection = new ServiceCollection();
        var etlAppSettingsConfig = _configuration.GetSection("ETLSettings");
        serviceCollection.Configure<ETLSettings>(etlAppSettingsConfig);
        //serviceCollection.AddSingleton<ETLSettings>();
        serviceCollection.AddSingleton<MetaFiles>();
        serviceCollection.AddSingleton<FileService>();
        serviceCollection.AddTransient<IETLService, ETLService>();
        serviceCollection.AddTransient<TransformService>();
        serviceCollection.AddSingleton<IUI, UI>();
        //serviceCollection.AddLogging(configure => configure.AddConsole());
        var serviceProvider = serviceCollection.BuildServiceProvider();
        var ui = ActivatorUtilities.CreateInstance<UI>(serviceProvider);
        //IUI ui = new UI();
        await ui.Menu();
    }
}