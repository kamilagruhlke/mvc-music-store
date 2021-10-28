using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MVCMusicStore.Data;
using MVCMusicStore.Models;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace MVCMusicStore.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly StoreDbContext storeDbContext;

        public HomeController(StoreDbContext storeDbContext, ILogger<HomeController> logger)
        {
            this.storeDbContext = storeDbContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            // Get most popular albums
            var albums = GetTopSellingAlbums(5);
            return View(albums);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private List<Album> GetTopSellingAlbums(int count)
        {
            // Group the order details by album and return
            // the albums with the highest count
            var getTopAlbums = storeDbContext.Albums
                .OrderByDescending(a => a.OrderDetails.Count())
                .Take(count)
                .ToList();

            return getTopAlbums;
        }
    }
}
