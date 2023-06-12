using DAL;
using Entity;
using Entity.EntityInterfaces;
using Entity.Scanners;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository.Core;
using Repository.Interfaces;
using Utility_LOG;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();
var services = scope.ServiceProvider;
var logManager = services.GetRequiredService<LogManager>();


try
{
    var app = services.GetRequiredService<App>();
    app.Run(args);
}
catch (Exception ex)
{
    //logManager.LogError($"An unexpected error occurred: {ex.Message}", LogProviderType.File);
    //logManager.LogError($"An unexpected error occurred: {ex.Message}", LogProviderType.Console);
}


static IHostBuilder CreateHostBuilder(string[] args)
{

    var config = new ConfigurationBuilder()
        .SetBasePath(Directory.GetCurrentDirectory())
        .AddJsonFile("appsettings.json")
        .Build();

    // var connectionString = GetConnectionString(); 
    var connectionString = config["ConnectionString:Value"];


    return Host.CreateDefaultBuilder(args)
        .ConfigureServices((_, services) =>
        {
            services.AddSingleton<App>();
            services.AddDbContext<KlaContext>(opt => opt.UseSqlServer(connectionString));
            services.AddSingleton<LogManager>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IUniqueIdsRepository, UniqueIdsRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddSingleton<IScanner, AlarmScanner>();
            services.AddSingleton<IScanner, EventScanner>();
            services.AddSingleton<IScanner, VariableScanner>();
            services.AddSingleton<MainManager>();
        });
}

//static string GetConnectionString()
//{
//    var dbHost = Environment.GetEnvironmentVariable("DB_HOST")
//                 ?? throw new ArgumentNullException("DB_HOST environment variable not found");
//    var dbName = Environment.GetEnvironmentVariable("DB_NAME")
//                 ?? throw new ArgumentNullException("DB_NAME environment variable not found");
//    var dbPassword = Environment.GetEnvironmentVariable("DB_SA_PASSWORD")
//                 ?? throw new ArgumentNullException("DB_SA_PASSWORD environment variable not found");


//    return $"Data Source={dbHost};Initial Catalog={dbName};User ID=sa; Password={dbPassword};TrustServerCertificate=true";
//}
