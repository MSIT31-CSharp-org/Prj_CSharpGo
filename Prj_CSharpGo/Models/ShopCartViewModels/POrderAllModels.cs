using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Prj_CSharpGo.Models.ShopCartViewModels
{
    public class POrderAllModel
    {
        private int uId;
        private int productId;
        private int quantity;
      
        public POrderAllModel(int uId, int productId, string productName, int unitPrice, int quantity, int sMTotal)
        {
            UId = uId;
            this.productId = productId;
            ProductName = productName;
            UnitPrice = unitPrice;
            this.quantity = quantity;
            SMTotal = sMTotal;
        }

        public int OrderId { get; set; }
        public int UserId { get; set; }
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public int? UnitPrice { get; set; }
        public short? Quantity { get; set; }
        public int? TotalPrice { get; set; }
        public double? Discount { get; set; }
        public string PayMethod { get; set; }
        public DateTime? OrderDate { get; set; }
        public string Commets { get; set; }
        public string Approval { get; set; }
        public List<Order> OrderList { get; set; }
        public List<OrderDetail> OrderDetailList { get; set; }
        public int UId { get; }
        public int SMTotal { get; }
    }
    //public class SCPtoOrderModel : POrderAllModel
    //{
        
    //}
}
