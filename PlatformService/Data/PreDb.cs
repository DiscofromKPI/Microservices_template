using Microsoft.EntityFrameworkCore;
using PlatformService.Models;

namespace PlatformService.Data;

public static class PreDb
{
    public static void PrepPopulation(IApplicationBuilder app, bool isProd)
    {
        using var serviceScope = app.ApplicationServices.CreateScope();
        SeedData(serviceScope.ServiceProvider.GetService<AppDbContext>(), isProd);
    }

    private static void SeedData(AppDbContext context, bool isProd)
    {
        if (isProd)
        {
            Console.WriteLine("Applying migrations");
            try
            {
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error with applying migration cause " + ex.Message);
            }
        }
        if (!context.Platforms.Any())
        {
            Console.WriteLine("Seeding data");

            context.Platforms.AddRange(
                new Platform() {Name = "Dotnet", Publisher = "Microsoft", Cost = "Free"},
                new Platform() {Name = "SqlServer", Publisher = "Microsoft", Cost = "Free"},
                new Platform() {Name = "Kubernetes", Publisher = "Cloud Native Computing Foundation", Cost = "Free"});
            context.SaveChanges();
        }
        else
        {
            Console.WriteLine("We already seeded");
        }
    }
}