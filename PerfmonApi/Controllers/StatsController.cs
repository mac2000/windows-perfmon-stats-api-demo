using System.Web;
using System.Web.Http;

namespace PerfmonApi.Controllers
{
	public class StatsController : ApiController
	{
		[HttpGet]
		[Route(nameof(Stats))]
		public object Stats() => HttpContext.Current.Application["_values"];
	}
}
