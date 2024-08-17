using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Models;
using mohaymen_codestar_Team02.Services.Administration;
using mohaymen_codestar_Team02.Services.Authenticatoin;
using mohaymen_codestar_Team02.Services.PasswordHandller;

namespace mohaymen_codestar_Team02.initialProgram;

public class InitialServices
{
    /*
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
    }*/
    private readonly DataContext _context;
    private readonly IPasswordService _passwordService;

    public InitialServices(DataContext context, IPasswordService passwordService)
    {
        _context = context;
        _passwordService = passwordService;
    }

    public void SeadRole()
    {
        List<Role> roles = new List<Role>()
        {
            new Role()
            {
                RoleId = 1,
                RoleType = "SystemAdmin"
            },
            new Role()
            {
                RoleId = 2,
                RoleType = "Analyst"
            },
            new Role()
            {
                RoleId = 3,
                RoleType = "DataAdmin"
            }
        };
        
        
        foreach (var role in roles)
        {
            
            if (!_context.Roles.Any(r => r.RoleType == role.RoleType))
            {
                _context.Roles.Add(role);
            }
        }

        _context.SaveChanges();
    }
    public async void SeadAdmin()
    {
        if (!_context.Users.Any(u => u.Username == "admin"))
        {
            var admin = new User()
            {
                Username = "admin",
            };
            _passwordService.CreatePasswordHash("admin", out byte[] passwordHash, out byte[] passwordSalt);
            admin.PasswordHash = passwordHash;
            admin.Salt = passwordSalt;

            var role = _context.Roles.FirstOrDefault(r => r.RoleType.ToLower().Equals(RoleType.SystemAdmin.ToString().ToLower()));
            
            UserRole userRole = new UserRole() { RoleId = role.RoleId, UserId = admin.UserId, Role = role, User = admin };
            _context.UserRoles.Add(userRole);
            
            _context.Users.Add(admin);
            _context.SaveChanges();
        }
    }
    
    
}