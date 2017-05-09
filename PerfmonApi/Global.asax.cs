using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Formatting;
using System.Timers;
using System.Web;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PerfmonApi.Services;

namespace PerfmonApi
{
	public class Global : HttpApplication
	{
		private readonly int _max = 60;
		private readonly Timer _timer = new Timer();
		private readonly ConcurrentDictionary<string, PerformanceCounter> _counters = new ConcurrentDictionary<string, PerformanceCounter>();
		private readonly ConcurrentDictionary<string, ConcurrentQueue<float>> _values = new ConcurrentDictionary<string, ConcurrentQueue<float>>();

		protected void Application_Start(object sender, EventArgs e)
		{
			Add("cpu", new PerformanceCounter("Processor", "% Processor Time", "_Total"));
			Add("ram", new PerformanceCounter("Memory", "% Committed Bytes In Use"));
			Add("hdd", new PerformanceCounter("PhysicalDisk", "% Disk Time", "_Total"));

			_timer.Interval = 1000;
			_timer.Elapsed += CollectValues;
			_timer.Start();

			HttpContext.Current.Application["_values"] = _values;

			GlobalConfiguration.Configure(config =>
			{
				config.Formatters.OfType<JsonMediaTypeFormatter>().First().SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
				config.MapHttpAttributeRoutes();
			});
		}

		private void Add(string name, PerformanceCounter counter)
		{
			_counters.GetOrAdd(name, key => counter);
			_values.GetOrAdd(name, new ConcurrentQueue<float>());
		}

		private void CollectValues(object sender, ElapsedEventArgs e)
		{
			var stats = new Dictionary<string, float>();
			foreach (var key in _counters.Keys)
			{
				var value = _counters[key].NextValue();
				_values[key].Enqueue(value);
				lock (_values[key])
				{
					while (_values[key].Count > _max && _values[key].TryDequeue(out float overflow))
					{
					}
				}
				stats.Add(key, value);
			}

			SocketService.SendStats(stats);
		}

		protected void Application_End(object sender, EventArgs e)
		{
			_timer.Dispose();

			foreach (var key in _counters.Keys)
			{
				_counters[key].Dispose();
			}
		}
	}
}