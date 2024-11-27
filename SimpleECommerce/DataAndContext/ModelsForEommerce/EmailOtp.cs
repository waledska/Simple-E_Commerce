using System;
using System.Collections.Generic;

namespace SimpleECommerce.DataAndContext.ModelsForEommerce
{
    public partial class EmailOtp
    {
        public int Id { get; set; }
        public string Email { get; set; } = null!;
        public string Otp { get; set; } = null!;
        public DateTime ValidTo { get; set; }
    }
}
