using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services;
using mohaymen_codestar_Team02.Services.Administration;
using mohaymen_codestar_Team02.Services.CookieService;
using mohaymen_codestar_Team02.Services.ProfileService;
using mohaymen_codestar_Team02.Services.TokenService;
using AuthenticationService = mohaymen_codestar_Team02.Services.Authenticatoin.AuthenticationService;
using IAuthenticationService = mohaymen_codestar_Team02.Services.Authenticatoin.IAuthenticationService;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddControllers();


//var connectionString = "Host=localhost;Port=5432;Database=mohaymen_group02_project;Username=postgres;Password=@Simpleuser01;";
builder.Services.AddHttpContextAccessor();
builder.Services.AddDbContext<DataContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddScoped<IAuthenticationService, AuthenticationService>()
    .AddScoped<IAdminService, AdminService>()
    .AddScoped<IProfileService, ProfileService>()
    .AddScoped<ITokenService, TokenService>()
    .AddScoped<ICookieService, CookieService>();


builder.Services.AddAuthentication(options =>
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
                if (!string.IsNullOrEmpty(token))
                {
                    context.Token = token;
                }

                return Task.CompletedTask;
            }
        };

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    "my top secret key is right here hehe jkjkjkj yguguygugyuguyguuygggyuguyuygu    jhj"))
        };
    });


builder.Services.AddAuthorization();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();


// add migration
/*using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}*/


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();