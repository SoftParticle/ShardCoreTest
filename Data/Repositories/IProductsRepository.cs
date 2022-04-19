using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Particle.Framework.ShardCore.Interfaces;
using Particle.Framework.ShardCore.Models;
using ShardCoreTest.DomainModels;

namespace ShardCoreTest.Data.Repositories
{
    public interface IProductsRepository : IRepository<Product>
    {
    }
}
