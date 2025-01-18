using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class AddSizeResponseModel
    {
        public AddSizeResponseModel()
        {
            message = "";
            Value = "";
        }
        public int Id { get; set; }
        public string Value { get; set; } = null!;
        public string message { get; set; }
    }
}
