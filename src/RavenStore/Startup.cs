using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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
            services.AddControllers();

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

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
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
