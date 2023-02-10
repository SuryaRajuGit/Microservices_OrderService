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
        ///<summary>
        /// Adds product to cart
        ///</summary>
        public void AddProduct(Product product, Guid userId)
        {
            Cart cart = _orderContext.Cart.Include(find => find.Product).Where(find => find.UserId == userId && find.BillNo == 0 && find.IsActive).FirstOrDefault();
            product.CartId = cart.Id;
            product.Cart = null;
            product.UpdatedDate = DateTime.Now;
            product.UpdatedBy = userId;
            Product product1 = cart.Product.Where(find => find.ProductId == product.ProductId && find.IsActive).FirstOrDefault();
            if(product1 != null )
            {
                product1.Quantity = product1.Quantity + product.Quantity;
                product1.IsActive = true;
                product1.UpdatedDate = DateTime.Now;
                _orderContext.SaveChanges();
            }
            else
            {
                _orderContext.Product.Add(product);
                _orderContext.SaveChanges();
            }
        }
        ///<summary>
        /// returns all products in the user cart
        ///</summary>
        public Cart GetAllProducstInCart(Guid id)
        {
            Cart cart = _orderContext.Cart.Include(term =>term.Product).Where(find => find.UserId == id && find.BillNo == 0 && find.IsActive).FirstOrDefault();
            return cart;
        }
        ///<summary>
        /// Checks if the product exist in the cart
        ///</summary>
        public bool IsProductInCart(Guid id, Guid userId)
        {
            Cart cart = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == userId && find.BillNo == 0 && find.IsActive).FirstOrDefault();
            bool isthere = cart.Product.Any(term => term.ProductId == id);
            if (isthere)
            {
                foreach (Product term in cart.Product.Where(sel => sel.IsActive && sel.ProductId == id))
                {
                    if (term.ProductId == id)
                    {
                        term.IsActive = false;
                        _orderContext.SaveChanges();
                        return true;
                    }
                }
            }
            return false;
        }
        ///<summary>
        /// updates cart details and return bool
        ///</summary>
        public bool GetCart(UpdateCart cart, Guid id)
        {
            Cart cart1 = _orderContext.Cart.Include(sel => sel.Product).Where(find => find.IsActive
             && find.UserId == id).FirstOrDefault();
            
            Product product = cart1.Product.Where(sel => sel.ProductId == cart.ProductId && sel.IsActive).FirstOrDefault();
            if (product == null)
            {
                return false;
            }
            product.Quantity = cart.Quantity == 0 ? product.Quantity : cart.Quantity;
            _orderContext.SaveChanges();
            return true;
        }
        ///<summary>
        /// Saves updated cart details
        ///</summary>
        public void UpdateCart(Cart updateCart)
        {
            _orderContext.Cart.Update(updateCart);
            _orderContext.SaveChanges();
        }
        ///<summary>
        /// Checks wish-list name exist or not
        ///</summary>
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
        ///<summary>
        /// Deletes wish-list 
        ///</summary>
        public bool DeleteWishList(Guid id)
        {
            bool isWishListExist = _orderContext.WishList.Any(find => find.Id == id && find.IsActive);
            if(isWishListExist)
            {
                WishList wish = _orderContext.WishList.First(find => find.Id == id && find.IsActive);
                wish.IsActive = false;
                wish.UpdatedDate = DateTime.Now;
                _orderContext.WishList.Update(wish);
                _orderContext.SaveChanges();
                return true;
            }
            return false;
        }
        ///<summary>
        /// Saves product wish-list 
        ///</summary>
        public bool SaveProductWishList(Entity.Model.WishListProduct productWishlist)
        {
            WishList wishlist = _orderContext.WishList.Include(find => find.WishListProduct).Where(find => find.Id == productWishlist.WishListId && find.IsActive).FirstOrDefault();
            bool isWishListExist = wishlist.WishListProduct.Any(find => find.ProductId == productWishlist.ProductId && find.IsActive);
            if(isWishListExist)
            {
                return true;
            }
            _orderContext.WishListProduct.Add(productWishlist);
            _orderContext.SaveChanges();
            return false;
        }
        ///<summary>
        /// Deletes product in wish-list
        ///</summary>
        public Tuple<string,string> DeleteProductWishList(Guid id, Guid productId)
        {
            WishList wishList = _orderContext.WishList.Include(term =>term.WishListProduct).Where(find => find.Id == id && find.IsActive).FirstOrDefault();
            if (wishList == null)
            {
                return new Tuple<string, string>("wishlist", id.ToString());
            }
            WishListProduct wishListProduct = wishList.WishListProduct.Where(find => find.ProductId == productId && find.IsActive).FirstOrDefault();
            if (wishListProduct == null)
            {
                return new Tuple<string, string>("product", productId.ToString());
            }
            _orderContext.WishListProduct.Remove(wishListProduct);        
            _orderContext.SaveChanges();
            return new Tuple<string, string>(string.Empty, string.Empty);
        }
        ///<summary>
        /// Removes product from user cart
        ///</summary>
        public bool IsProductRemoved(Guid id, Guid userId)
        {
            Cart cart = _orderContext.Cart.Include(term => term.Product.Where(sel => sel.IsActive)).Where(find => find.UserId == userId && find.IsActive).FirstOrDefault();
            Product product = cart.Product.Where(find => find.ProductId == id && find.IsActive).FirstOrDefault();
            if(product != null)
            {
                product.IsActive = false;
                product.UpdatedDate = DateTime.Now;
                product.UpdatedBy = userId;
                _orderContext.Product.Update(product);
                _orderContext.SaveChanges();
                return true;
            }
            return false;
        }
        ///<summary>
        /// Generates bill no 
        ///</summary>
        public int GenerateBillNo(Cart cart, Bill bill, Guid cartId)
        {
            Cart cart1 = new Cart()
            {
                Id = Guid.NewGuid(),
                UserId = cart.UserId,
                IsActive = true,
                UpdatedDate = DateTime.Now,
            };
            _orderContext.Cart.Update(cart);
            _orderContext.Bill.Add(bill);
            _orderContext.Cart.Add(cart1);
            _orderContext.SaveChanges();
            return bill.Id;
        }
        ///<summary>
        /// Checks if the product exist in the cart or not
        ///</summary>
        public Tuple<string, string> IsProductExistInCart(WishListToCart cartTOWishList,Guid userId)
        {
            Guid wishListId = Guid.Parse(cartTOWishList.WishListId);
            WishList wishList = _orderContext.WishList.Include(term => term.WishListProduct).Where(find => find.Id == wishListId && find.IsActive).FirstOrDefault();
            if (wishList == null)
            {
                return new Tuple<string, string>("wishlist", cartTOWishList.WishListId.ToString());
            }
            WishListProduct wishListProduct = wishList.WishListProduct.Where(find => find.ProductId == Guid.Parse(cartTOWishList.ProductId) && find.IsActive).FirstOrDefault();
            if (wishListProduct == null)
            {
                return new Tuple<string, string>("product", cartTOWishList.ProductId.ToString());
            }
            WishListProduct wishListProduct1 = wishList.WishListProduct.Where(find => find.ProductId == Guid.Parse(cartTOWishList.ProductId) && find.IsActive).First();
            wishListProduct1.IsActive = false;
            wishListProduct1.UpdatedDate = DateTime.Now;
            wishListProduct1.UpdatedBy = userId;
            _orderContext.WishListProduct.Update(wishListProduct1);
            Cart cart = _orderContext.Cart.Include(src => src.Product).Where(find => find.UserId == userId && find.BillNo == 0 && find.IsActive).FirstOrDefault() ;   //.Select(s => s.Product.Where(s => s.ProductId == wishListId).FirstOrDefault()).FirstOrDefault();
            Product product1 = cart.Product.Where(find => find.ProductId == Guid.Parse(cartTOWishList.ProductId) && find.IsActive).FirstOrDefault();
            
            if(product1 != null)
            {
                product1.Quantity = product1.Quantity + 1;
                product1.UpdatedDate = DateTime.Now;
                product1.UpdatedBy = userId;
                product1.IsActive = true;
            }
            else
            {
                Product product = new Product()
                {
                    ProductId = Guid.Parse(cartTOWishList.ProductId),
                    Id = Guid.NewGuid(),
                    CartId = cart.Id,
                    Quantity = 1,
                    UpdatedBy = userId,
                    UpdatedDate = DateTime.Now,
                    IsActive = true
                };
                _orderContext.Product.Add(product);
            }
            _orderContext.SaveChanges();
            return new Tuple<string, string>(string.Empty, string.Empty);
        }
        ///<summary>
        /// Generates bill number 
        ///</summary>
        public int GetBillNo()
        {
            int max = 0;
            foreach (Bill item in _orderContext.Bill)
            {
                int billNo = item.Id;
                if(billNo > max )
                {
                    max = billNo;
                }
            }
            return max + 1;
        }
        ///<summary>
        /// updates cart details 
        ///</summary>
        public void CheckOut(Cart cart, Guid userId)
        {
            _orderContext.Cart.Add(cart);
            _orderContext.SaveChanges();
        }
        ///<summary>
        /// Create cart for user
        ///</summary>
        public void CreateCart(Cart cart)
        {
            _orderContext.Cart.Add(cart);
            _orderContext.SaveChanges();
        }
        ///<summary>
        /// checks cart id exist or not
        ///</summary>
        public bool IsCartExist(Guid id)
        {
            return _orderContext.Cart.Any(find => find.UserId == id && find.BillNo == 0 && find.IsActive);
        }
        ///<summary>
        /// returns Cart model 
        ///</summary>
        public Cart GetCartProducts(Guid id)
        {
            Cart cart = _orderContext.Cart.Include(term => term.Product).Where(find => find.UserId == id && find.IsActive && find.BillNo == 0).FirstOrDefault();
            if (cart == null)
            {
                return null;
            }
            return cart;
        }
        ///<summary>
        /// Checks wish-list id exist or not
        ///</summary>
        public bool CheckWishList(Guid wishListId)
        {
            return _orderContext.WishList.Any(find => find.Id == wishListId && find.IsActive);
        }
        ///<summary>
        /// Returns all products in wish-list
        ///</summary>
        public List<WishListProduct> GetProductsInWishList(Guid userId, Guid wishListId)
        {
            WishList wishList = _orderContext.WishList.Include(term => term.WishListProduct).Where(find => find.Id == wishListId && find.UserId == userId && find.IsActive).FirstOrDefault();
            if(wishList == null)
            {
                return null;
            }
            List<WishListProduct> productList = wishList.WishListProduct.ToList();
            return productList;
        }
        ///<summary>
        /// Resturns wish-list id
        ///</summary>
        public Guid GetWishlistId(string name,Guid id)
        {
            return _orderContext.WishList.Where(find => find.Name == name && find.UserId == id && find.IsActive).Select(src => src.Id).First();
        }
        ///<summary>
        /// Return Cart Model
        ///</summary>
        public Cart? GetCart(Guid userId)
        {
            Cart cart = _orderContext.Cart.Include(src => src.Product.Where(sel => sel.IsActive)).Where(find => find.UserId == userId && find.BillNo == 0 && find.IsActive).First();
            return cart;

        }
        ///<summary>
        /// Deletes user details 
        ///</summary>
        public void DeleteUserData(Guid id)
        {
            foreach (Cart item in _orderContext.Cart.Where(find => find.IsActive && find.UserId == id))
            {
                item.IsActive = false;
            }
            foreach (WishList item in _orderContext.WishList.Where(sel => sel.IsActive && sel.UserId == id))
            {
                item.IsActive = false;
            }
            _orderContext.SaveChanges();
        }
        ///<summary>
        /// checks user id exist or not
        ///</summary>
        public bool IsUserExist(Guid userId)
        {
            return _orderContext.Cart.Any(find => find.UserId == userId && find.IsActive);
        }
        ///<summary>
        /// checks user id exist or not
        ///</summary>
        public bool IsTheUserExist(Guid userId)
        {
            return _orderContext.Cart.Any(find => find.UserId == userId && find.IsActive);
        }
        ///<summary>
        ///Returns list of order details
        ///</summary>
        public List<Bill> GetOrderDetails(Guid userId)
        {

            List<Bill> bill = new List<Bill>();
            foreach (Cart item in _orderContext.Cart.Include(src=>src.Bill).Where(find =>find.UserId == userId && find.IsActive))
            {
                if(item.BillNo != 0 && item.IsActive)
                {
                    Bill bill1 = item.Bill.First();
                    bill.Add(bill1);
                }
            }
            return bill;
        }
        ///<summary>
        /// Returns single order details 
        ///</summary>
        public Bill GetOrderDetails(Guid userId, int billNo)
        {
            return _orderContext.Bill.Include(sel => sel.Cart).Where(find => find.Id == billNo && find.IsActive).First();
        }
        ///<summary>
        /// Checks order id exist or not
        ///</summary>
        public bool IsOrderIdExist(int id)
        {
            return _orderContext.Bill.Any(src => src.Id == id && src.IsActive);
        }
        public List<Cart> GetOrderProductIds(Guid id)
        {
            return _orderContext.Cart.Include(sel => sel.Product).Where(sel => sel.IsActive && sel.UserId == id && sel.BillNo != 0).ToList();
        }
        public Cart GetCart(int i)
        {
            return _orderContext.Cart.Include(sel => sel.Product).Where(sel => sel.IsActive && sel.BillNo == i).First();
        }
    }
}
