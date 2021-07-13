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
    public class GetProduct
    {
        public class Query : IRequest<Product>
        {
            public string Name { get; set; }
        }

        internal class Handler : IRequestHandler<Query, Product>
        {
            private readonly IDocumentStore _store;

            public Handler(IDocumentStore store)
            {
                _store = store;
            }

            public async Task<Product> Handle(Query query, CancellationToken cancellationToken)
            {
                #region Query Validation

                //if (string.IsNullOrWhiteSpace(query.Name))
                //    throw new ValidationException();                

                #endregion

                using var session = _store.OpenAsyncSession();

                Product product = await session.Query<Product>()
                    .FirstOrDefaultAsync(x => x.Name == query.Name, token: cancellationToken);

                #region Product Existence Validation

                //if (product == null)
                //    throw new KeyNotFoundException();                

                #endregion

                return product;
            }
        }
    }
}
