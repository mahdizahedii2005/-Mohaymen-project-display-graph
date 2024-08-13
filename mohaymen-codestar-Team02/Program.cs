using Microsoft.AspNetCore.Authentication;
using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;
using mohaymen_codestar_Team02.Services.Administration;
using mohaymen_codestar_Team02.Services.ProfileService;
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
    .AddScoped<IProfileService, ProfileService>();

var app = builder.Build();


using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var context = services.GetRequiredService<DataContext>();
    if (context.Database.GetPendingMigrations().Any())
    {
        context.Database.Migrate();
    }
}


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

app.Run();