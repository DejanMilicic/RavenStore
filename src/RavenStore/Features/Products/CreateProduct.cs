using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Antiforgery;
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
                #region Command Validation

                //if (string.IsNullOrWhiteSpace(command.Name))
                //    throw new ValidationException();

                //if (command.Price < 0)
                //    throw new ValidationException();

                #endregion

                using var session = _store.OpenAsyncSession();

                #region Existing Product Validation

                //Product existing = await session.Query<Product>().FirstOrDefaultAsync(x => x.Name == command.Name, token: cancellationToken);
                //if (existing != null)
                //    throw new ValidationException();                

                #endregion

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
