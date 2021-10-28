using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCMusicStore.Data;
using MVCMusicStore.Models;
using MVCMusicStore.ViewModels;
using System.Linq;
using System.Text.Encodings.Web;

namespace MVCMusicStore.Controllers
{
    public class ShoppingCartController : Controller
    {

        private readonly StoreDbContext storeDbContext;

        public ShoppingCartController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        public IActionResult Index()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext, storeDbContext);
            // Set up our ViewModel
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };

            return View(viewModel);
        }

        public ActionResult AddToCart(int id)
        {
            // Retrieve the album from the database
            var addedAlbum = storeDbContext.Albums
            .Single(album => album.AlbumId == id);
            // Add it to the shopping cart
            var cart = ShoppingCart.GetCart(this.HttpContext, storeDbContext);
            cart.AddToCart(addedAlbum);
            // Go back to the main store page for more shopping
            return RedirectToAction("Index");
        }

        //
        // AJAX: /ShoppingCart/RemoveFromCart/5
        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            // Remove the item from the cart
            var cart = ShoppingCart.GetCart(this.HttpContext, storeDbContext);
            // Get the name of the album to display confirmation
            string albumName = storeDbContext.Carts.Include(e => e.Album)
            .Single(item => item.RecordId == id).Album.Title;
            // Remove from cart
            int itemCount = cart.RemoveFromCart(id);
            // Display the confirmation message
            var results = new ShoppingCartRemoveViewModel
            {
               Message = HtmlEncoder.Default.Encode(albumName) +
            " has been removed from your shopping cart.",
                CartTotal = cart.GetTotal(),
                CartCount = cart.GetCount(),
                ItemCount = itemCount,
                DeleteId = id
            };
            return Json(results);
        }

        [HttpGet]
        public IActionResult CartSummary()
        {
            var cart = ShoppingCart.GetCart(HttpContext, storeDbContext);
            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }
    }
}
