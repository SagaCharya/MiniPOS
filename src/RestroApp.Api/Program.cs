using Microsoft.EntityFrameworkCore;
using RestroApp.Infrastructure.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();

// Add DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
var pgUrl = builder.Configuration["DATABASE_URL"] ?? Environment.GetEnvironmentVariable("DATABASE_URL");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    if (!string.IsNullOrEmpty(pgUrl))
    {
        options.UseNpgsql(pgUrl);
    }
    else
    {
        options.UseSqlServer(connectionString);
    }
});

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        policy =>
        {
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader();
        });
});

var app = builder.Build();

// AUTOMATIC MIGRATIONS (Production helper)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // PostgreSQL usually needs a small delay or retry logic in container, 
    // but for simple demo, just migrate.
    if (!string.IsNullOrEmpty(pgUrl))
    {
       db.Database.Migrate();
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseDefaultFiles(); // Support index.html automatically
app.UseStaticFiles();
app.UseAuthorization();

app.MapControllers();

// SPA Fallback: Direct all non-API requests to index.html
app.MapFallbackToFile("index.html");

app.Run();
