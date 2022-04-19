using Microsoft.EntityFrameworkCore;
using Particle.Framework.ShardCore.Implementations;
using Particle.Framework.ShardCore.Models;
using ShardCoreTest.Data.EntityFramework;
using ShardCoreTest.DomainModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace ShardCoreTest.Data.Repositories
{
    public class ProductsRepository : GenericRepository<Product>, IProductsRepository
    {
        public ProductsRepository(ShardUnitOfWork unitOfWork)
            : base(unitOfWork)
        {
        }
    }
}
