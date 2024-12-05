using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class updateOrderStatus
    {
        public updateOrderStatus()
        {
            orderStatus = "";
        }

        [Required]
        public int orderId { get; set; }
        [Required]
        public string orderStatus { get; set; }
    }
}
