using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.ShopCartViewModels
{
    public class PShopCartViewMolds
    {
        public Order order { get; set; }

        public IEnumerable<Product> Product { get; set; }

        public IEnumerable<User> User { get; set; }
        public IEnumerable<ShoppingCart> ShoppingCart { get; set; }

        public int? SMTotal { get; set; }

        public int? BigTotal { get; set; }

    }
}
