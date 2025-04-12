using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace AnastasiiaPortfolio.Hubs
{
    public class CrmHub : Hub
    {
        public async Task UpdateDashboard(string data)
        {
            await Clients.All.SendAsync("ReceiveDashboardUpdate", data);
        }

        public async Task UpdateClientData(string clientId, string data)
        {
            await Clients.All.SendAsync("ReceiveClientUpdate", clientId, data);
        }

        public async Task NotifyClientActivity(string clientId, string activity)
        {
            await Clients.All.SendAsync("ReceiveClientActivity", clientId, activity);
        }
    }
} 