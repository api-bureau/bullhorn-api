using ApiBureau.Bullhorn.Api.Console.Services;
using Microsoft.Extensions.DependencyInjection;

namespace ApiBureau.Bullhorn.Api.Console
{
    public static class Program
    {
        public static async Task Main()
        {
            var services = new ServiceCollection();

            var startup = new Startup();

            startup.ConfigureServices(services);

            var serviceProvider = services.BuildServiceProvider();

            var service = serviceProvider.GetService<UpdateFieldService>();

            if (service == null) return;

            await service.UpdateAsync();
        }
    }
}
