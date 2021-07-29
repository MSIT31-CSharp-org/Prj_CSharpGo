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

    }



    //public class ShowAllshCart {

    //    public Order order { get; set; }

    //    public int? SMTotal { get; set; }

    //    public int? BigTotal { get; set; }
    //    }
    
}
