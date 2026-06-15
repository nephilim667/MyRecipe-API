using MyRecipe.Api.Extensions;
using MyRecipe.Infrastructure.DataBase.Contexts;
using MyRecipe.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiDocumentation();
builder.Services.AddMediatRService();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseOpenApiDocumentation();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

await app.Services.SeedDatabaseAsync();
await app.RunAsync();