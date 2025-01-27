﻿using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class CartRow
    {
        public int Id { get; set; }
        public int ProductVariationId { get; set; }
        public int Quantity { get; set; }
        public string UserId { get; set; } = null!;

        public virtual ProductVariation ProductVariation { get; set; } = null!;
    }
}
