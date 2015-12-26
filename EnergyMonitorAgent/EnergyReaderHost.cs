using EnergyMonitorAgent.Model;

namespace EnergyMonitorAgent
{
	using System;
	using System.Threading.Tasks;

	public class EnergyReaderHost
	{
		private readonly IEnergyReader energyReader;
		private readonly EventHubWriter eventHubWriter;

		public EnergyReaderHost(IEnergyReader energyReader)
		{
			this.energyReader = energyReader;
			energyReader.ReadingReceived += energyReader_ReadingReceived;

			eventHubWriter = new EventHubWriter();
		}

		public void Start()
		{
			Task.Run(() => energyReader.Start());
		}

		public void Stop() { }

		void energyReader_ReadingReceived(object sender, EnergyReading energyReading)
		{
			string rawData = energyReading.RawData;
			if (!string.IsNullOrWhiteSpace(rawData) && rawData.Length >= 40)
			{
				rawData = rawData.Substring(0, 40);
			}

			Console.WriteLine("{0} {1} {2} {3}", energyReading.TimeStamp, energyReading.Temperature, energyReading.Energy, rawData);
			eventHubWriter.SendToEventHub(energyReading);
		}
	}
}
