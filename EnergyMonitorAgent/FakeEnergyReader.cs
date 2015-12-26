using System;
using System.Timers;
using EnergyMonitorAgent.Model;

namespace EnergyMonitorAgent
{
	public class FakeEnergyReader : IEnergyReader
	{
		protected virtual void OnReadingReceived(EnergyReading rdg)
		{
			var handler = this.ReadingReceived;
			if (handler != null)
			{
				handler(this, rdg);
			}
		}

		public FakeEnergyReader()
		{
			timer = new Timer();
		}

		private readonly Timer timer;
		private readonly Random random = new Random();

		public void Start()
		{
			timer.Interval = 1000;
			timer.Elapsed += Timer_Elapsed;
			timer.AutoReset = true;
			timer.Start();
		}

		private void Timer_Elapsed(object sender, ElapsedEventArgs e)
		{
			var energyReading = new EnergyReading
			{
				Energy = random.Next(19, 21),
				RawData = string.Empty,
				TimeStamp = DateTime.UtcNow,
				Source = "fake",
				IsValid = true
			};

			OnReadingReceived(energyReading);
		}

		public event EventHandler<EnergyReading> ReadingReceived;
	}
}