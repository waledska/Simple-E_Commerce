using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class Address
    {
        public Address()
        {
            Orders = new HashSet<Order>();
        }

        public int Id { get; set; }
        public string Country { get; set; } = null!;
        public string FullName { get; set; } = null!;
        public string MobileNumber { get; set; } = null!;
        public string StreetName { get; set; } = null!;
        public string BuildingName { get; set; } = null!;
        public string? City { get; set; }
        public string UserId { get; set; } = null!;
        public bool IsDefault { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
