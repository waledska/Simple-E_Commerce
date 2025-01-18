namespace SimpleECommerce.Helpers
{
    public class orderStatuses
    {
        public string? pending { get; set; }
        public string? processing { get; set; }
        public string? shipped { get; set; }
        public string? delivered { get; set; }
        public string? cancelled { get; set; }
        public string? returned { get; set; }
    }
}