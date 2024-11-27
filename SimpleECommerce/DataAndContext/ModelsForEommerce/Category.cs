using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class Category
    {
        public Category()
        {
            Products = new HashSet<Product>();
        }

        public int Id { get; set; }
        public string Value { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
