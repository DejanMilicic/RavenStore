using System;
using System.Linq;
using System.Net.Http;
using Bogus;
using FluentAssertions;
using Raven.Client.Documents.Session;
using RavenStore.Models;
using Specs.Infrastructure;
using Xunit;

namespace Specs.Features.Products.CreateProduct
{
    [Trait("Create Product", "Duplicate Name")]
    public class CreateProduct_DuplicateName : Fixture
    {
        private readonly HttpResponseMessage _response;
        private readonly dynamic _newProduct;
        private Product _existingProduct;

        public CreateProduct_DuplicateName()
        {
            _existingProduct = new Faker<Product>()
                .StrictMode(true)
                .RuleFor(p => p.Id, f => Guid.NewGuid().ToString())
                .RuleFor(p => p.Name,  f => "Apple")
                .RuleFor(p => p.Price, f => Double.Parse(f.Commerce.Price()))
                .Generate();

            using (var session = Store.OpenSession())
            {
                session.Store(_existingProduct);
                session.SaveChanges();
            }
            
            _newProduct = new
            {
                Name = "Apple",
                Price = 19.99
            };

            WaitForUserToContinueTheTest(Store);
            
            _response = TestServer.CreateClient().PostAsync("/product", Serialize(_newProduct)).Result;
        }

        [Fact(DisplayName = "1. Status 400 is returned")]
        public void Status400Returned()
        {
            _response.StatusCode.Should().Be(400);
        }

        [Fact(DisplayName = "2. One product is in the database")]
        public void OneProductExists()
        {
            Store.OpenSession().Query<Product>().Statistics(out QueryStatistics stats).ToList();

            stats.TotalResults.Should().Be(1);
        }

        [Fact(DisplayName = "3. Existing product was not modified")]
        public void ExistingProductUnmodified()
        {
            Product product = Store.OpenSession().Query<Product>().Single();

            product.Should().BeEquivalentTo(_existingProduct);
        }
    }
}
