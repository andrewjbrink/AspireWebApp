using Microsoft.AspNetCore.SignalR;

namespace MudBlazorWebApp.Services;


public class SalesHub : Hub
{
    public async Task SendSales(object sales)
    {
        await Clients.All.SendAsync("ReceiveSales", sales);
    }
}
