using MartinParkerAngularCV.SharedUtils.Models.Configuration;
using MartinParkerAngularCV.SharedUtils;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MartinParkerAngularCV.Utils;
using MartinParkerAngularCV.SharedUtils.Models.ServiceBus;
using MartinParkerAngularCV.SharedUtils.Enums;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Caching.Distributed;

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
            services.Configure<ServiceBusConfiguration>(Configuration.GetSection("ServiceBus"));

            services.AddMemoryCache();

            services.AddSingleton<KeyVaultHelper>()
                .AddSingleton<BlobStoreHelper>()
                .AddSingleton<TranslationHelper>()
                .AddSingleton<ServiceBusHelper>();

            ServiceProvider provider = services.BuildServiceProvider();

            IDistributedCache distributedCache = provider.GetService<IDistributedCache>();

            provider
                .GetService<ServiceBusHelper>()
                .Subscribe(
                    new ResetTranslationsCacheSubsriptionRequirements(
                        ServiceBusTopic.ResetTranslationsCache, 
                        "CoreAPI", 
                        distributedCache
                    )
                ).Wait();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseRequestLocalization();

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
