using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MVCMusicStore.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MVCMusicStore.Models
{
    public class ShoppingCart
    {
        private readonly StoreDbContext storeDbContext;

        public ShoppingCart(StoreDbContext storeDbContext)
        {
            this.storeDbContext = storeDbContext;
        }

        string ShoppingCartId { get; set; }

        public const string CartSessionKey = "CartId";

        public static ShoppingCart GetCart(HttpContext context, StoreDbContext storeDbContext)
        {
            var cart = new ShoppingCart(storeDbContext);
            cart.ShoppingCartId = cart.GetCartId(context);
            return cart;
        }

        // Helper method to simplify shopping cart calls
        public static ShoppingCart GetCart(Controller controller, StoreDbContext storeDbContex)
        {
            return GetCart(controller.HttpContext, storeDbContex);
        }

        public void AddToCart(Album album)
        {
            // Get the matching cart and album instances
            var cartItem = storeDbContext.Carts.SingleOrDefault(
           c => c.CartId == ShoppingCartId
           && c.AlbumId == album.AlbumId);
            if (cartItem == null)
            {
                // Create a new cart item if no cart item exists
                cartItem = new Cart
                {
                    AlbumId = album.AlbumId,
                    CartId = ShoppingCartId,
                    Count = 1,
                    DateCreated = DateTime.Now
                };
                storeDbContext.Carts.Add(cartItem);
            }
            else
            {
                // If the item does exist in the cart, then add one to the quantity
                cartItem.Count++;
            }
            // Save changes
            storeDbContext.SaveChanges();
        }

        public int RemoveFromCart(int id)
        {
            // Get the cart
            var cartItem = storeDbContext.Carts.Single(
           cart => cart.CartId == ShoppingCartId
           && cart.RecordId == id);

            int itemCount = 0;
            if (cartItem != null)
            {
                if (cartItem.Count > 1)
                {
                    cartItem.Count--;
                    itemCount = cartItem.Count;
                }
                else
                {
                    storeDbContext.Carts.Remove(cartItem);
                }
                // Save changes
                storeDbContext.SaveChanges();
            }
            return itemCount;
        }

        public void EmptyCart()
        {
            var cartItems = storeDbContext.Carts.Where(cart => cart.CartId == ShoppingCartId);
            foreach (var cartItem in cartItems)
            {
                storeDbContext.Carts.Remove(cartItem);
            }
            // Save changes
            storeDbContext.SaveChanges();
        }
        public List<Cart> GetCartItems()
        {
            return storeDbContext.Carts
                .Include(e => e.Album)
                    .ThenInclude(e => e.Artist)
                .Include(e => e.Album)
                    .ThenInclude(e => e.Genre)
                .Where(cart => cart.CartId == ShoppingCartId).ToList();
        }
        public int GetCount()
        {
            // Get the count of each item in the cart and sum them up
            int? count = (from cartItems in storeDbContext.Carts
                          where cartItems.CartId == ShoppingCartId
                          select (int?)cartItems.Count).Sum();
            // Return 0 if all entries are null
            return count ?? 0;
        }
        public decimal GetTotal()
        {
            // Multiply album price by count of that album to get
            // the current price for each of those albums in the cart
            // sum all album price totals to get the cart total

            decimal? total = (from cartItems in storeDbContext.Carts
                              where cartItems.CartId == ShoppingCartId
                              select (int?)cartItems.Count * cartItems.Album.Price).Sum();
            return total ?? decimal.Zero;
        }

        public int CreateOrder(Order order)
        {
            decimal orderTotal = 0;
            var cartItems = GetCartItems();
            // Iterate over the items in the cart, adding the order details for each
            foreach (var item in cartItems)
            {
                var orderDetail = new OrderDetail
                {
                    AlbumId = item.AlbumId,
                    OrderId = order.OrderId,
                    UnitPrice = item.Album.Price,
                    Quantity = item.Count
                };
                // Set the order total of the shopping cart
                orderTotal += (item.Count * item.Album.Price);
                storeDbContext.OrderDetails.Add(orderDetail);
            }
            // Set the order's total to the orderTotal count
            order.Total = orderTotal;
            // Save the order
            storeDbContext.SaveChanges();
            // Empty the shopping cart
            EmptyCart();
            // Return the OrderId as the confirmation number
            return order.OrderId;
        }

        // We're using HttpContextBase to allow access to cookies.
        public string GetCartId(HttpContext context)
        {
            if (context.Session.GetString(CartSessionKey) == null)
            {
                if (!string.IsNullOrWhiteSpace(context.User.Identity.Name))
                {
                    context.Session.SetString(CartSessionKey, context.User.Identity.Name);
                }
                else
                {
                    // Generate a new random GUID using System.Guid class
                    Guid tempCartId = Guid.NewGuid();
                    // Send tempCartId back to client as a cookie
                    context.Session.SetString(CartSessionKey, tempCartId.ToString());
                }
            }

            return context.Session.GetString(CartSessionKey);
        }

        // When a user has logged in, migrate their shopping cart to
        // be associated with their username
        public void MigrateCart(string userName)
        {
            var shoppingCart = storeDbContext.Carts.Where(c => c.CartId == ShoppingCartId);
            foreach (Cart item in shoppingCart)
            {
                item.CartId = userName;
            }
            storeDbContext.SaveChanges();
        }
    }
}
