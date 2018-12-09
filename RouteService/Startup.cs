using System;
using System.Net.Http;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Polly;
using Polly.Extensions.Http;
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

            var myflightsservice = new FlightsServiceClient.Flightsservice(new Uri(flightsServiceUrl));

            services.AddHttpClient<FlightsServiceClient.IFlightsservice, FlightsServiceClient.Flightsservice>()
                        .SetHandlerLifetime(TimeSpan.FromMinutes(2))  //Set lifetime to five minutes
                        .AddPolicyHandler(GetRetryPolicy());

            services.AddSingleton<IAirlineProviderFactory>(p => { return new AirlineProviderFactory(flightServiceTtl); });
            services.AddSingleton<IAirportProviderFactory>( p => { return new AirportProviderFactory(flightServiceTtl);});
            services.AddSingleton<IRouteProviderFactory>(p => { return new RouteProviderFactory(flightServiceTtl); });

            services.AddScoped<FlightsServiceClient.IFlightsservice, FlightsServiceClient.Flightsservice>(p =>
            {
                // create a named/configured HttpClient
                var httpClient = p.GetRequiredService<IHttpClientFactory>()
                    .CreateClient(nameof(FlightsServiceClient.IFlightsservice));
                var baseUri = new Uri(flightsServiceUrl);
                return new FlightsServiceClient.Flightsservice(baseUri, httpClient);
            });

            services.AddScoped<IAirlineProvider>(p =>
            {
                var factory = p.GetRequiredService<IAirlineProviderFactory>();
                var flightService =  p.GetRequiredService<FlightsServiceClient.IFlightsservice>();
                return factory.Get(flightService);
            });

            services.AddScoped<IAirportProvider>(p =>
            {
                var factory = p.GetRequiredService<IAirportProviderFactory>();
                var flightService = p.GetRequiredService<FlightsServiceClient.IFlightsservice>();
                return factory.Get(flightService);
            });

            services.AddScoped<IRouteProvider>(p =>
            {
                var factory = p.GetRequiredService<IRouteProviderFactory>();
                var flightService = p.GetRequiredService<FlightsServiceClient.IFlightsservice>();
                return factory.Get(flightService);
            });

            services
               .AddMvc(options =>
               {
                   options.Filters.Add<OperationCancelledExceptionFilter>();
                   options.Filters.Add(new ThrottleFilter() {Milliseconds = 200 });
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

        static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
        {
            return HttpPolicyExtensions
                .HandleTransientHttpError()
                .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.NotFound)
                .WaitAndRetryAsync(6, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2,
                                                                            retryAttempt)));
        }
    }
}
