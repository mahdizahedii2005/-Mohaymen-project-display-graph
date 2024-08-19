using DotNetEnv;
using mohaymen_codestar_Team02.initialProgram;

var builder = WebApplication.CreateBuilder(args);
ConfigureEnvironment();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigins",
        builder =>
        {
            builder.WithOrigins("http://localhost:4200") 
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials();
        });
});

InitialServices.ConfigureServices(builder.Services, builder);
var app = builder.Build();


InitialApp.ConfigureApp(app);

app.UseCors("AllowSpecificOrigins");

app.Run();

void ConfigureEnvironment()
{
    Env.Load();
}