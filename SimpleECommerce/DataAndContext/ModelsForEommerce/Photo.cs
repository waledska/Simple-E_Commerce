using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class Photo
    {
        public int Id { get; set; }
        public string Path { get; set; } = null!;
        public int ProductVariationId { get; set; } // Foreign key

        public virtual ProductVariation ProductVariation { get; set; } = null!;
    }
}
