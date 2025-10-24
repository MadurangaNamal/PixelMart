using PixelMart.API;

var builder = WebApplication.CreateBuilder(args);

// Adding services to the container and configure request/response pipeline
var app = await builder
       .ConfigureServices()
       .ConfigurePipelineAsync();

await app.RunAsync();
