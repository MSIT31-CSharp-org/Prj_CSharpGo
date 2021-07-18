using System;
using System.Collections.Generic;

#nullable disable

namespace Prj_CSharpGo.Models
{
    public partial class User
    {
        public User()
        {
            CampOrders = new HashSet<CampOrder>();
            Orders = new HashSet<Order>();
            Recipes = new HashSet<Recipe>();
        }

        public int UserId { get; set; }
        public string UserAccount { get; set; }
        public string UserPassword { get; set; }
        public string UserName { get; set; }
        public string Birthday { get; set; }
        public string Region { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Img { get; set; }
        public string DiscountCode { get; set; }
        public DateTime? UpdateDate { get; set; }
        public string UserStatus { get; set; }
        public string Vip { get; set; }

        public virtual ICollection<CampOrder> CampOrders { get; set; }
        public virtual ICollection<Order> Orders { get; set; }
        public virtual ICollection<Recipe> Recipes { get; set; }
    }
}
