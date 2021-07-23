using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace WebSockets.SocketsManager
{
    public abstract class SocketHandler
    {
        public ConnectionManager Connection { get; set; }

        public SocketHandler(ConnectionManager connection)
        {
            Connection = connection;
        }

        public virtual async Task OnConnected(WebSocket sockets)
        {
            await Task.Run(() => { Connection.AddSockets(sockets); });
        }

        public virtual async Task OnDissonnected(WebSocket socket)
        {
            await Connection.RemoveSocketsAsync(Connection.GetId(socket));
        }

        public async Task SendMessage(WebSocket socket, string message)
        {
            if (socket.State != WebSocketState.Open)
                return;
            await socket.SendAsync(new ArraySegment<byte>(Encoding.ASCII.GetBytes(message), 0, message.Length),
                WebSocketMessageType.Text, true, CancellationToken.None);
        }

        public async Task SendMessage(string id, string message)
        {
            await SendMessage(Connection.GetSocketById(id), message);
        }

        public async Task SendMessageToAll(string message)
        {
            foreach (var con in Connection.GetAllConnections())
                await SendMessage(con.Value, message);
        }

        public abstract Task Receive(WebSocket socket, WebSocketReceiveResult result, byte[] buffer);

    }
}
