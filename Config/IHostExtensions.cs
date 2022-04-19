using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Particle.Framework.ShardCore.Contexts;
using Particle.Framework.ShardCore.Interfaces;
using Particle.Framework.ShardCore.Models;
using ShardCoreTest.Data.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.Config
{
    public static class IHostExtensions
    {
        public static IHost MigrateDatabase(this IHost webHost)
        {
            var serviceScopeFactory = (IServiceScopeFactory)webHost.Services.GetService(typeof(IServiceScopeFactory));

            using (var scope = serviceScopeFactory.CreateScope())
            {
                var services = scope.ServiceProvider;
                var shardInformationDbContext = services.GetRequiredService<ShardInformationDbContext>();

                shardInformationDbContext.Database.Migrate();
                
                
                var shardsConfiguration = services.GetRequiredService<IOptions<ShardedRepositoryOptions>>().Value;

                if (shardsConfiguration.SeedShardsIfDontExist)
                {
                    var objectSet = shardInformationDbContext.Set<ShardInformation>();

                    foreach (var shard in shardsConfiguration.Shards)
                    {
                        var existingShard = objectSet.FirstOrDefault(e => e.ShardIdPrefix == shard.ShardIdPrefix);

                        if (existingShard == null)
                        {
                            shard.Enabled = true;
                            shard.ReadEnabled = true;
                            shard.WriteEnabled = true;
                            shard.UpdateEnabled = true;
                            shard.InsertDate = DateTime.UtcNow;
                            shardInformationDbContext.Add(shard);
                        }
                    }

                    shardInformationDbContext.SaveChanges();
                }

                foreach (var shard in shardsConfiguration.Shards)
                {
                    
                    var shardDbContext = new ShardDbContext(shard.ConnectionString);
                    shardDbContext.Database.Migrate();
                }
            }

            return webHost;
        }
    }
}
