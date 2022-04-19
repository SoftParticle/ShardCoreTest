using Particle.Framework.ShardCore.Exceptions;
using Particle.Framework.ShardCore.Implementations;
using Particle.Framework.ShardCore.Interfaces;
using Particle.Framework.ShardCore.Models;
using ShardCoreTest.Data.Repositories;
using ShardCoreTest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShardCoreTest.Services
{
    public class ProductsService : IProductsService
    {
        private static DateTime? startSeedTime;
        private static DateTime? stopSeedTime;
        private static DateTime? lastBlockFinishDateTime;
        private static long lastBlockDurationInSeconds = 0;
        private readonly IProductsShardedRepository productsRepository;
        private readonly IShardInformationRepository shardInformationRepository;

        public delegate Task ProductsChangedHandler(ProductsService sender);
        public static event ProductsChangedHandler OnProductsChanged; 
        
        public ProductsService(IProductsShardedRepository productsRepository, IShardInformationRepository shardInformationRepository)
        {
            this.productsRepository = productsRepository;
            this.shardInformationRepository = shardInformationRepository;
        }

        public void SaveProduct(Product product)
        {
            product.ProductProperties.Add(new ProductProperty() { Description = "AAA" });
            product.ProductProperties.Add(new ProductProperty() { Description = "BBB" });

            if (product.Id == Guid.Empty)
            {
                this.productsRepository.Add(product);
            }
            else
            {
                var existingProduct = this.productsRepository.GetByShardKey(product.Id);
                existingProduct.Model = product.Model;
                existingProduct.Description = product.Description;
                existingProduct.Price = product.Price;

                this.productsRepository.Update(existingProduct);
            }

            this.productsRepository.UnitOfWork.SaveChanges();
        }

        public Product GetProduct(Guid id)
        {
            var product = this.productsRepository.GetByShardKeyWithIncludes(id);

            if(product == null)
            {
                throw new NotFoundException("Product not found");
            }

            return product;
        }

        public void DeleteProduct(Guid id)
        {
            var product = this.productsRepository.GetByShardKey(id);

            if (product == null)
            {
                throw new NotFoundException("Product not found");
            }

            this.productsRepository.Delete(product);
            this.productsRepository.UnitOfWork.SaveChanges();
        }

        public void MoveToShard(string productId, string shardIdPrefix)
        {
            this.productsRepository.MoveToShardWithIncludes(Guid.Parse(productId), shardIdPrefix);
            this.productsRepository.UnitOfWork.SaveChanges();
        }

        public async Task<PagedCollectionResult<Product>> GetAllAsync(int page, int pageSize, string sortColumn, string sortDirection, string searchTerm)
        {
            return await this.productsRepository.GetAllAsync(page, pageSize, sortColumn, sortDirection, searchTerm);
        }

        public PagedCollectionResult<Product> GetAll(int page, int pageSize, string sortColumn, string sortDirection, string searchTerm)
        {
            return this.productsRepository.GetAll(page, pageSize, sortColumn, sortDirection, searchTerm);
        }

        public void SeedProducts()
        {
            if (startSeedTime.HasValue && !stopSeedTime.HasValue)
            {
                return;
            }

            startSeedTime = DateTime.UtcNow;
            OnProductsChanged(this);

            var unitOfWork = this.productsRepository.UnitOfWork as ShardedRepositoryUnitOfWork<Product>;
            unitOfWork.SetContextsDetectChangesEnabled(false);

            var products = new List<Product>();
            lastBlockFinishDateTime = DateTime.UtcNow;

            for(var i = 1; i <= 1000000; i++)
            {
                var product = new Product()
                {
                    Model = "Model " + i,
                    Description = "Description " + i,
                    Price = i,
                    ProductProperties = new List<ProductProperty>()
                    {
                        new ProductProperty()
                        {
                            Description = "Product Property 1 " + i
                        },
                        new ProductProperty()
                        {
                            Description = "Product Property 2 " + i
                        }
                    }
                };

                products.Add(product);
                
                if(i % 50000 == 0)
                {
                    this.productsRepository.AddBulkAsync(products).Wait();
                    products.Clear();
                    var currentBlockFinishDateTime = DateTime.UtcNow;

                    // If there is a subscriber to the event
                    if (OnProductsChanged != null)
                    {
                        if(i == 1000000)
                        {
                            stopSeedTime = DateTime.UtcNow;
                        }
                        lastBlockDurationInSeconds = (long)(currentBlockFinishDateTime - lastBlockFinishDateTime.Value).TotalSeconds;
                        lastBlockFinishDateTime = currentBlockFinishDateTime;
                        OnProductsChanged(this);
                    }
                }
            }

            unitOfWork.ClearChangeTrackers();
            unitOfWork.SetContextsDetectChangesEnabled(true);
            startSeedTime = null;
            stopSeedTime = null;
        }

        public ShardsStats GetProductsStats()
        {
            var shards = this.shardInformationRepository.GetShards();
            var shardsProductsCount = new Dictionary<string, long>();
            var productsTotal = 0;

            foreach(var shard in shards)
            {
                this.productsRepository.SetObjectSet(shard.ShardIdPrefix);
                var productsCount = this.productsRepository.GetObjectSetProductCount();
                shardsProductsCount[shard.ShardFriendlyName] = productsCount;
                productsTotal += productsCount;
            }

            var result = new ShardsStats()
            {
                ShardsProductsCount = shardsProductsCount,
                TotalProducts = productsTotal,
                LastBlockDurationInSeconds = lastBlockDurationInSeconds,
                StartSeedTime = startSeedTime,
                StopSeedTime = stopSeedTime
            };

            return result;
        }
    }
}
