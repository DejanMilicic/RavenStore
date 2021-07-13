using System.Linq;
using System.Net.Http;
using FluentAssertions;
using Raven.Client.Documents.Session;
using RavenStore.Models;
using Specs.Infrastructure;
using Xunit;

namespace Specs.Features.Products.GetProduct
{
    [Trait("Get Product", "Happy Path")]
    public class GetProduct_HappyPath : Fixture
    {
        private readonly HttpResponseMessage _response;
        private readonly Product _product;
        private readonly Product _fetchedProduct;

        public GetProduct_HappyPath()
        {
            _product = Generate.Product();

            using (var session = Store.OpenSession())
            {
                session.Advanced.WaitForIndexesAfterSaveChanges();
                session.Store(_product);
                session.SaveChanges();
            }

            _response = TestServer.CreateClient().GetAsync($"/product?name={_product.Name}").Result;
            _fetchedProduct = Deserialize<Product>(_response);

            WaitForUserToContinueTheTest(Store);
        }

        [Fact(DisplayName = "1. Status 200 is returned")]
        public void Status200Returned()
        {
            _response.StatusCode.Should().Be(200);
        }

        [Fact(DisplayName = "2. Fetched product is OK")]
        public void FetchedProductIsOkay()
        {
            _fetchedProduct.Should().BeEquivalentTo(_product);
        }
    }
}
