using ToP.Application.Interfaces;
using ToP.Application.Services;
using ToP.Infrastructure.Data;
using ToP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

// Force backend to use launchSettings http port
builder.WebHost.UseUrls("http://localhost:5036");

// Enable controllers with camelCase JSON
builder.Services.AddControllers().AddJsonOptions(o =>
{
    o.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

bool useDatabase = builder.Configuration.GetValue<bool>("UseDatabase", false);

if (useDatabase)
{
    Console.WriteLine("? Using DATABASE storage mode");
    var connectionString = builder.Configuration.GetConnectionString("TournamentDb")
        ?? "Data Source=tournament.db";
    builder.Services.AddDbContext<TournamentDbContext>(options =>
        options.UseSqlite(connectionString));
    builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
    builder.Services.AddSingleton<IRoundRobinService, RoundRobinService>();
    builder.Services.AddScoped<IPlayerService>(serviceProvider =>
    {
        var repository = serviceProvider.GetRequiredService<IPlayerRepository>();
        var roundRobinService = serviceProvider.GetRequiredService<IRoundRobinService>();
        return new PlayerServiceDb(repository, roundRobinService);
    });
    builder.Services.AddScoped<ITournamentService>(serviceProvider =>
    {
        var roundRobinService = serviceProvider.GetRequiredService<IRoundRobinService>();
        var playerService = serviceProvider.GetRequiredService<IPlayerService>();
        return new TournamentService(roundRobinService, playerService);
    });
}
else
{
    Console.WriteLine("? Using IN-MEMORY storage mode");
    builder.Services.AddSingleton<IRoundRobinService, RoundRobinService>();
    builder.Services.AddSingleton<IPlayerService>(serviceProvider =>
    {
        var roundRobinService = serviceProvider.GetRequiredService<IRoundRobinService>();
        return new PlayerService(roundRobinService);
    });
    builder.Services.AddSingleton<ITournamentService>(serviceProvider =>
    {
        var roundRobinService = serviceProvider.GetRequiredService<IRoundRobinService>();
        var playerService = serviceProvider.GetRequiredService<IPlayerService>();
        return new TournamentService(roundRobinService, playerService);
    });
}

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins(
            "http://localhost:3000",
            "http://localhost:5173",
            "http://localhost:7263"
        )
        .AllowAnyHeader()
        .AllowAnyMethod();
    });
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (useDatabase)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TournamentDbContext>();
        dbContext.Database.EnsureCreated();
        Console.WriteLine("? Database initialized with seed data");
    }
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseCors("AllowFrontend");
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}
app.UseAuthorization();
app.MapControllers();
app.Run();
