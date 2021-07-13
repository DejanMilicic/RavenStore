using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Raven.Client.Documents;
using Raven.Client.Documents.Indexes;
using Raven.TestDriver;
using RavenStore;
using Xunit;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Specs.Infrastructure
{
    public class Fixture : RavenTestDriver, IClassFixture<WebApplicationFactory<Startup>>
    {
        protected readonly IDocumentStore Store;
        protected readonly TestServer TestServer;

        public Fixture()
        {
            Store = this.GetDocumentStore();
            IndexCreation.CreateIndexes(typeof(Startup).Assembly, Store);

            TestServer = SetupHost().GetTestServer();
        }

        public IHost SetupHost()
        {
            return new HostBuilder()
                .ConfigureWebHost(webBuilder =>
                {
                    webBuilder.UseStartup<RavenStore.Startup>();
                    webBuilder
                        .UseTestServer()
                        .ConfigureTestServices(collection =>
                        {
                            collection.AddSingleton<IDocumentStore>(Store);
                        });
                })
                .StartAsync().Result;
        }

        public StringContent Serialize<T>(T obj)
        {
            return new StringContent(
                JsonSerializer.Serialize(obj),
                Encoding.UTF8,
                MediaTypeNames.Application.Json);
        }

        public T Deserialize<T>(HttpResponseMessage response)
        {
            if (response.StatusCode != HttpStatusCode.OK)
                return default(T);

            string content = response.Content.ReadAsStringAsync().Result;
            return JsonConvert.DeserializeObject<T>(content);
        }
    }
}
