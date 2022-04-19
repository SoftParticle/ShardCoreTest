using Particle.Framework.ShardCore.Implementations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.Data.EntityFramework
{
    public class ShardUnitOfWork : UnitOfWork
    {
        public ShardUnitOfWork(ShardDbContext context) : base(context)
        {
        }
    }
}
