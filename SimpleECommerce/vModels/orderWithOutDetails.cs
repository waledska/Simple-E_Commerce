using System.ComponentModel.DataAnnotations;
using SimpleECommerce.DataAndContext.ModelsForEommerce;

namespace SimpleECommerce.vModels
{
    public class orderWithOutDetails
    {
        public orderWithOutDetails()
        {
            OrderAddress = new Address();
        }
        public int orderId { get; set; }
        public string? UserId { get; set; }
        public string OrderStatus { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime DateOfOrder { get; set; }
        public virtual Address OrderAddress { get; set; } = null!;
    }
}
