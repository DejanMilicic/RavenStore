using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using RavenStore.Features.Products;
using RavenStore.Models;

namespace RavenStore.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("product")]
        public async Task<Product> Get([FromQuery] GetProduct.Query query) => await _mediator.Send(query);

        [HttpPost("product")]
        public async Task Post([FromBody] CreateProduct.Command command) => await _mediator.Send(command);
    }
}
