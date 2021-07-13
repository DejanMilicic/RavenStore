using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Hellang.Middleware.ProblemDetails;
using MediatR;
using Microsoft.Extensions.Configuration;
using Raven.Client;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using RavenStore.Infrastructure;

namespace RavenStore
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
            services
                .AddProblemDetails(ConfigureProblemDetails)
                .AddControllers();

            services.AddMediatR(typeof(Startup));

            services.AddSingleton<IDocumentStore>(ctx =>
            {
                var dbConfig = Configuration.GetSection("Database").Get<Settings.DatabaseSettings>();

                var store = new DocumentStore
                {
                    Urls = dbConfig.Urls,
                    Database = dbConfig.DatabaseName
                };

                store.Initialize();

                IndexCreation.CreateIndexes(typeof(Startup).Assembly, store);

                return store;
            });
        }

        private void ConfigureProblemDetails(ProblemDetailsOptions options)
        {
            options.Rethrow<NotSupportedException>();

            options.MapToStatusCode<NotImplementedException>(StatusCodes.Status501NotImplemented);
            options.MapToStatusCode<ValidationException>(StatusCodes.Status400BadRequest);
            options.MapToStatusCode<KeyNotFoundException>(StatusCodes.Status404NotFound);

            options.MapToStatusCode<HttpRequestException>(StatusCodes.Status503ServiceUnavailable);

            // Because exceptions are handled polymorphically, this will act as a "catch all" mapping, which is why it's added last.
            // If an exception other than NotImplementedException and HttpRequestException is thrown, this will handle it.
            options.MapToStatusCode<Exception>(StatusCodes.Status500InternalServerError);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseProblemDetails();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
