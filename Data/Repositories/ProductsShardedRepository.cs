using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.Extensions.Options;
using Particle.Framework.ShardCore.Implementations;
using Particle.Framework.ShardCore.Interfaces;
using Particle.Framework.ShardCore.Models;
using ShardCoreTest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Dynamic.Core;

namespace ShardCoreTest.Data.Repositories
{
    public class ProductsShardedRepository : GenericShardedRepository<Product, ProductsRepository>, IProductsShardedRepository
    {
        public ProductsShardedRepository(ShardInformationRepository shardInformationRepository, IServiceProvider serviceProvider, IOptions<ShardedRepositoryOptions> options) : base(shardInformationRepository, serviceProvider, options)
        {
        }

        public Product GetByShardKeyWithIncludes(Guid id)
        {
            var include = this.GetIncludes();
            return this.GetByShardKey(id, include);
        }

        public void MoveToShardWithIncludes(Guid id, string shardIdPrefix)
        {
            var include = this.GetIncludes();
            this.MoveToShard(id, shardIdPrefix, include);
        }

        public PagedCollectionResult<Product> GetAll(int page, int pageSize, string sortColumn, string sortDirection, string searchTerm)
        {
            return base.GetAll(page, pageSize, sortColumn, sortDirection, fullTextSearchTerm: searchTerm);
        }

        public async Task<PagedCollectionResult<Product>> GetAllAsync(int page, int pageSize, string sortColumn, string sortDirection, string searchTerm)
        {
            return await this.GetAllAsync(page, pageSize, sortColumn, sortDirection, searchTerm);
        }

        public int GetObjectSetProductCount()
        {
            return this.ObjectSet.AsQueryable().AsNoTracking().Count();
        }

        private Func<IQueryable<Product>, IIncludableQueryable<Product, object>> GetIncludes()
        {
            Func<IQueryable<Product>, IIncludableQueryable<Product, object>> include = entity =>
                entity.Include(e => e.ProductProperties);

            return include;
        }
    }
}
