using System.Web.Http;

namespace PerfmonApi.Controllers
{
	public class PingController : ApiController
	{
		[HttpGet]
		[Route("ping")]
		public string Ping() => "pong";
	}
}
