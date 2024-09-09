using mohaymen_codestar_Team02.initialProgram;
using mohaymen_codestar_Team02.Middlewares;

var builder = WebApplication.CreateBuilder(args);
builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200", "https://codestar-g2.abriment.com")
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
});

InitialServices.ConfigureServices(builder.Services, builder);
var app = builder.Build();


InitialApp.ConfigureApp(app);

app.UseCors("AllowSpecificOrigins");
app.UseMiddleware<SanitizationMiddleware>();
app.Run();