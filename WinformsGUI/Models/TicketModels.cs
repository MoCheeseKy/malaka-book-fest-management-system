namespace WinformsGUI.Models
{
    public class TicketPriceResponse
    {
        public string TicketType { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }

    public class TicketResponse
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public string TicketType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public DateTime PurchaseDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime? ScanDate { get; set; }
    }

    public class ScanTicketRequest
    {
        public Guid TicketId { get; set; }
    }
}
