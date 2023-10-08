#define __WINDOWS__
//#define __LINUX__

using Microsoft.EntityFrameworkCore;
using ScriptScheduler.Core.CsScript;
using ScriptScheduler.Core.PythonScript;
using ScriptScheduler.Entity;
using Serilog;

IHost host = Host.CreateDefaultBuilder(args)
#if __WINDOWS__
    .UseWindowsService()
#endif
#if __LINUX__
    .UseSystemd()
#endif
    .UseSerilog((context, provider, config) =>
    {
        config.Enrich.FromLogContext()
            .Enrich.WithMachineName()
            .WriteTo.Debug()
            .WriteTo.Console()
            .ReadFrom.Configuration(context.Configuration);
    })
    .ConfigureServices((hostContext, services) =>
    {
        var connectionString = hostContext.Configuration.GetConnectionString("sqlite");
        services.AddDbContextPool<AppDbContext>(options => options.UseSqlite(connectionString));

        #region [csscript]

        services.Configure<CsScriptOption>(hostContext.Configuration.GetSection(nameof(CsScriptOption)));
        services.AddSingleton<CsScriptExecutor>();
        services.AddSingleton<CsScriptSetup>();
        services.AddHostedService<CsScriptWorker>();

        #endregion
        
        #region [python]

        services.Configure<PythonScriptOption>(hostContext.Configuration.GetSection(nameof(PythonScriptOption)));
        services.AddSingleton<PythonExecutor>();
        services.AddSingleton<PythonSetup>();
        services.AddHostedService<PythonScriptWorker>();        

        #endregion
    })
    .ConfigureAppConfiguration((context, builder) =>
    {
#if DEBUG
        builder.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true);
#else
        builder.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
#endif
        builder.AddEnvironmentVariables();
    })
    .Build();

// Database
var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>()
    .UseSqlite(host.Services.GetService<IConfiguration>().GetConnectionString("sqlite"));
var context = new AppDbContext(optionsBuilder.Options);
context.Database.EnsureCreated();

host.Run();

Log.CloseAndFlush();