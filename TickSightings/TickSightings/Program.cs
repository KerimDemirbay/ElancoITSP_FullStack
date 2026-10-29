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
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();

            var elancoBaseUrl = builder.Configuration["ElancoApi:BaseUrl"]
            ?? throw new InvalidOperationException("ElancoApi:BaseUrl is either missing from appsettings.json or is wrong");

            builder.Services.AddHttpClient<ElancoApiClient>(client =>
            {
                client.BaseAddress = new Uri(elancoBaseUrl);
            })
            .AddStandardResilienceHandler();
            builder.Services.AddSingleton<SightingStore>();
            builder.Services.AddHostedService<SightingRefreshService>();
            var app = builder.Build();

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
