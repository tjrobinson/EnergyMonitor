using System.Text;
using ConfigR;
using EnergyMonitorAgent.Model;
using Microsoft.ServiceBus.Messaging;
using Newtonsoft.Json;

namespace EnergyMonitorAgent
{
	public class EventHubWriter
	{
		private readonly EventHubClient eventHubClient;

		public EventHubWriter()
		{
			string eventHubName = Config.Global.Get<string>("EventHubName");
			string eventHubConnectionString = Config.Global.Get<string>("EventHubConnectionString");

			eventHubClient = EventHubClient.CreateFromConnectionString(eventHubConnectionString, eventHubName);
		}

		public void SendToEventHub(EnergyReading e)
		{
			eventHubClient.Send(new EventData(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(e))));
		}
	}
}