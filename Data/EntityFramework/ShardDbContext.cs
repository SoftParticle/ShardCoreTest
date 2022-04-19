using Microsoft.EntityFrameworkCore;
using ShardCoreTest.Data.Mappings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.Data.EntityFramework
{
    public class ShardDbContext : DbContext
    {
        private readonly string connectionString;


        // This constructor is required to prevent an exception in bulk operations.
        public ShardDbContext() : base()
        {
        }

        public ShardDbContext(string connectionString) : base()
        {
            this.connectionString = connectionString;
        }

        public ShardDbContext(DbContextOptions<ShardDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // Register Mappings
            ProductMap.RegisterMapping(builder);
            ProductPropertyMap.RegisterMapping(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
