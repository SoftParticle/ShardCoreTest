using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.DomainModels
{
    public class ShardsStats
    {
        public ShardsStats()
        {
            this.ShardsProductsCount = new Dictionary<string, long>();
        }

        public long TotalProducts { get; set; }

        public long LastBlockDurationInSeconds { get; set; }

        public DateTime? StartSeedTime { get; set; }

        public DateTime? StopSeedTime { get; set; }

        public Dictionary<string, long> ShardsProductsCount { get; set; }
    }
}
