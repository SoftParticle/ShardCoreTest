using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Particle.Framework.CommonExtensions;
using ShardCoreTest.Models;
using ShardCoreTest.Services;
using ShardCoreTest.SignalRHubs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ShardCoreTest.Controllers
{
    public class HomeController : Controller
    {
        private bool disposed;
        private readonly IProductsService productsService;
        private readonly IHubContext<ShardsStatsHub> hubContext;

        public HomeController(IProductsService productsService, IHubContext<ShardsStatsHub> context)
        {
            this.productsService = productsService;
            this.hubContext = context;
            ProductsService.OnProductsChanged += OnProductsChangedHandler;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult New()
        {
            var viewModel = new Product();
            return View("Save", viewModel);
        }

        public IActionResult Save(Product viewModel)
        {
            if (ModelState.IsValid)
            {
                this.productsService.SaveProduct(viewModel);
                return RedirectToAction("Index");
            }
            else 
            {
                return View(viewModel);
            }
        }

        public IActionResult Edit(Guid id)
        {
            var product = this.productsService.GetProduct(id);

            var viewModel = new Product()
            {
                Id = product.Id,
                Model = product.Model,
                Description = product.Description,
                Price = product.Price,
                ProductProperties = product.ProductProperties
            };

            return View("Save", viewModel);
        }

        public bool Delete(Guid id)
        {
            try
            {
                this.productsService.DeleteProduct(id);

                return true;
            }
            catch
            {
                return false;
            }
        }

        [HttpPost]
        public IActionResult LoadData()
        {
            try
            {
                var dataTableParameters = Request.GetDataTableParameters();

                var productsPagedCollection = this.productsService.GetAll(
                dataTableParameters.page,
                dataTableParameters.pageSize,
                dataTableParameters.sortColumn,
                dataTableParameters.sortDirection,
                dataTableParameters.searchTerm
                );

                //Returning Json Data  
                return Json(new { draw = dataTableParameters.draw, recordsFiltered = productsPagedCollection.Total, recordsTotal = productsPagedCollection.Total, data = productsPagedCollection.Data });
            }
            catch (Exception e)
            {
                throw;
            }
        }

        [HttpPost]
        public IActionResult MoveToShard(string productid, string shardIdPrefix)
        {
            this.productsService.MoveToShard(productid, shardIdPrefix);
            return RedirectToAction("Index");
        }

        [HttpPost]
        public void SeedProducts()
        {
            this.productsService.SeedProducts();
        }

        [HttpGet]
        public IActionResult Shards()
        {
            return View();
        }

        [HttpGet]
        public ShardsStatsViewModel ShardsStats()
        {
            var shardsStats = this.productsService.GetProductsStats();
            var viewModel = new ShardsStatsViewModel()
            {
                ShardsProductsCount = shardsStats.ShardsProductsCount,
                TotalProducts = shardsStats.TotalProducts,
                StartSeedTime = shardsStats.StartSeedTime,
                StopSeedTime = shardsStats.StopSeedTime
            };

            return viewModel;
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        async Task OnProductsChangedHandler(ProductsService sender)
        {
            var shardsStats = sender.GetProductsStats();
            var viewModel = new ShardsStatsViewModel()
            {
                ShardsProductsCount = shardsStats.ShardsProductsCount,
                TotalProducts = shardsStats.TotalProducts,
                LastBlockDurationInSeconds = shardsStats.LastBlockDurationInSeconds,
                StartSeedTime = shardsStats.StartSeedTime,
                StopSeedTime = shardsStats.StopSeedTime
            };

            await this.hubContext.Clients.All.SendAsync("UpdateShardsStats", viewModel);
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                {
                    //dispose managed resources
                    ProductsService.OnProductsChanged -= OnProductsChangedHandler;
                }
            }
            //dispose unmanaged resources
            disposed = true;
        }

        public new void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}

