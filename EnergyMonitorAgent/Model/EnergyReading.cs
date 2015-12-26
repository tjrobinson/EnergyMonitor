using System;

namespace EnergyMonitorAgent.Model
{
	public class EnergyReading
	{
		public float Energy { get; set; }

		public float Temperature { get; set; }

		public DateTime TimeStamp { get; set; }

		public bool IsValid { get; set; }

		public string RawData { get; set; }

		public string Source { get; set; }

		public override string ToString()
		{
			return TimeStamp.ToLongTimeString() + " - " + Energy.ToString("N") + " Watts - " + Temperature.ToString("N") + " Degrees";
		}
	}
}