
using SteamItemSeller.Application;
using SteamItemSeller.Application.Interfaces;
using SteamItemSeller.Services.ApiServices;
using SteamItemSeller.Services.SteamServices;
using SteamItemSeller.Services.SteamServices.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Default Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<HttpRetryHandler>();

// Injection Services

builder.Services.AddScoped<IClientUseCases, ClientUseCases>();
builder.Services.AddScoped<IUserProfile, UserProfile>();
builder.Services.AddScoped<IUserInventory, UserInventory>();

var app = builder.Build();

// Middleware

app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SteamItemSeller API V1");
    c.RoutePrefix = string.Empty;
});
app.UseHttpsRedirection();

app.MapControllers();

app.Run();