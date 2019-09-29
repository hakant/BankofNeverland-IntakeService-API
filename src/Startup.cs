using System;
using AutoMapper;
using FluentValidation.AspNetCore;
using IntakeApi.Configuration;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents;
using Microsoft.Azure.Documents.Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace IntakeApi
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
            services.AddCors(options =>
                {
                    // This is a demo app so it's ok
                    options.AddPolicy("AllowAll",
                        builder => builder
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                        );
                });

            services.AddMvc()
                .AddFluentValidation(fv =>
                {
                    fv.ImplicitlyValidateChildProperties = true;
                    fv.RegisterValidatorsFromAssemblyContaining<Startup>();
                })
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.Configure<CosmosDbConfig>(Configuration.GetSection("CosmosDb"));

            var cosmosDbConfig = new CosmosDbConfig();
            Configuration.GetSection("CosmosDb").Bind(cosmosDbConfig);

            services.AddSingleton<IDocumentClient>(x =>
                new DocumentClient(
                    new Uri(cosmosDbConfig.ServiceEndPoint),
                    cosmosDbConfig.PrimaryKey
                    )
            );

            services.AddAutoMapper(typeof(Startup));
            services.AddMediatR(typeof(Startup));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseHsts();
            app.UseCors("AllowAll");
            app.UseMvc();
        }
    }
}
