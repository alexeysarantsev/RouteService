using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using RouteService.Api.Filters;
using RouteService.FlightsServiceProvider;
using RouteService.Model.Interfaces;
using Swashbuckle.AspNetCore.Swagger;

namespace RouteService
{
    public class Startup
    {
        private const string flightsServiceUrlKey = "FlightsServiceUrl";
        private const string flightServiceCasheLifetimeKey = "FlightServiceCasheLifetime";
        

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        private IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            string flightsServiceUrl = Configuration[flightsServiceUrlKey];
            int flightServiceCasheLifetime = int.Parse(Configuration[flightServiceCasheLifetimeKey]);
            TimeSpan flightServiceTtl = TimeSpan.FromMinutes(flightServiceCasheLifetime);

            var flightsservice = new FlightsServiceClient.Flightsservice(new Uri(flightsServiceUrl));

            var airlineProvider = new AirlineProvider(flightsservice);
            var routeProvider = new RouteProvider(flightsservice);
            var airportProvider = new AirportProvider(flightsservice);

            var airlineProviderCached = new AirlineProviderCached(flightServiceTtl, airlineProvider);
            var airportProviderCached = new AirportProviderCached(flightServiceTtl, airportProvider);
            var routeProviderCached = new RouteProviderCached(flightServiceTtl, routeProvider);

            services.AddSingleton<IAirlineProvider>(airlineProviderCached);
            services.AddSingleton<IRouteProvider>(routeProviderCached);
            services.AddSingleton<IAirportProvider>(airportProviderCached);

            services
               .AddMvc(options =>
               {
                   options.Filters.Add<OperationCancelledExceptionFilter>();
               })
               .AddJsonOptions(options =>
               {
                   options.SerializerSettings.Formatting = Formatting.Indented;
               })
               .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "Route API",
                    TermsOfService = "None",
                    Contact = new Contact
                    {
                        Name = "Alexey Sarantsev",
                        Email = "alexey.sarantsev@gmail.com"
                    }
                });

                // set the comments path for the Swagger JSON and UI
                foreach (string xmlDocument in System.IO.Directory.EnumerateFiles(AppContext.BaseDirectory, "*.xml"))
                {
                    options.IncludeXmlComments(xmlDocument, includeControllerXmlComments: true);
                }
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Route Service API v1");
                options.RoutePrefix = string.Empty;
            });

            app.UseCors(_ => _.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin().AllowCredentials());
            app.UseMvc();
        }
    }
}
