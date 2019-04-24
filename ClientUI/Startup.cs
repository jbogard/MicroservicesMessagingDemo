using System;
using AutoMapper;
using ClientUI.Data;
using ClientUI.Hubs;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NServiceBus;
using Sales.Messages;
using Endpoint = NServiceBus.Endpoint;

namespace ClientUI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }
        private IHubContext<SubmissionNotificationHub> _hub { get; set; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddEntityFrameworkSqlServer();
            services.AddDbContext<StoreDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));


            services.AddLogging(loggingBuilder => loggingBuilder.AddConsole());

            services.AddAutoMapper();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            services.AddSignalR(hubOptions => { hubOptions.EnableDetailedErrors = true; });


            services.AddSingleton(new Lazy<IHubContext<SubmissionNotificationHub>>(() => _hub));

            services.AddNServiceBus("ClientUI");
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

            app.UseSignalR(routes =>
            {
                routes.MapHub<SubmissionNotificationHub>("/submissionHub");
            });

            _hub = app.ApplicationServices.GetRequiredService<IHubContext<SubmissionNotificationHub>>();

        }
    }

    public static class NServiceBusExtension
    {
        public static void AddNServiceBus(this IServiceCollection services, string endpointName)
        {
            var endpointConfiguration = new EndpointConfiguration(endpointName);
            endpointConfiguration.EnableInstallers();

            var transport = endpointConfiguration.UseTransport<RabbitMQTransport>().ConnectionString("host=localhost");
            transport.UseConventionalRoutingTopology();

            endpointConfiguration.UseContainer<ServicesBuilder>(
                customizations: customizations => { customizations.ExistingServices(services); });

            var routing = transport.Routing();
            DoCustomRouting(routing);

            var endpoint =  Endpoint.Start(endpointConfiguration).GetAwaiter().GetResult();
            services.AddSingleton(sp => endpoint);
        }

        private static void DoCustomRouting(RoutingSettings<RabbitMQTransport> routing)
        {
            routing.RouteToEndpoint(typeof(PlaceOrder), "Sales");
        }
    }
}
