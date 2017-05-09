using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Microsoft.Web.WebSockets;
using PerfmonApi.Services;

namespace PerfmonApi.Controllers
{
	public class SocketController : ApiController
	{
		[HttpGet]
		[Route("socket")]
		public HttpResponseMessage Get()
		{
			HttpContext.Current.AcceptWebSocketRequest(new SocketService());
			return Request.CreateResponse(HttpStatusCode.SwitchingProtocols);
		}
	}
}
