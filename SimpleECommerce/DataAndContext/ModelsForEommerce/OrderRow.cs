using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class OrderRow
    {
        public int Id { get; set; }
        public int OrderId { get; set; }
        public int ProductVariationId { get; set; }
        public int Quantity { get; set; }
        public decimal PriceForProduct { get; set; }
        public virtual Order Order { get; set; } = null!;
        public virtual ProductVariation ProductVariation { get; set; } = null!;
    }
}
