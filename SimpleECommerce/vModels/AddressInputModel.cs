using System.ComponentModel.DataAnnotations;
namespace SimpleECommerce.vModels
{
    public class AddressInputModel
    {
        public int Id { get; set; } // Only needed for Update operation
        public string BuildingName { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public bool IsDefault { get; set; }
        [Required]
        public string MobileNumber { get; set; }
        [Required]
        public string StreetName { get; set; }
    }
}
