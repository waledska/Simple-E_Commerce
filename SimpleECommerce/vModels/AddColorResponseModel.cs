using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class AddColorResponseModel
    {
        public AddColorResponseModel()
        {
            message = "";
            Value = "";
        }
        public int Id { get; set; }
        public string Value { get; set; } = null!;
        public string message { get; set; }
    }
}
