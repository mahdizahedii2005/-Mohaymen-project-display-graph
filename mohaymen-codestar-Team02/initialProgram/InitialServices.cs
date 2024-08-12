using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services.Administration;
using mohaymen_codestar_Team02.Services.Authenticatoin;
using mohaymen_codestar_Team02.Services.ProfileService;

namespace mohaymen_codestar_Team02.initialProgram;

public class InitialServices
{
    public static void Init(WebApplicationBuilder builder)
    {
        
        // var connectionString = Environment.GetEnvironmentVariable("connectionStringForDb");
        var connectionString = "Host=localhost;Database=mohaymen_group02_project;Username=postgres;Password=1274542332Mz;";
        builder.Services.AddHttpContextAccessor();
        builder.Services.AddDbContext<DataContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString(connectionString)))
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IAdminService, AdminService>()
            .AddScoped<IProfileService, ProfileService>();
        Console.WriteLine(connectionString);
    }
}