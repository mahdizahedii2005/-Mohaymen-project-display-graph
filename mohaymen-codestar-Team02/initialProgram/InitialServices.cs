using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Mapper;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.Administration;
using mohaymen_codestar_Team02.Services.Authenticatoin;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.DataAdminService;
using mohaymen_codestar_Team02.Services.FileReaderService;
using mohaymen_codestar_Team02.Services.PasswordHandller;
using mohaymen_codestar_Team02.Services.ProfileService;
using mohaymen_codestar_Team02.Services.StoreData;
using mohaymen_codestar_Team02.Services.StoreData.Abstraction;
using mohaymen_codestar_Team02.Services.TokenService;

namespace mohaymen_codestar_Team02.initialProgram;

public class InitialServices
{
    private readonly DataContext _context;
    private readonly IPasswordService _passwordService;

    public InitialServices(DataContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public static void ConfigureServices(IServiceCollection services, WebApplicationBuilder builder)
    {
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
        services.AddControllers();
        services.AddHttpContextAccessor();

        // Configure DbContext and Dependency Injection
        var cs = Environment.GetEnvironmentVariable("CONNECTION_STRING");
        services.AddDbContext<DataContext>(options =>
            options.UseNpgsql(cs));

        services
            .AddScoped<IAuthenticationService, AuthenticationService>()
            .AddScoped<IAdminService, AdminService>()
            .AddScoped<IProfileService, ProfileService>()
            .AddScoped<ITokenService, TokenService>()
            .AddScoped<ICookieService, CookieService>()
            .AddScoped<IPasswordService, PasswordService>()
            .AddScoped<InitialServices>()
            .AddSingleton<IEdageStorer, EdgeStorerCsv>()
            .AddSingleton<IVertexStorer, VertexStorerCsv>()
            .AddTransient<IStorHandler, StoreDataService>()
            .AddSingleton<IFileReader, ReadCsvFile>()
            .AddSingleton<IDataAdminService, DataAdminService>();

        services.AddAutoMapper(typeof(AutoMapperProfile));
        services.AddAuthorization();

        ConfigureAuthentication(services);
    }

    public static void ConfigureAuthentication(IServiceCollection services)
    {
        services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.Events = new JwtBearerEvents
                {
                    OnMessageReceived = context =>
                    {
                        var token = context.HttpContext.Request.Cookies["login"];
                        if (!string.IsNullOrEmpty(token)) context.Token = token;

                        return Task.CompletedTask;
                    }
                };

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(
                            "my top secret key is right here hehe jkjkjkj yguguygugyuguyguuygggyuguyuygu    jhj"))
                };
            });
    }

    public void SeadRole()
    {
        List<Role> roles = new()
        {
            new Role
            {
                RoleId = 1,
                RoleType = "SystemAdmin"
            },
            new Role
            {
                RoleId = 2,
                RoleType = "Analyst"
            },
            new Role
            {
                RoleId = 3,
                RoleType = "DataAdmin"
            }
        };


        foreach (var role in roles)
        {
            if (!_context.Roles.Any(r => r.RoleType == role.RoleType))
                _context.Roles.Add(role);

            _context.SaveChanges();
        }
    }

    public void SeadAdmin()
    {
        if (!_context.Users.Any(u => u.Username == "admin"))
        {
            var admin = new User()
            {
                Username = "admin"
            };
            _passwordService.CreatePasswordHash("admin", out var passwordHash, out var passwordSalt);
            admin.PasswordHash = passwordHash;
            admin.Salt = passwordSalt;

            var role = _context.Roles.FirstOrDefault(r =>
                r.RoleType.ToLower().Equals(RoleType.SystemAdmin.ToString().ToLower()));

            var userRole = new UserRole()
                { RoleId = role.RoleId, UserId = admin.UserId, Role = role, User = admin };
            _context.UserRoles.Add(userRole);

            _context.Users.Add(admin);
            _context.SaveChanges();
        }
    }
}