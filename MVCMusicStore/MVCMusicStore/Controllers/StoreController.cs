using Microsoft.AspNetCore.Mvc;
using System.Web;
using MVCMusicStore.Models;
using System.Collections.Generic;
using MVCMusicStore.Data;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MVCMusicStore.Controllers
{
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
            var album = storeDbContext.Albums.Find(id);

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
