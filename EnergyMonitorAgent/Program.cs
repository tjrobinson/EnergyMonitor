using ConfigR;
using EnergyMonitorAgent.Model;
using Topshelf;

namespace EnergyMonitorAgent
{
	class Program
	{
		static void Main(string[] args)
		{
			HostFactory.Run(x => 
			{
				x.Service<EnergyReaderHost>(s => 
				{
					s.ConstructUsing(name => new EnergyReaderHost(new FakeEnergyReader())); // EnergyReader("COM5");
					s.WhenStarted(tc => tc.Start());
					s.WhenStopped(tc => tc.Stop());
				});
				x.RunAsLocalSystem();
				x.SetDescription("Energy Monitor");
				x.SetDisplayName("EnergyMonitor");
				x.SetServiceName("EnergyMonitor");
			});
		}
	}
}
