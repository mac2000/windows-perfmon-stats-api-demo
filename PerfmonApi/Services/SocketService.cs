using System.Collections.Generic;
using Microsoft.Web.WebSockets;
using Newtonsoft.Json;

namespace PerfmonApi.Services
{
	public class SocketService : WebSocketHandler
	{
		private static readonly WebSocketCollection Clients = new WebSocketCollection();

		public override void OnOpen() => Clients.Add(this);

		//public override void OnMessage(string message) => Clients.Broadcast(message);

		public static void SendStats(Dictionary<string, float> stats) => SendMessage(JsonConvert.SerializeObject(stats));
		private static void SendMessage(string message) => Clients.Broadcast(message);
	}
}