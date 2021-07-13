using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Raven.Client.Documents;
using RavenStore.Models;

namespace RavenStore.Features.Products
{
    public class CreateProduct
    {
        public class Command : IRequest
        {
            public string Name { get; set; }

            public double Price { get; set; }
        }

        internal class Handler : AsyncRequestHandler<Command>
        {
            private readonly IDocumentStore _store;

            public Handler(IDocumentStore store)
            {
                _store = store;
            }

            protected override async Task Handle(Command command, CancellationToken cancellationToken)
            {
                using var session = _store.OpenAsyncSession();

                Product product = new Product
                {
                    Name = command.Name,
                    Price = command.Price
                };

                await session.StoreAsync(product, cancellationToken);
                await session.SaveChangesAsync(cancellationToken);
            }
        }
    }
}
