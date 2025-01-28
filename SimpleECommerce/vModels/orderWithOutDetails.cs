using System.ComponentModel.DataAnnotations;
using SimpleECommerce.DataAndContext.ModelsForEommerce;

namespace SimpleECommerce.vModels
{
    public class orderWithOutDetails
    {
        public orderWithOutDetails()
        {
            OrderAddress = new simpleAddressDetails();
        }
        public int orderId { get; set; }
        public string? UserId { get; set; }
        public string OrderStatus { get; set; } = null!;
        public decimal TotalAmount { get; set; }
        public DateTime DateOfOrder { get; set; }
        public virtual simpleAddressDetails OrderAddress { get; set; } = null!;
    }
    public class simpleAddressDetails
    {
        public int addressId { get; set; }
        public string? country { get; set; }
        public string? city { get; set; } = null!;
        public string? streetName { get; set; }
    }
}
