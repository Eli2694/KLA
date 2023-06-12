using DAL;
using Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Repository;
using System;
using System.IO;

using IHost host = CreateHostBuilder(args).Build();
using var scope = host.Services.CreateScope();

var services = scope.ServiceProvider;

try
{
    services.GetRequiredService<App>().Run(args);
}
catch (ArgumentException ex)
{
    //ask user for valid input path
    //add to log
}
catch (IndexOutOfRangeException ex)
{
    //act 
    //log
}
catch (InvalidOperationException ioe)
{
    Console.WriteLine("A required service could not be resolved:");
    Console.WriteLine(ioe);
    //add logs
}
catch (Exception ex)
{
    Console.WriteLine("An unexpected error occurred:");
    Console.WriteLine(ex);
    //add logs
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
            services.AddDbContext<DbAccess>(opt => opt.UseSqlServer(connectionString));
            services.AddTransient<IRepositoryUsers, UserRepository>();
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
