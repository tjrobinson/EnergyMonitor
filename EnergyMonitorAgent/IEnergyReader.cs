using System;
using EnergyMonitorAgent.Model;

namespace EnergyMonitorAgent
{
	public interface IEnergyReader
	{
		event EventHandler<EnergyReading> ReadingReceived;

		void Start();
	}
}