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
        public void AddProduct(Product product, Guid userId)
        {
            var x = _orderContext.Cart.Include(find => find.Product).Where(find => find.UserId == userId && find.BillNo == 0 ).FirstOrDefault();
            product.CartId = x.Id;
            product.Cart = null;
            bool t = true;
            //if(x.Product.Count() == 0)
            //{
            //    _orderContext.Product.Add(product);
            //    _orderContext.SaveChanges();
            //    t = false;

            //}
            var f = x.Product.Where(find => find.ProductId == product.ProductId).FirstOrDefault();
            if(f != null && t)
            {
                f.Quantity = f.Quantity + product.Quantity;
                _orderContext.Product.Update(f);
                _orderContext.SaveChanges();

            }
            else
            {
                _orderContext.Product.Add(product);
                _orderContext.SaveChanges();
            }
            

            //try
            //{
            //    _orderContext.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException ex)
            //{
            //        ex.Entries.Single().Reload();
            //        _orderContext.SaveChanges();

            //}


            //Cart cartItem = _orderContext.Cart.Include(term => term.BillNo).Include(term => term.Product).Where(find => find.UserId == cart.UserId).FirstOrDefault();

            //if (cartItem == null)
            //{
            //    foreach (var item in x.Product)
            //    {
            //        if (item.ProductId == x.Product.Select(item => item.ProductId).First())
            //        {
            //            item.Quantity = item.Quantity + x.Product.Select(src => src.Quantity).First();
            //            _orderContext.Cart.Update(x);
            //            break;
            //        }
            //    }
            //    _orderContext.Cart.Add(cart);
            //}
            //else
            //{
            //    Guid CartId = cart.Product.Select(item => item.ProductId).First();
            //    bool isProductExists = cartItem.Product.Any(find => find.ProductId == CartId);
            //    if (isProductExists)
            //    {
            //        foreach (var item in cartItem.Product)
            //        {
            //            if (item.ProductId == cart.Product.Select(item => item.ProductId).First())
            //            {
            //                item.Quantity = item.Quantity + cart.Product.Select(src => src.Quantity).First();
            //                _orderContext.Cart.Update(cartItem);  
            //                break;
            //            }
            //        }
            //    }
            //    else
            //    {
            //        cartItem.Product = new List<Product>();
            //        Product product = new Product
            //        {
            //            Id = Guid.NewGuid(),
            //            Quantity=cart.Product.Select(item => item.Quantity).First(),
            //            ProductId = cart.Product.Select(item => item.Id).First(),
            //            CartId=cart.Id
            //        };
            //        cartItem.Product.Add(product);
            //        _orderContext.Product.Add(product);
            //    }
            //    _orderContext.SaveChanges();
            //}

        }
        public Cart GetAllProducstInCart(Guid id)
        {
            var f = _orderContext.Cart.Include(term =>term.Product).Where(find => find.UserId == id && find.BillNo == 0).FirstOrDefault();
            
            return f;
        }

        public bool IsProductInCart(Guid id, Guid userId)
        {
            var li = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == userId && find.BillNo == 0).FirstOrDefault();
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
        public void UpdateCart(Cart updateCart)
        {
            //var lis = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == updateCart.UserId && find.BillNo == 0).FirstOrDefault();
            //// Guid id = lis.Product.Select(find => find.ProductId).First();

            //var isTh = lis.Product.Where(find => find.ProductId == id).FirstOrDefault();
            _orderContext.Cart.Update(updateCart);
            _orderContext.SaveChanges();
            //isTh.Quantity = isTh.Quantity + 1;
            //if (isTh != null)
            //{
            //    _orderContext.Product.Update(isTh);
            //    _orderContext.SaveChanges();
            //    return true;
            //}
            //_orderContext.Cart.Update(updateCart);
            //return false;

        }
        public bool IsWishListNameExists(WishList wishList)
        {
            var b = _orderContext.WishList.Any(term => term.Name == wishList.Name && term.UserId == wishList.UserId);
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
        public bool SaveProductWishList(Entity.Model.WishListProduct productWishlist)
        {
            var b = _orderContext.WishList.Include(find => find.WishListProduct).Where(find => find.Id == productWishlist.WishListId).FirstOrDefault();
            var t = b.WishListProduct.Any(find => find.ProductId == productWishlist.ProductId);
            if(t)
            {
                return true;
            }
            _orderContext.WishListProduct.Add(productWishlist);
            _orderContext.SaveChanges();
            return true;
            
            //if(b == null)
            //{
            //    return new Tuple<string, string>("wishlist", productWishlist.WishListId.ToString());
            //}
            //var bo = b.WishListProduct.Where(find => find.ProductId == productWishlist.ProductId).FirstOrDefault();
            //if(bo == null)
            //{
            //    return new Tuple<string, string>("product", productWishlist.ProductId.ToString());
            //}
            //_orderContext.WishListProduct.Add(productWishlist);
            //_orderContext.SaveChanges();
            //return new Tuple<string, string>(string.Empty,string.Empty);
        }
        public Tuple<string,string> DeleteProductWishList(Guid id, Guid productId)
        {
            var x = _orderContext.WishList.Include(term =>term.WishListProduct).Where(find => find.Id == id).FirstOrDefault();
            if (x == null)
            {
                return new Tuple<string, string>("wishlist", id.ToString());
            }
            var bo = x.WishListProduct.Where(find => find.ProductId == productId).FirstOrDefault();
            if (bo == null)
            {
                return new Tuple<string, string>("product", productId.ToString());
            }
            _orderContext.WishListProduct.Remove(bo);        
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
        public int GenerateBillNo(Bill payment,Guid cartId)
        {
            var x = _orderContext.Cart.Include(term => term.Bill).Where(find => find.Id == cartId).First();
            var c = _orderContext.Cart.Select(term => term.Bill).Count();
            x.BillNo = c;
            foreach (var item in x.Bill)
            {
                item.BillNo = c;
            }
            Cart cart = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = x.UserId
            };
            
            _orderContext.Cart.Update(x);
            _orderContext.Cart.Add(cart);
            _orderContext.SaveChanges();
            return c;
        }
        public Tuple<string, string> IsProductExistInCart(WishListToCart cartTOWishList,Guid userId)
        {
            Guid wishListId = Guid.Parse(cartTOWishList.WishListId);
            var x = _orderContext.WishList.Include(term => term.WishListProduct).Where(find => find.Id == wishListId).FirstOrDefault();
            if (x == null)
            {
                return new Tuple<string, string>("wishlist", cartTOWishList.WishListId.ToString());
            }
            var bo = x.WishListProduct.Where(find => find.ProductId == Guid.Parse(cartTOWishList.ProductId)).FirstOrDefault();
            if (bo == null)
            {
                return new Tuple<string, string>("product", cartTOWishList.ProductId.ToString());
            }
            // x.WishListProduct.Where(find => find.ProductId == cartTOWishList.ProductId)
            var xx = x.WishListProduct.Where(find => find.ProductId == Guid.Parse(cartTOWishList.ProductId)).First();
            _orderContext.WishListProduct.Remove(xx);
            var c = _orderContext.Cart.Include(src => src.Product).Where(find => find.UserId == userId && find.BillNo == 0).FirstOrDefault() ;   //.Select(s => s.Product.Where(s => s.ProductId == wishListId).FirstOrDefault()).FirstOrDefault();
            var t = c.Product.Where(find => find.ProductId == Guid.Parse(cartTOWishList.ProductId)).FirstOrDefault();
            
            if(t != null)
            {
                t.Quantity = t.Quantity + 1;
                _orderContext.Product.Update(t);
            }
            else
            {
                Product product = new Product()
                {
                    ProductId = Guid.Parse(cartTOWishList.ProductId),
                    Id = Guid.NewGuid(),
                    CartId=c.Id,
                    Quantity = 1
                };
                _orderContext.Product.Add(product);
            }
            _orderContext.SaveChanges();
            return new Tuple<string, string>(string.Empty, string.Empty);
        }
        public int GetBillNo()
        {
            return _orderContext.Cart.Select(sel => sel.BillNo).Count() + 1;
        }
        public void CheckOut(Cart cart, Guid userId)
        {
            //var o = _orderContext.Cart.Select(sel => sel.BillNo).Count() + 1;
            //cart.BillNo = o;
            //foreach (var item in cart.Bill)
            //{
            //    item.BillNo = o;
            //}
            _orderContext.Cart.Add(cart);
            _orderContext.SaveChanges();
           // return o;
        }

        public void CreateCart(Cart cart)
        {
            _orderContext.Cart.Add(cart);
            _orderContext.SaveChanges();
        }
        public bool IsCartExist(Guid id)
        {
            return _orderContext.Cart.Any(find => find.UserId == id && find.BillNo == 0);
            
        }
        public Cart GetCartProducts(Guid id)
        {
            var o = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == id && find.BillNo == 0).FirstOrDefault();
            if (o == null)
            {
                return null;
            }
            return o;
            
        }
        public bool CheckWishList(Guid wishListId)
        {
            return _orderContext.WishList.Any(find => find.Id == wishListId);
        }
        public List<WishListProduct> GetProductsInWishList(Guid userId, Guid wishListId)
        {
            var o = _orderContext.WishList.Include(term => term.WishListProduct).Where(find => find.Id == wishListId && find.UserId == userId).FirstOrDefault();
            if(o == null)
            {
                return null;
            }
            var x = o.WishListProduct.ToList();
            return x;
        }
        public Guid GetWishlistId(string name)
        {
            return _orderContext.WishList.Where(find => find.Name == name).Select(src => src.Id).First();
        }
        public Cart? GetCart(Guid userId)
        {
            var x = _orderContext.Cart.Include(src => src.Product).Where(find => find.UserId == userId && find.BillNo == 0).First();
            return x;

        }
        public void DeleteUserData(Guid id)
        {
            var x = _orderContext.Cart.Where(find => find.UserId == id).ToList();
            _orderContext.Cart.RemoveRange(x);
            var y =_orderContext.WishList.Where(find => find.UserId == id).ToList();
            _orderContext.WishList.RemoveRange(y);
            _orderContext.SaveChanges();
        }
        public bool IsUserExist(Guid userId)
        {
            return _orderContext.Cart.Any(find =>find.UserId == userId);
        }
        public bool IsTheUserExist(Guid userId)
        {
            return _orderContext.Cart.Any(find => find.UserId == userId);
        }
    }
}
