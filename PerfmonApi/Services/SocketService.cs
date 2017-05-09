using Microsoft.Web.WebSockets;

namespace PerfmonApi.Services
{
	public class SocketService : WebSocketHandler
	{
		private static readonly WebSocketCollection Clients = new WebSocketCollection();

		public override void OnOpen() => Clients.Add(this);

		// public override void OnMessage(string message) => Clients.Broadcast(message);

		public static void SendStats(string message) => Clients.Broadcast(message);
	}
}