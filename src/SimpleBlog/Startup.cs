using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SimpleBlog.Data;
using SimpleBlog.Infrastructures;
using SimpleBlog.Infrastructures.Filters;
using SimpleBlog.Models;
using System;
using System.Linq;
using System.Reflection;

namespace SimpleBlog
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configSection = Configuration.GetSection(nameof(Config));
            services.Configure<Config>(configSection);
            var appConfig = new Config();
            configSection.Bind(appConfig);

            services.AddDbContext<AppDbContext>(options =>
            {
                switch (appConfig.Database.Provider)
                {
                    case Config.DatabaseModel.ProviderEnum.SqlServer:
                        options.UseSqlServer(appConfig.Database.ConnectionStrings);
                        break;
                    case Config.DatabaseModel.ProviderEnum.Sqlite:
                        options.UseSqlite(appConfig.Database.ConnectionStrings);
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(appConfig.Database.Provider));
                }
            });
            services.AddDefaultIdentity<User>()
                .AddDefaultUI(UIFramework.Bootstrap4)
                .AddEntityFrameworkStores<AppDbContext>();

            services.AddRouting(options =>
            {
                options.ConstraintMap["slugify"] = typeof(SlugifyParameterTransformer);
            });

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddMvcOptions(options =>
                {
                    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParameterTransformer()));
                    options.Filters.Add<StatusCodeExceptionFilter>();
                })
                .AddRazorPagesOptions(options =>
                {
                    options.Conventions.Add(new PageRouteTransformerConvention(new SlugifyParameterTransformer()));
                })
                .AddControllersAsServices();

            services.AddMemoryCache();
            services.AddHttpContextAccessor();
            services.AddAutoMapper();

            var appServices = Assembly.GetExecutingAssembly().ExportedTypes
                .Where(t => t.Namespace.EndsWith(".Services") && t.Name.EndsWith("Service"));
            foreach (var appService in appServices)
                services.AddScoped(appService);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseStatusCodePages();

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller:slugify=Post}/{action:slugify=Index}/{id?}");
            });
        }
    }
}
