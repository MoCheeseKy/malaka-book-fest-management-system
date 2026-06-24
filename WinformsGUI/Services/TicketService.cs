using WinformsGUI.Models;
using WinformsGUI.Utils;

namespace WinformsGUI.Services
{
    public class TicketService
    {
        public async Task<List<TicketPriceResponse>?> GetTicketPricesAsync()
        {
            return await ApiClient.GetAsync<List<TicketPriceResponse>>("Ticket/prices");
        }

        public async Task<List<TicketResponse>?> GetMyTicketsAsync()
        {
            // Note: Since this is admin side, we might just be viewing "my" tickets if admin is a user
            // or if there's an admin endpoint to get all tickets, we'd use that. 
            // For now, mapping to what's available in Swagger
            return await ApiClient.GetAsync<List<TicketResponse>>("Ticket/my");
        }

        public async Task<string> ScanTicketAsync(ScanTicketRequest request)
        {
            try
            {
                // The scan endpoint doesn't return a complex object in typical scenarios,
                // we'll just return a success string or let EnsureSuccessStatusCode handle failures.
                await ApiClient.PostAsync<ScanTicketRequest, object>("Ticket/scan", request);
                return "Success";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
