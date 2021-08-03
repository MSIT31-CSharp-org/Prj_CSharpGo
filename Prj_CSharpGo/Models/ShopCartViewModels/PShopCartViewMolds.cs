using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.ShopCartViewModels
{

    public class returnshCartIndexVM
    {
        public List<Product> Products { get; set; }
        public List<User> Users { get; set; }
        public List<ShoppingCart> ShoppingCarts { get; set; }
        public ShoppingCartUserFilterModel PuserFilterModel { get; set; }

        
        
    }

    public class ShoppingCartUserFilterModel
    {
       
        public int UserId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int? Quantity { get; set; }
        public int? UnitPrice { get; set; }
        public int? SMTotal { get; set; }
        public int? BigTotal { get; set; }

    }

    public class SaveSCtoOrderModel : ShoppingCart
    {

    }
}
    
    