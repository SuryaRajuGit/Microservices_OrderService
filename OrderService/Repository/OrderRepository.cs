using Order_Service.Contracts;
using Order_Service.Entities.Dtos;
using Order_Service.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace Order_Service.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly OrderContext _orderContext;

        public OrderRepository(OrderContext orderContext )
        {
            _orderContext = orderContext;
        }
        public void AddProduct(Cart cart)
        {
            
            Cart cartItem = _orderContext.Cart.Include(term => term.BillNo).Include(term => term.Product).Where(find => find.UserId == cart.UserId).FirstOrDefault();
            
            if (cartItem == null)
            {
                _orderContext.Cart.Add(cart);
            }
            else
            {
                Guid CartId = cart.Product.Select(item => item.ProductId).First();
                bool isProductExists = cartItem.Product.Any(find => find.ProductId == CartId);
                if (isProductExists)
                {
                    foreach (var item in cartItem.Product)
                    {
                        if (item.ProductId == cart.Product.Select(item => item.ProductId).First())
                        {
                            item.Quantity = item.Quantity + cart.Product.Select(src => src.Quantity).First();
                            _orderContext.Cart.Update(cartItem);
                            
                            break;
                        }
                    }
                }
                else
                {
                    cartItem.Product = new List<Product>();
                    Product product = new Product
                    {
                        Id = Guid.NewGuid(),
                        Quantity=cart.Product.Select(item => item.Quantity).First(),
                        ProductId = cart.Product.Select(item => item.Id).First(),
                        CartId=cart.Id
                    };
                    cartItem.Product.Add(product);
                    _orderContext.Product.Add(product);
                }
                _orderContext.SaveChanges();
            }
            
        }

        public bool IsProductInCart(Guid id, Guid userId)
        {
            var li = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == userId).FirstOrDefault();
            bool isthere = li.Product.Any(term => term.ProductId == id);
            if (isthere)
            {
                foreach (var term in li.Product)
                {
                    if (term.ProductId == id)
                    {
                        var p = _orderContext.Product.First(find => find.ProductId == id);
                        _orderContext.Product.Remove(p);
                        _orderContext.SaveChanges();
                        return true;
                    }
                }
         
            }
            return false;
        }
        public bool IsProductAdded(Cart updateCart)
        {
            var lis = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == updateCart.Id).FirstOrDefault();
            Guid id = lis.Product.Select(find => find.ProductId).First();
            bool isTh = lis.Product.Any(term => term.ProductId == id);
            if(isTh)
            {
                _orderContext.Cart.Update(updateCart);
                _orderContext.SaveChanges();
                return true;
            }
            return false;

        }
        public bool IsWishListNameExists(WishList wishList)
        {
            var b = _orderContext.WishList.Any(term => term.Name == wishList.Name);
            if(!b)
            {
                _orderContext.WishList.Add(wishList);
                _orderContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteWishList(Guid id)
        {
            var b = _orderContext.WishList.Any(find => find.Id == id);
            if(b)
            {
                var wish = _orderContext.WishList.First(find => find.Id == id);
                _orderContext.WishList.Remove(wish);
                _orderContext.SaveChanges();
                return true;
            }
            return false;
        }
        public Tuple<string,string> SaveProductWishList(Entity.Model.WishListProduct productWishlist)
        {
            var b = _orderContext.WishList.Include(find => find.WishListProduct).Where(find => find.Id == productWishlist.WishListId).FirstOrDefault();
            if(b == null)
            {
                return new Tuple<string, string>("wishlist", productWishlist.WishListId.ToString());
            }
            var bo = b.WishListProduct.Where(find => find.ProductId == productWishlist.ProductId).FirstOrDefault();
            if(bo == null)
            {
                return new Tuple<string, string>("product", productWishlist.ProductId.ToString());
            }
            _orderContext.WishListProduct.Add(productWishlist);
            _orderContext.SaveChanges();
            return new Tuple<string, string>(string.Empty,string.Empty);
        }
        public Tuple<string,string> DeleteProductWishList(Guid id, Guid productId)
        {
            var x = _orderContext.WishList.Where(find => find.Id == id).FirstOrDefault();
            if (x == null)
            {
                return new Tuple<string, string>("wishlist", id.ToString());
            }
            var bo = x.WishListProduct.Where(find => find.ProductId == productId).FirstOrDefault();
            if (bo == null)
            {
                return new Tuple<string, string>("product", productId.ToString());
            }
            _orderContext.WishList.Remove(x);        
            _orderContext.SaveChanges();
            return new Tuple<string, string>(string.Empty, string.Empty);
        }
        public bool IsProductRemoved(Guid id, Guid userId)
        {
            var i = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == userId).FirstOrDefault();
            var b = i.Product.Where(find => find.ProductId == id).FirstOrDefault();
            if(b != null)
            {
                _orderContext.Product.Remove(b);
                _orderContext.SaveChanges();
                return true;
            }
            return false;

        }
    }
}
