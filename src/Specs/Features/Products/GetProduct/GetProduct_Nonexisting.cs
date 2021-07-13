using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Raven.Client.Documents.Session;
using RavenStore.Models;
using Specs.Infrastructure;
using Xunit;

namespace Specs.Features.Products.GetProduct
{
    [Trait("Get Product", "Nonexisting")]
    public class GetProduct_Nonexisting : Fixture
    {
        private readonly HttpResponseMessage _response;
        private readonly Product _product;
        private readonly Product _fetchedProduct;

        public GetProduct_Nonexisting()
        {
            _product = Generate.Product();

            using (var session = Store.OpenSession())
            {
                session.Advanced.WaitForIndexesAfterSaveChanges();
                session.Store(_product);
                session.SaveChanges();
            }

            _response = TestServer.CreateClient().GetAsync($"/product?name={_product.Name}xxx").Result;
            _fetchedProduct = Deserialize<Product>(_response);

            WaitForUserToContinueTheTest(Store);
        }

        [Fact(DisplayName = "1. Status 404 is returned")]
        public void Status404Returned()
        {
            _response.StatusCode.Should().Be(404);
        }

        [Fact(DisplayName = "2. No products returned")]
        public void NoProductReturned()
        {
            _fetchedProduct.Should().BeNull();
        }
    }
}
