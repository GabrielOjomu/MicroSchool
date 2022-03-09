using Catalog.API.Entities;
using Catalog.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Catalog.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CatalogController : ControllerBase
    {
        private readonly IProductRepository _productRepo;
        private readonly ILogger<CatalogController> _logger;

        public CatalogController(ILogger<CatalogController> logger, IProductRepository productRepo)
        {
            _logger = logger;
            _productRepo = productRepo;
        }


        // GET: api/<CatalogController>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<IEnumerable<string>>> GetProducts()
        {
            var products = await _productRepo.GetProducts();

            return Ok(products);
        }

        // GET api/<CatalogController>/asd
        [HttpGet("{id:length(24)}",Name ="GetProduct")]
        [ProducesResponseType((int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> Get(string id)
        {
            var product = await _productRepo.GetProduct(id);

            if(product == null)
            {
                _logger.LogError($"Product with ID: {id} not found");
                return NotFound();
            }

            return Ok(product);
        }

        [ProducesResponseType(typeof(IEnumerable<Product>), (int)HttpStatusCode.OK)]
        // GET api/<CatalogController>/Phones
        [HttpGet("[action]/{category}", Name = "GetProductByCategory")]
        public async Task<ActionResult<IEnumerable<Product>>> GetProductByCategory(string category)
        {
            var products = await _productRepo.GetProductByCategory(category);

            return Ok(products);
        }

        // POST api/<CatalogController>
        [HttpPost("[action]", Name = "CreateProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> Post([FromBody] Product product)
        {
            await _productRepo.CreateProduct(product);

            return CreatedAtRoute("GetProduct", new {id = product.Id}, product);
        }

        // PUT api/<CatalogController>/5
        [HttpPut("{id}", Name = "UpdateProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> Put([FromBody] Product product)
        {
            return Ok(await _productRepo.UpdateProduct(product));
        }

        // DELETE api/<CatalogController>/5
        [HttpDelete("{id:length(24)}", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(Product), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<Product>> Delete(string id)
        {
            return Ok(await _productRepo.DeleteProduct(id));
        }
    }
}
