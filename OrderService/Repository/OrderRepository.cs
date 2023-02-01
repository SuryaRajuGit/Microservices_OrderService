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
            Cart cart = _orderContext.Cart.Include(find => find.Product).Where(find => find.UserId == userId && find.BillNo == 0 ).FirstOrDefault();
            product.CartId = cart.Id;
            product.Cart = null;
            Product product1 = cart.Product.Where(find => find.ProductId == product.ProductId).FirstOrDefault();
            if(product1 != null )
            {
                product1.Quantity = product1.Quantity + product.Quantity;
                _orderContext.Product.Update(product1);
                _orderContext.SaveChanges();
            }
            else
            {
                _orderContext.Product.Add(product);
                _orderContext.SaveChanges();
            }
        }
        public Cart GetAllProducstInCart(Guid id)
        {
            Cart cart = _orderContext.Cart.Include(term =>term.Product).Where(find => find.UserId == id && find.BillNo == 0).FirstOrDefault();
            
            return cart;
        }

        public bool IsProductInCart(Guid id, Guid userId)
        {
            Cart cart = _orderContext.Cart.Include(term => term.Product.Where(sel =>sel.IsActive && sel.ProductId == id)).Where(find => find.UserId == userId && find.BillNo == 0 && find.IsActive).FirstOrDefault();
            bool isthere = cart.Product.Any(term => term.ProductId == id);
            if (isthere)
            {
                foreach (Product term in cart.Product)
                {
                    if (term.ProductId == id)
                    {
                        Product product = cart.Product.Where(find => find.ProductId == id).FirstOrDefault();
                        product.IsActive = false;
                        _orderContext.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }
        public bool GetCart(UpdateCart cart, Guid id)
        {
            Cart cart1 = _orderContext.Cart.Include(sel => sel.Product.Where(sel => sel.IsActive && sel.ProductId == cart.ProductId)).Where(find => find.IsActive
             && find.UserId == id).FirstOrDefault();
            if(cart1.Product.Count() == 0)
            {
                return false;
            }
            Product product = cart1.Product.Where(sel => sel.ProductId == cart.ProductId).FirstOrDefault();
            product.Quantity = cart.Quantity;
            _orderContext.SaveChanges();
            return true;


        }
        public void UpdateCart(Cart updateCart)
        {
            _orderContext.Cart.Update(updateCart);
            _orderContext.SaveChanges();
        }
        public bool IsWishListNameExists(WishList wishList)
        {
            bool isWishListIdExist = _orderContext.WishList.Any(term => term.Name == wishList.Name && term.IsActive && term.UserId == wishList.UserId);
            if(!isWishListIdExist)
            {
                _orderContext.WishList.Add(wishList);
                _orderContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool DeleteWishList(Guid id)
        {
            bool isWishListExist = _orderContext.WishList.Any(find => find.Id == id);
            if(isWishListExist)
            {
                WishList wish = _orderContext.WishList.First(find => find.Id == id);
                _orderContext.WishList.Remove(wish);
                _orderContext.SaveChanges();
                return true;
            }
            return false;
        }
        public bool SaveProductWishList(Entity.Model.WishListProduct productWishlist)
        {
            WishList wishlist = _orderContext.WishList.Include(find => find.WishListProduct).Where(find => find.Id == productWishlist.WishListId).FirstOrDefault();
            bool isWishListExist = wishlist.WishListProduct.Any(find => find.ProductId == productWishlist.ProductId);
            if(isWishListExist)
            {
                return true;
            }
            _orderContext.WishListProduct.Add(productWishlist);
            _orderContext.SaveChanges();
            return false;
        }
        public Tuple<string,string> DeleteProductWishList(Guid id, Guid productId)
        {
            WishList wishList = _orderContext.WishList.Include(term =>term.WishListProduct.Where(sel => sel.IsActive)).Where(find => find.Id == id).FirstOrDefault();
            if (wishList == null)
            {
                return new Tuple<string, string>("wishlist", id.ToString());
            }
            WishListProduct wishListProduct = wishList.WishListProduct.Where(find => find.ProductId == productId).FirstOrDefault();
            if (wishListProduct == null)
            {
                return new Tuple<string, string>("product", productId.ToString());
            }
            _orderContext.WishListProduct.Remove(wishListProduct);        
            _orderContext.SaveChanges();
            return new Tuple<string, string>(string.Empty, string.Empty);
        }
        public bool IsProductRemoved(Guid id, Guid userId)
        {
            Cart cart = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == userId).FirstOrDefault();
            Product product = cart.Product.Where(find => find.ProductId == id).FirstOrDefault();
            if(product != null)
            {
                _orderContext.Product.Remove(product);
                _orderContext.SaveChanges();
                return true;
            }
            return false;
        }
        public int GenerateBillNo(Cart cart, Bill bill, Guid cartId)
        {
            Cart cart1 = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = cart.UserId
            };
            _orderContext.Cart.Update(cart);
            _orderContext.Bill.Add(bill);
            _orderContext.Cart.Add(cart1);
            _orderContext.SaveChanges();
            return bill.Id;
        }
        public Tuple<string, string> IsProductExistInCart(WishListToCart cartTOWishList,Guid userId)
        {
            Guid wishListId = Guid.Parse(cartTOWishList.WishListId);
            WishList wishList = _orderContext.WishList.Include(term => term.WishListProduct).Where(find => find.Id == wishListId).FirstOrDefault();
            if (wishList == null)
            {
                return new Tuple<string, string>("wishlist", cartTOWishList.WishListId.ToString());
            }
            WishListProduct wishListProduct = wishList.WishListProduct.Where(find => find.ProductId == Guid.Parse(cartTOWishList.ProductId)).FirstOrDefault();
            if (wishListProduct == null)
            {
                return new Tuple<string, string>("product", cartTOWishList.ProductId.ToString());
            }
            WishListProduct wishListProduct1 = wishList.WishListProduct.Where(find => find.ProductId == Guid.Parse(cartTOWishList.ProductId)).First();
            _orderContext.WishListProduct.Remove(wishListProduct1);
            Cart cart = _orderContext.Cart.Include(src => src.Product).Where(find => find.UserId == userId && find.BillNo == 0).FirstOrDefault() ;   //.Select(s => s.Product.Where(s => s.ProductId == wishListId).FirstOrDefault()).FirstOrDefault();
            Product product1 = cart.Product.Where(find => find.ProductId == Guid.Parse(cartTOWishList.ProductId)).FirstOrDefault();
            
            if(product1 != null)
            {
                product1.Quantity = product1.Quantity + 1;
                _orderContext.Product.Update(product1);
            }
            else
            {
                Product product = new Product()
                {
                    ProductId = Guid.Parse(cartTOWishList.ProductId),
                    Id = Guid.NewGuid(),
                    CartId=cart.Id,
                    Quantity = 1
                };
                _orderContext.Product.Add(product);
            }
            _orderContext.SaveChanges();
            return new Tuple<string, string>(string.Empty, string.Empty);
        }
        public int GetBillNo()
        {
            int max = 0;
            foreach (Bill item in _orderContext.Bill)
            {
                int billNo = item.Id;
                if(billNo > max)
                {
                    max = billNo;
                }
            }
            return max + 1;
        }
        public void CheckOut(Cart cart, Guid userId)
        {
            _orderContext.Cart.Add(cart);
            _orderContext.SaveChanges();
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
            Cart cart = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == id && find.BillNo == 0).FirstOrDefault();
            if (cart == null)
            {
                return null;
            }
            return cart;
            
        }
        public bool CheckWishList(Guid wishListId)
        {
            return _orderContext.WishList.Any(find => find.Id == wishListId);
        }
        public List<WishListProduct> GetProductsInWishList(Guid userId, Guid wishListId)
        {
            WishList wishList = _orderContext.WishList.Include(term => term.WishListProduct).Where(find => find.Id == wishListId && find.UserId == userId).FirstOrDefault();
            if(wishList == null)
            {
                return null;
            }
            List<WishListProduct> productList = wishList.WishListProduct.ToList();
            return productList;
        }
        public Guid GetWishlistId(string name)
        {
            return _orderContext.WishList.Where(find => find.Name == name).Select(src => src.Id).First();
        }
        public Cart? GetCart(Guid userId)
        {
            Cart cart = _orderContext.Cart.Include(src => src.Product).Where(find => find.UserId == userId && find.BillNo == 0).First();
            return cart;

        }
        public void DeleteUserData(Guid id)
        {
            List<Cart> cartList = _orderContext.Cart.Where(find => find.UserId == id).ToList();
            _orderContext.Cart.RemoveRange(cartList);
            List<WishList> wishList =_orderContext.WishList.Where(find => find.UserId == id).ToList();
            _orderContext.WishList.RemoveRange(wishList);
            _orderContext.SaveChanges();
        }
        public bool IsUserExist(Guid userId)
        {
            return _orderContext.Cart.Any(find => find.UserId == userId);
        }
        public bool IsTheUserExist(Guid userId)
        {
            return _orderContext.Cart.Any(find => find.UserId == userId);
        }

        public List<Bill> GetOrderDetails(Guid userId)
        {
            List<Bill> bill = new List<Bill>();
            foreach (Cart item in _orderContext.Cart.Include(src=>src.Bill).Where(find =>find.UserId == userId))
            {
                if(item.BillNo != 0)
                {
                    Bill bill1 = item.Bill.First();
                    bill1.Cart = null;
                    bill.Add(bill1);
                }
            }
            return bill;
        }
        public Bill GetOrderDetails(Guid userId, int billNo)
        {
            return _orderContext.Bill.Where(find => find.Id == billNo).First();
        }
        public bool IsOrderIdExist(int id)
        {
            return _orderContext.Bill.Any(src => src.Id == id);
        }
    }
}
