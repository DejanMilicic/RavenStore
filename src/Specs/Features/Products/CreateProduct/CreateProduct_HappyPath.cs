using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Raven.Client.Documents.Session;
using RavenStore.Models;
using Specs.Infrastructure;
using Xunit;

namespace Specs.Features.Products.CreateProduct
{
    [Trait("Create Product", "Happy Path")]
    public class CreateProduct_HappyPath : Fixture
    {
        private readonly HttpResponseMessage _response;
        private readonly dynamic _newProduct;

        public CreateProduct_HappyPath()
        {
            _newProduct = new
            {
                Name = "Apple",
                Price = 19.99
            };

            _response = TestServer.CreateClient().PostAsync("/product", Serialize(_newProduct)).Result;

            WaitForUserToContinueTheTest(Store);
        }

        [Fact(DisplayName = "1. Status 200 is returned")]
        public void Status200Returned()
        {
            _response.StatusCode.Should().Be(200);
        }

        [Fact(DisplayName = "2. One product is created in the database")]
        public void OneProductCreated()
        {
            Store.OpenSession().Query<Product>().Statistics(out QueryStatistics stats).ToList();

            stats.TotalResults.Should().Be(1);
        }

        [Fact(DisplayName = "3. New product has expected content")]
        public void ExpectedContent()
        {
            var product = Store.OpenSession().Query<Product>().Single();

            product.Name.Should().Be(_newProduct.Name);
            product.Price.Should().Be(_newProduct.Price);
        }
    }
}
