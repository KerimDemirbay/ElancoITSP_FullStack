using Microsoft.EntityFrameworkCore;
using TickSightings.Database;
using TickSightings.Services;

namespace TickSightings
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            builder.Services.AddProblemDetails();
            builder.Services.AddOpenApi();

            var elancoBaseUrl = builder.Configuration["ElancoApi:BaseUrl"]
                ?? throw new InvalidOperationException("ElancoApi:BaseUrl is missing from appsettings.json");

            builder.Services.AddHttpClient<ElancoApiClient>(client =>
            {
                client.BaseAddress = new Uri(elancoBaseUrl);
            })
            .AddStandardResilienceHandler();

            var connectionString = builder.Configuration.GetConnectionString("TickSightings")
                ?? throw new InvalidOperationException("ConnectionStrings:TickSightings is missing from appsettings.json");

            builder.Services.AddDbContext<TickSightingsDbContext>(options =>
                options.UseSqlite(connectionString));

            builder.Services.AddSingleton<RefreshStatus>();
            builder.Services.AddHostedService<SightingRefreshService>();
            builder.Services.AddScoped<SightingQueryService>();
            builder.Services.AddScoped<ReportService>();

            var app = builder.Build();

            // Create the database file and tables on first run.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<TickSightingsDbContext>();
                db.Database.EnsureCreated();
                db.Database.ExecuteSqlRaw("PRAGMA journal_mode=WAL;");
            }

            app.UseExceptionHandler();
            app.UseStatusCodePages();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                app.UseSwaggerUI(o => o.SwaggerEndpoint("/openapi/v1.json", "v1"));
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}