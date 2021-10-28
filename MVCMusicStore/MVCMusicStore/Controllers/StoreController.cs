using Microsoft.AspNetCore.Mvc;
using System.Web;
using MVCMusicStore.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace MVCMusicStore.Controllers
{
    [Authorize]
    public class StoreController : Controller
    {
        private readonly StoreDbContext storeDbContext;

        public StoreController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        public IActionResult Index()
        {
            var genres = storeDbContext.Genres.ToList();

            return View(genres);
        }

        public IActionResult Details(int id)
        {
            var album = storeDbContext.Albums
                .Include(e => e.Genre)
                .ThenInclude(e => e.Albums)
                .ThenInclude(e => e.Artist)
                .FirstOrDefault(e => e.AlbumId == id);

            if (album is null)
            {
                return NotFound();
            }

            return View(album);
        }

        public IActionResult Browse(string genre)
        {
            var genreModel = storeDbContext.Genres.Include("Albums")
                .Single(g => g.Name == genre);

            return View(genreModel);
        }

        public string Browser(string genre)
        {
            string message = HttpUtility.HtmlEncode($"Store.Genre, Genre = {genre}");
            return message;
        }

        public string Detail(int id)
        {
            string message = $"Store.Detail, Id = {id}";
            return message;
        }
    }
}
