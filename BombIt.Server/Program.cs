using BombIt.Server.Game.Facade;
using BombIt.Server.Game.Services;
using BombIt.Server.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddSignalR();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowClient", policy =>
    {
        policy.WithOrigins("http://localhost", "https://localhost")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddSingleton<GameFacade>(); // Facade Pattern
builder.Services.AddHostedService<GameLoopService>();

var app = builder.Build();
// Thread-safety demonstraction for Singleton pattern
BombIt.Server.Game.Services.SingletonThreadSafetyBenchmark.RunBenchmark();

// Configure the HTTP request pipeline.
app.UseHttpsRedirection();

app.UseCors("AllowClient");

app.MapHub<GameHub>("/gamehub");

app.Run();