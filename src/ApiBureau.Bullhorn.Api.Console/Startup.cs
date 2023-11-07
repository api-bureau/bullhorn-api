using ApiBureau.Bullhorn.Api.Console.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;

namespace ApiBureau.Bullhorn.Api.Console;

public class Startup
{
    private IConfiguration Configuration { get; }

    public Startup()
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json")
            .AddJsonFile($"appsettings.{GetEnvironmentVariable()}.json", true, true)
            .AddEnvironmentVariables();

        if (IsDevelopment()) builder.AddUserSecrets(typeof(Program).Assembly);

        Configuration = builder.Build();

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(Configuration)
            .Enrich.FromLogContext()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .CreateLogger();

        AppDomain.CurrentDomain.ProcessExit += (s, e) => Log.CloseAndFlush();

        static bool IsDevelopment()
        {
            var devEnvironmentVariable = GetEnvironmentVariable();

            return string.IsNullOrEmpty(devEnvironmentVariable) || string.Equals(devEnvironmentVariable, "development", StringComparison.OrdinalIgnoreCase);
        }

        static string? GetEnvironmentVariable() => Environment.GetEnvironmentVariable("DOTNET_ENVIRONMENT");
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddSingleton(Configuration);

        services.AddLogging(builder => builder
            .AddSerilog(dispose: true)
        );

        services.AddBullhorn(Configuration, "BullhornSettings:RestApi");

        services.AddScoped<PlayGroundService>();
        services.AddScoped<UpdateFieldService>();
    }
}