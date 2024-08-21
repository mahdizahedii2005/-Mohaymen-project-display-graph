using Microsoft.EntityFrameworkCore;
using mohaymen_codestar_Team02.Data;

namespace mohaymen_codestar_Team02.initialProgram;

public class InitialApp
{
    public static void ConfigureApp(WebApplication app)
    {
        // // Apply pending migrations (uncomment if needed)
        // using (var scope = app.Services.CreateScope())
        // {
        //     var services = scope.ServiceProvider;
        //
        //     var context = services.GetRequiredService<DataContext>();
        //     context.Database.EnsureCreated();
        //
        //     if (context.Database.GetPendingMigrations().Any())
        //     {
        //         context.Database.Migrate();
        //     }
        // }

        // Initialize services
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            var initialServices = services.GetRequiredService<InitialServices>();
            initialServices.SeadRole();
            initialServices.SeadAdmin();
        }

        // Configure middleware
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
    }
}