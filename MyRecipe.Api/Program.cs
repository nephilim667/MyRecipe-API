using MyRecipe.Api.Extensions;
using MyRecipe.Infrastructure.DataBase.Contexts;
using MyRecipe.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Custom Swagger services
builder.Services.AddOpenApiDocumentation();

// Custom CQRS Services
builder.Services.AddMediatRService();

// Custom infrastructure services
builder.Services.AddInfrastructureServices(builder.Configuration);

// Custom authentication services
builder.Services.AddJwtAuthentication(builder.Configuration);

// Logging services
builder.Logging.ClearProviders();
builder.Logging.AddConsole();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApiDocumentation();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.Services.SeedDatabaseAsync();
await app.RunAsync();
