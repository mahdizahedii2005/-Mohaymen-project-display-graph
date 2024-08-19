using DotNetEnv;
using mohaymen_codestar_Team02.initialProgram;

var builder = WebApplication.CreateBuilder(args);
ConfigureEnvironment();
InitialServices.ConfigureServices(builder.Services, builder);
var app = builder.Build();


InitialApp.ConfigureApp(app);

app.Run();

void ConfigureEnvironment()
{
    Env.Load();
}