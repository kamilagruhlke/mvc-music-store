using Microsoft.AspNetCore.Mvc;
using System.Web;
using MVCMusicStore.Models;
using System.Collections.Generic;

namespace MVCMusicStore.Controllers
{
    public class StoreController : Controller
    {
        public IActionResult Index()
        {
            var genres = new List<Genre>
            {
                new Genre {Name = "Disco" },
                new Genre {Name = "Jazz" },
                new Genre {Name = "Rock" }
            };

            return View(genres);
        }

        public ActionResult Details(int id)
        {
            var album = new Album { Title = $"Album {id}" };

            return View(album);
        }

        public ActionResult Browse(string genre)
        {
            var genreModel = new Genre { Name = $"{genre}" };
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
