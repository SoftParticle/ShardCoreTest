using Particle.Framework.ShardCore.Models;
using ShardCoreTest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.Services
{
    public interface IProductsService
    {
        void SaveProduct(Product product);

        Product GetProduct(Guid id);

        void DeleteProduct(Guid id);

        void MoveToShard(string productid, string shardIdPrefix);

        PagedCollectionResult<Product> GetAll(int page, int pageSize, string sortColumn, string sortDirection, string searchTerm);

        Task<PagedCollectionResult<Product>> GetAllAsync(int page, int pageSize, string sortColumn, string sortDirection, string searchTerm);
        
        void SeedProducts();

        ShardsStats GetProductsStats();
    }
}
