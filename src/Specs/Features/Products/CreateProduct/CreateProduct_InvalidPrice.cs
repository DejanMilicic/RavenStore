using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Raven.Client.Documents.Session;
using RavenStore.Models;
using Specs.Infrastructure;
using Xunit;

namespace Specs.Features.Products.CreateProduct
{
    [Trait("Create Product", "Invalid Price")]
    public class CreateProduct_InvalidPrice : Fixture
    {
        private readonly HttpResponseMessage _response;
        private readonly dynamic _newProduct;

        public CreateProduct_InvalidPrice()
        {
            _newProduct = new
            {
                Name = "Apple",
                Price = -19.99
            };

            _response = TestServer.CreateClient().PostAsync("/product", Serialize(_newProduct)).Result;

            WaitForUserToContinueTheTest(Store);
        }

        [Fact(DisplayName = "1. Status 400 is returned")]
        public void Status400Returned()
        {
            _response.StatusCode.Should().Be(400);
        }

        [Fact(DisplayName = "2. No products is created in the database")]
        public void NoProductsCreated()
        {
            Store.OpenSession().Query<Product>().Statistics(out QueryStatistics stats).ToList();

            stats.TotalResults.Should().Be(0);
        }
    }
}
