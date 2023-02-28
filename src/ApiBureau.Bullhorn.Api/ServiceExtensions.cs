using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Polly;

namespace ApiBureau.Bullhorn.Api;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds Bullhorn REST API
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <param name="key"></param>
    public static void AddBullhorn(this IServiceCollection services, IConfiguration configuration, string key = "BullhornSettings")
    {
        ArgumentNullException.ThrowIfNull(services);
        ArgumentNullException.ThrowIfNull(configuration);

        var settings = configuration.GetSection(key).Get<BullhornSettings>();

        ArgumentNullException.ThrowIfNull(settings);

        services.AddBullhorn(settings);
    }

    public static void AddBullhorn(this IServiceCollection services, BullhornSettings configureSettings)
    {
        services.TryAddSingleton(Options.Create(configureSettings));
        services.AddHttpClient<BullhornClient>()
            .AddPolicyHandler(Policy.TimeoutAsync<HttpResponseMessage>(20))
            .AddTransientHttpErrorPolicy(policyBuilder => policyBuilder.WaitAndRetryAsync(new[] { TimeSpan.FromSeconds(3) }));
    }
}