using Microsoft.AspNetCore.SignalR;

namespace FlashSocket.Web.Hubs
{
    public sealed class ChatHub : Hub<IChatHub>
    {
        private static Dictionary<string, List<string>> _connections = new();
        public override async Task OnConnectedAsync()
        {
			Console.WriteLine("new Connection was noticed..");
            var context = Context.GetHttpContext();
            var cont = Context;
            var connectionId = Context.ConnectionId;
            var groups = Groups;
            var clients = Clients;
            await Clients.All.ReceiveMessage("System", $"{Context.ConnectionId} connected");
        }
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.ReceiveMessage(user, message);
        }

        public async Task SendPrivateMessage(string userId, string sender, string message)
        {
            await Clients.Caller.ReceiveMessage(sender + " test", $"caller is {sender} we know what do you did.");
            await Clients.Client(userId).ReceiveMessage(sender, message + "1");
            await Groups.AddToGroupAsync(userId, "MyGroup");

            // Connect to group
            if (_connections.TryGetValue(Context.ConnectionId, out List<string>? value))
            {
                value.Add(userId);
            }
            else
            {
                _connections.Add(Context.ConnectionId, [userId]);
            }

        }

        public async Task SendToMyGroup(string sender, string message)
        {
            await Clients.Group("MyGroup").ReceiveMessage(sender, message + "1");
            await Clients.Clients(_connections[Context.ConnectionId]).ReceiveMessage(sender, message + "2");
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            await Clients.All.ReceiveMessage("System", $"{Context.ConnectionId} disconnected");
            _connections.Remove(Context.ConnectionId);
        }
    }

    public interface IChatHub
    {
        Task ReceiveMessage(string user, string message);
    }
}
