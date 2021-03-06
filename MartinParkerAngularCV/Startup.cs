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
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Http;

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
            services.Configure<KeyVaultConfiguration>(Configuration.GetSection("KeyVault"));
            services.Configure<BlobStoreConfiguration>(Configuration.GetSection("BlobStore"));
            services.Configure<ServiceBusConfiguration>(Configuration.GetSection("ServiceBus"));

            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            services.AddSingleton<KeyVaultHelper>()
                .AddSingleton<BlobStoreHelper>()
                .AddSingleton<TranslationHelper>()
                .AddSingleton<ServiceBusHelper>();

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            services.AddControllersWithViews();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ServiceBusHelper serviceBusHelper, IDistributedCache distributedCache)
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
                app.UseSpaStaticFiles();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseAuthorization();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            serviceBusHelper
                .Subscribe(
                    new ResetTranslationsCacheSubsriptionRequirements(
                        ServiceBusTopic.ResetTranslationsCache,
                        "CoreAPI",
                        distributedCache
                    )
                ).Wait();
        }
    }
}
