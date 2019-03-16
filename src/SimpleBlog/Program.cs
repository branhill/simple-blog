using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlog.Data;
using SimpleBlog.Models;

namespace SimpleBlog
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var config = services.GetRequiredService<IConfiguration>();
                if (config.GetValue("Development:IsDataSeedingEnabled", false))
                {
                    var userManager = services.GetRequiredService<UserManager<User>>();
                    var dbContext = services.GetRequiredService<AppDbContext>();
                    DataSeeding.Seed(dbContext, userManager);
                }
            }

            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
