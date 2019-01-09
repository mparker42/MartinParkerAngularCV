using MartinParkerAngularCV.SharedUtilities.Models.Configuration;
using MartinParkerAngularCV.SharedUtilities;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MartinParkerAngularCV
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);

            services.Configure<KeyVaultConfiguration>(Configuration.GetSection("KeyVault"));
            services.Configure<BlobStoreConfiguration>(Configuration.GetSection("BlobStore"));

            services.AddMemoryCache();

            services.AddSingleton<KeyVaultHelper>()
                .AddSingleton<BlobStoreHelper>();

            ServiceProvider provider = services.BuildServiceProvider();
            KeyVaultHelper keyVaultHelper = provider.GetService<KeyVaultHelper>();

            services.AddDistributedRedisCache(options =>
            {
                options.Configuration = keyVaultHelper.GetSecret(Configuration.GetValue<string>("RedisSecretURL")).Result;
                options.InstanceName = "Core";
            });

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });
        }
    }
}
