using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace RavenStore.Controllers
{
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet("product")]
        public async Task<string> Get() => "Here is your product";
    }
}
