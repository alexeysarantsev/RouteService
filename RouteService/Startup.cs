﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace RouteService
{
    public class Startup
    {
        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services
               .AddMvc(options =>
               {
                    //options.Conventions.Add(new CommaSeparatedQueryStringConvention());
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
