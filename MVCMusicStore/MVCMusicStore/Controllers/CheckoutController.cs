using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MVCMusicStore.Data;
using MVCMusicStore.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MVCMusicStore.Controllers
{
    [Authorize]
    public class CheckoutController : Controller
    {
        private readonly StoreDbContext storeDbContext;

        public CheckoutController(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        const string PromoCode = "FREE";

        //
        // GET: /Checkout/AddressAndPayment
        public IActionResult AddressAndPayment()
        {
            return View();
        }

        //
        // POST: /Checkout/AddressAndPayment
        [HttpPost]
        public async Task<IActionResult> AddressAndPayment([FromForm] IFormCollection values)
        {
            var order = new Order();
            await TryUpdateModelAsync(order);

            try
            {
                if (string.Equals(values["PromoCode"], PromoCode,
                StringComparison.OrdinalIgnoreCase) == false)
                {
                    return View(order);
                }
                else
                {
                    order.Username = User.Identity.Name;
                    order.OrderDate = DateTime.Now;
                    //Save Order
                    storeDbContext.Orders.Add(order);
                    storeDbContext.SaveChanges();
                    //Process the order
                    var cart = ShoppingCart.GetCart(this.HttpContext, storeDbContext);
                    cart.CreateOrder(order);
                    return RedirectToAction("Complete",
                    new { id = order.OrderId });
                }
            }
            catch
            {
                //Invalid - redisplay with errors
                return View(order);
            }
        }

        //
        // GET: /Checkout/Complete
        public IActionResult Complete(int id)
        {
            // Validate customer owns this order
            bool isValid = storeDbContext.Orders.Any(
            o => o.OrderId == id &&
            o.Username == User.Identity.Name);
            if (isValid)
            {
                return View(id);
            }
            else
            {
                return View("Error");
            }
        }
    }
}
