using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.ViewModels
{
    public class MemberOrder
    {

        public Order _order { get; set; }
        public IEnumerable<OrderDetail> OrderDetails { get; set; }
        public IEnumerable<Product> Products { get; set; }
        public IEnumerable<User> Users { get; set; }


        // Order
        public int OrderId { get; set; }
        public int UserId { get; set; }
        public int? TotalPrice { get; set; }
        public string PayMethod { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Approval { get; set; }

        // OrderDetail
        public string ProductId { get; set; }
        public int? UnitPrice { get; set; }
        public short? Quantity { get; set; }
        public double? Discount { get; set; }
        public string Commets { get; set; }
        public int Odpk { get; set; }


        // Products
        public string Address { get; set; }
        public string UserName { get; set; }

        // Users
        public string Email { get; set; }
        public string UserAccount { get; set; }
        public string UserPassword { get; set; }
        public string ConfirmPassword { get; set; }
        public DateTime? Birthday { get; set; }
        public string Region { get; set; }
        public string Phone { get; set; }
        public string Img { get; set; }
        public string DiscountCode { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UserStatus { get; set; }
        public bool? IsSuccess { get; set; }
    }
}
