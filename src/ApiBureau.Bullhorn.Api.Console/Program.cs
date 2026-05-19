using ApiBureau.Bullhorn.Api.Console;
using ApiBureau.Bullhorn.Api.Console.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var startup = new Startup();

startup.ConfigureServices(services);

var serviceProvider = services.BuildServiceProvider();

var updateService = serviceProvider.GetService<UpdateFieldService>();
var service = serviceProvider.GetService<PlayGroundService>();


//if (updateService != null)
//    await updateService.UpdateAsync();

if (service != null)
    await service.GetEntitiesAsync();