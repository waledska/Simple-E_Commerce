using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class Cart
    {
        public Cart()
        {
            CartRows = new HashSet<CartRow>();
        }

        public int Id { get; set; }
        public string UserId { get; set; } = null!;

        public virtual ICollection<CartRow> CartRows { get; set; }
    }
}
