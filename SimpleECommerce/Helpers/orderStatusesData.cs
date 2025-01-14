namespace SimpleECommerce.Helpers
{
    public class orderStatusesData
    {
        public string Pending { get; set; } = null!;
        public string Processing { get; set; } = null!;
        public string Shipped { get; set; } = null!;
        public string Delivered { get; set; } = null!;
        public string Returned { get; set; } = null!;
    }
}