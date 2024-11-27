using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class Product
    {
        public Product()
        {
            ProductVariations = new HashSet<ProductVariation>();
        }

        public int Id { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; } = null!;
        public string? Description { get; set; }
        public bool isDeleted { get; set; }
        public int categoryId { get; set; }

        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductVariation> ProductVariations { get; set; }
    }
}
