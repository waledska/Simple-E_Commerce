using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class ProductVariation
    {
        public ProductVariation()
        {
            CartRows = new HashSet<CartRow>();
            OrderRows = new HashSet<OrderRow>();
            Photos = new HashSet<Photo>();
        }

        public int Id { get; set; }
        public int ProductId { get; set; }
        public int ColorId { get; set; }
        public int SizeId { get; set; }
        public string? Sku { get; set; }
        public int QuantityInStock { get; set; }
        // New property for the main product variation photo
        public string MainProductVariationPhoto { get; set; } = null!;
        public bool isDeleted { get; set; }

        public virtual Color Color { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
        public virtual Size Size { get; set; } = null!;
        public virtual ICollection<CartRow> CartRows { get; set; }
        public virtual ICollection<OrderRow> OrderRows { get; set; }
        public virtual ICollection<Photo> Photos { get; set; }
    }
}
