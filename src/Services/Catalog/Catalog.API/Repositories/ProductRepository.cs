using Catalog.API.DTO;
using Catalog.API.Entities;
using MongoDB.Driver;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Catalog.API.Repositories
{
    public interface IProductRepository
    {
        public Task<IEnumerable<Product>> GetProducts();

        public Task<Product> GetProduct(string id);

        public Task<IEnumerable<Product>> GetProductByName(string name);

        public Task<IEnumerable<Product>> GetProductByCategory(string category);

        public Task CreateProduct(Product product);

        public Task<bool> UpdateProduct(Product product);

        public Task<bool> DeleteProduct(string id);
    }

    public class ProductRepository : IProductRepository
    {
        private readonly ICatalogContext _context;

        public ProductRepository(ICatalogContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Product>> GetProducts()
        {
            return await _context.Products
                .Find(p => true)
                .ToListAsync();
        }

        public async Task<Product> GetProduct(string id)
        {
            return await _context.Products
                .Find(p => p.Id == id)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByName(string name)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(p => p.Name, name);

            return await _context.Products
                .Find(filter)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductByCategory(string category)
        {
            //FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(p => p.Category, category);

            return await _context.Products
                .Find(p => p.Category == category)
                .ToListAsync();
        }

        public async Task CreateProduct(Product product)
        {
            await _context.Products.InsertOneAsync(product);
        }

        public async Task<bool> UpdateProduct(Product product)
        {
            var result = await _context
                        .Products
                        .ReplaceOneAsync(filter: a => a.Id == product.Id, replacement: product);

            return result.IsAcknowledged && result.ModifiedCount > 0;
        }

        public async Task<bool> DeleteProduct(string id)
        {
            FilterDefinition<Product> filter = Builders<Product>.Filter.ElemMatch(p => p.Id, id);

            var result = await _context
                        .Products
                        .DeleteOneAsync(filter);

            return result.IsAcknowledged && result.DeletedCount > 0;
        }
    }
}
