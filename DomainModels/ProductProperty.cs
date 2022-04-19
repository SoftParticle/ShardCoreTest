using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.DomainModels
{
    public class ProductProperty
    {
        public long Id { get; set; }

        public string Description { get; set; }

        public Guid ProductId { get; set; }
        
        public Product Product { get; set; }
    }
}
