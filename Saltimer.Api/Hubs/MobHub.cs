namespace Saltimer.Api.Hubs;

using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

public class MobHub : Hub
{
    public async Task SetTime(string message)
    {
        await Clients.All.SendAsync("newMessage", "anonymous", message);
    }
}