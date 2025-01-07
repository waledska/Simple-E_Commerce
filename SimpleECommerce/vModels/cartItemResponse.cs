using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class cartItemResponse
    {
        public cartItemResponse()
        {
            colorValue = String.Empty;
            sizeValue = String.Empty;
            mainVarPhoto = String.Empty;
            productName = String.Empty;
            productDescription = String.Empty;
        }
        // cart row data
        public int cartRowId { get; set; }
        public int QuantityOfVarInCart { get; set; }
        // variaiton data
        public int variaitonId { get; set; }
        public bool isVariationDeleted { get; set; }
        public string mainVarPhoto { get; set; }
        public string colorValue { get; set; }
        public string sizeValue { get; set; }
        // product data
        public int productId { get; set; }
        public string productName { get; set; } = null!;
        public decimal productPrice { get; set; }
        public string productDescription { get; set; } = null!;
    }
}
