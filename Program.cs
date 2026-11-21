using PixelMart.API;

var builder = WebApplication.CreateBuilder(args);

var app = await builder
       .ConfigureServices()
       .ConfigurePipelineAsync();

await app.RunAsync();
