using ToP.Application.Interfaces;
using ToP.Application.Services;
using ToP.Infrastructure.Data;
using ToP.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Configuration: Toggle between in-memory or database storage
// Set "UseDatabase": true in appsettings.json or appsettings.Development.json
bool useDatabase = builder.Configuration.GetValue<bool>("UseDatabase", false);

if (useDatabase)
{
    Console.WriteLine("? Using DATABASE storage mode");

    // Database configuration
    var connectionString = builder.Configuration.GetConnectionString("TournamentDb")
        ?? "Data Source=tournament.db";

    builder.Services.AddDbContext<TournamentDbContext>(options =>
        options.UseSqlite(connectionString));

    builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();

    // RoundRobinService is singleton for caching
    builder.Services.AddSingleton<IRoundRobinService, RoundRobinService>();

    // PlayerServiceDb uses database with repository
    builder.Services.AddScoped<IPlayerService>(serviceProvider =>
    {
        var repository = serviceProvider.GetRequiredService<IPlayerRepository>();
        var roundRobinService = serviceProvider.GetRequiredService<IRoundRobinService>();
        return new PlayerServiceDb(repository, roundRobinService);
    });
}
else
{
    Console.WriteLine("? Using IN-MEMORY storage mode");

    // In-memory configuration (original implementation)
    builder.Services.AddSingleton<IRoundRobinService, RoundRobinService>();
    builder.Services.AddSingleton<IPlayerService>(serviceProvider =>
    {
        var roundRobinService = serviceProvider.GetRequiredService<IRoundRobinService>();
        return new PlayerService(roundRobinService);
    });
}

// Add CORS for frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Initialize database if using database mode
if (useDatabase)
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<TournamentDbContext>();

        // Creates database and seeds initial data
        dbContext.Database.EnsureCreated();

        Console.WriteLine("? Database initialized with seed data");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


// Ensure routing and CORS are configured before HTTPS redirection and
// authorization so CORS headers are present on redirect and endpoint responses.
app.UseRouting();
app.UseCors("AllowFrontend");

// In development we avoid forcing HTTPS redirects because the dev server
// (and the CRA proxy) talk to the HTTP endpoint. Redirect responses can
// cause the browser to see a cross-origin redirect (http -> https) and
// trigger CORS failures. In production we keep the redirection.
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();

app.MapControllers();

app.Run();
