using Particle.Framework.ShardCore.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.DomainModels
{
    public class Product : ShardedEntity
    {
        public Product()
        {
            this.ProductProperties = new List<ProductProperty>();
        }

        public string Model { get; set; }
        
        public string Description { get; set; }

        public float Price { get; set; }

        public List<ProductProperty> ProductProperties { get; set; }
    }
}
