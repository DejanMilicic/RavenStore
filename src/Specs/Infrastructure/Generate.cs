using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bogus;
using RavenStore.Models;

namespace Specs.Infrastructure
{
    public static class Generate
    {
        public static Product Product()
        {
            return new Faker<Product>()
                .StrictMode(true)
                .RuleFor(p => p.Id, f => Guid.NewGuid().ToString())
                .RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => Double.Parse(f.Commerce.Price()))
                .Generate();
        }
    }
}
