using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Particle.Framework.ShardCore.Configuration;
using Particle.Framework.ShardCore.Contexts;
using Particle.Framework.ShardCore.Implementations;
using Particle.Framework.ShardCore.Interfaces;
using Particle.Framework.ShardCore.Models;
using ShardCoreTest.Data.EntityFramework;
using ShardCoreTest.Data.Repositories;
using ShardCoreTest.Services;
using ShardCoreTest.SignalRHubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddSignalR();

            // Don't forget to set the migrations assembly
            services.AddDbContext<ShardInformationDbContext>(options =>
               options.UseSqlServer("Server=ServerName\\SQLEXPRESS;Database=SHARDTEST_INFORMATION;Trusted_Connection=True;MultipleActiveResultSets=true",
               b => b.MigrationsAssembly(System.Reflection.Assembly.GetExecutingAssembly().FullName)));

            services.AddDbContext<ShardDbContext>(options =>
               options.UseSqlServer("Server=ServerName\\SQLEXPRESS;Database=SHARDTEST_SHARD1;Trusted_Connection=True;MultipleActiveResultSets=true"),
               ServiceLifetime.Transient);

            services.AddScoped<ShardInformationUnitOfWork>();
            services.AddTransient<ShardUnitOfWork>();
            services.Configure<ShardedRepositoryOptions>(e =>
            {
                e.ShardIdLength = 8;
                e.DataBalancingStrategy = DataBalancingStrategy.Random;
                e.ShardInformationCacheEnabled = true;
                e.ShardInformationCacheDurationInSeconds = 600;
                e.ShardsConnectionTimeoutInSeconds = 600;
                e.SeedShardsIfDontExist = true;
                e.Shards = new List<ShardInformation>()
                {
                    new ShardInformation()
                    {
                        ShardFriendlyName = "Shard1",
                        ShardIdPrefix = "00000001",
                        ConnectionString = "Server=ServerName\\SQLEXPRESS;Database=SHARDTEST_SHARD1;Trusted_Connection=True;MultipleActiveResultSets=true",
                        DatabaseServerName = "ServerName\\SQLEXPRESS",
                        DatabaseName = "SHARDTEST_SHARD1",
                        DatabaseSchema = "dbo"
                    },
                    new ShardInformation()
                    {
                        ShardFriendlyName = "Shard2",
                        ShardIdPrefix = "00000002",
                        ConnectionString = "Server=ServerName\\SQLEXPRESS;Database=SHARDTEST_SHARD2;Trusted_Connection=True;MultipleActiveResultSets=true",
                        DatabaseServerName = "ServerName\\SQLEXPRESS",
                        DatabaseName = "SHARDTEST_SHARD2",
                        DatabaseSchema = "dbo"
                    }//,
                    //new ShardInformation()
                    //{
                    //    ShardFriendlyName = "Shard3",
                    //    ShardIdPrefix = "00000003",
                    //    ConnectionString = "Server=ServerName\\SQLEXPRESS;Database=SHARDTEST_SHARD3;Trusted_Connection=True;MultipleActiveResultSets=true",
                    //    DatabaseServerName = "ServerName\\SQLEXPRESS",
                    //    DatabaseName = "SHARDTEST_SHARD3",
                    //    DatabaseSchema = "dbo"
                    //}
                };
            });

            services.AddTransient<IProductsService, ProductsService>();
            services.AddTransient<IShardInformationRepository, ShardInformationRepository>();
            services.AddTransient<ShardInformationRepository>();
            services.AddTransient<IProductsRepository, ProductsRepository>();
            services.AddTransient<ProductsRepository>();
            services.AddTransient<IProductsShardedRepository, ProductsShardedRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseShardCore();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapHub<ShardsStatsHub>("/shardsStatsHub");
            });
        }
    }
}
