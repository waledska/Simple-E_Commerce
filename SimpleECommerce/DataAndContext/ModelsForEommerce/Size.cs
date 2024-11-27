using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class Size
    {
        public Size()
        {
            ProductVariations = new HashSet<ProductVariation>();
        }

        public int Id { get; set; }
        public string Value { get; set; } = null!;

        public virtual ICollection<ProductVariation> ProductVariations { get; set; }
    }
}
