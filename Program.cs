using PixelMart.API;

var builder = WebApplication.CreateBuilder(args);

// Adding services to the container and configure request/response pipeline
var app = builder
       .ConfigureServices()
       .ConfigurePipeline();

await app.RunAsync();
