﻿using System.ComponentModel.DataAnnotations;

namespace SimpleECommerce.vModels
{
    public class ProductVariationResponseModel
    {
        public int Id { get; set; }
        public string Color { get; set; } = null!;
        public string Size { get; set; } = null!;
        public bool isDeleted { get; set; }
        public string mainVariationPhotoPath { get; set; } = null!;
        public List<string> ImagesPathes { get; set; } = new List<string>();
        public int QuantityInStock { get; set; }
        public string? Sku { get; set; }
        public string message { get; set; } = null!;
    }
}
