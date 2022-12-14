using ApiBureau.Bullhorn.Api.Console;
using ApiBureau.Bullhorn.Api.Console.Services;
using Microsoft.Extensions.DependencyInjection;

var services = new ServiceCollection();

var startup = new Startup();

startup.ConfigureServices(services);

var serviceProvider = services.BuildServiceProvider();

//var service = serviceProvider.GetService<UpdateFieldService>();
var service = serviceProvider.GetService<PlayGroundService>();

if (service == null) return;

//await service.UpdateAsync();
await service.GetEntitiesAsync();