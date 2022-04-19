
using Particle.Framework.ShardCore.Interfaces;
using Particle.Framework.ShardCore.Models;
using ShardCoreTest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.Data.Repositories
{
    public interface IProductsShardedRepository : IShardedRepository<Product>
    {
        Product GetByShardKeyWithIncludes(Guid id);

        void MoveToShardWithIncludes(Guid id, string shardIdPrefix);

        PagedCollectionResult<Product> GetAll(int page, int pageSize, string sortColumn, string sortDirection, string searchTerm);
        
        Task<PagedCollectionResult<Product>> GetAllAsync(int page, int pageSize, string sortColumn, string sortDirection, string searchTerm);

        int GetObjectSetProductCount();
    }
}
