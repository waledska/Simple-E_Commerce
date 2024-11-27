using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class Order
    {
        public Order()
        {
            OrderRows = new HashSet<OrderRow>();
        }

        public int Id { get; set; }
        public string? UserId { get; set; }
        public string OrderStatus { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime DateOfOrder { get; set; }
        public int AddressId { get; set; }

        public virtual Address Address { get; set; } = null!;
        public virtual ICollection<OrderRow> OrderRows { get; set; }
    }
}
