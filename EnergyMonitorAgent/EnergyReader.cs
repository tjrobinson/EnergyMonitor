using System.Diagnostics;
using System.Xml;
using EnergyMonitorAgent.Model;

namespace EnergyMonitorAgent
{
	using System;
	using System.IO.Ports;

	public class EnergyReader : IEnergyReader
	{
		public EnergyReader(string portName)
		{
			this.portName = portName;
		}

		private string rxBuffer;

		private SerialPort comm;

		private readonly string portName;

		private readonly int baudRate = 57600;

		private readonly int dataBits = 8;

		private readonly Parity parity = Parity.None;

		private readonly StopBits stopBits = StopBits.One;

		public event EventHandler<EnergyReading> ReadingReceived;

		public void Start()
		{
			this.comm = new SerialPort(this.portName, this.baudRate, this.parity, this.dataBits, this.stopBits);
			this.comm.DataReceived += this.SerialDataReceived;
			this.comm.Open();
		}

		private EnergyReading ExtractReading(ref string receiveBuffer)
		{
			int pos = receiveBuffer.IndexOf(Environment.NewLine, StringComparison.Ordinal);
			var readingXml = receiveBuffer.Substring(0, pos);
			receiveBuffer = receiveBuffer.Substring(pos + 2);

			if (readingXml.Contains("<tmpr>"))
				return ParseAsEnergyReading(readingXml);
			else if (readingXml.Contains("<hist>")) return new HistoryReading(readingXml);
			else return null;
		}

		public EnergyReading ParseAsEnergyReading(string readingData)
		{
			var energyReading = new EnergyReading
			{
				TimeStamp = DateTime.MinValue,
				Energy = float.NaN,
				Temperature = float.NaN,
				IsValid = false,
				Source = "energymonitor"
			};

			try
			{
				// parse the xml based reading
				XmlDocument xDoc = new XmlDocument();
				xDoc.LoadXml(readingData);

				energyReading.RawData = xDoc.OuterXml;

				XmlNode xNode = xDoc.SelectSingleNode("/msg/time");
				energyReading.TimeStamp = XmlConvert.ToDateTime(xNode.InnerText);
				xNode = xDoc.SelectSingleNode("/msg/tmpr");
				if (null == xNode)
				{
					// try the tmprF element instead
					xNode = xDoc.SelectSingleNode("/msg/tmprF");
				}
				if (xNode != null)
					energyReading.Temperature = System.Xml.XmlConvert.ToSingle(xNode.InnerText);
				xNode = xDoc.SelectSingleNode("/msg/ch1/watts");
				if ((null != xNode) && (string.Empty != xNode.InnerText))
				{
					energyReading.Energy = System.Xml.XmlConvert.ToInt32(xNode.InnerText);
				}

				xNode = xDoc.SelectSingleNode("/msg/ch2/watts");
				if ((null != xNode) && (string.Empty != xNode.InnerText))
				{
					energyReading.Energy += System.Xml.XmlConvert.ToInt32(xNode.InnerText);
				}

				xNode = xDoc.SelectSingleNode("/msg/ch3/watts");
				if ((null != xNode) && (string.Empty != xNode.InnerText))
				{
					energyReading.Energy += System.Xml.XmlConvert.ToInt32(xNode.InnerText);
				}

				energyReading.IsValid = true;
			}
			catch (Exception ex)
			{
				// invalid reading
				Debug.WriteLine("Error parsing xml data to create EnergyReading: " + ex.Message);
			}

			return energyReading;
		}

		private bool IsReading(string receiveBuffer)
		{
			// if it has a newline char then it is the end of a reading
			return (receiveBuffer.IndexOf(Environment.NewLine, StringComparison.Ordinal) != -1);
		}

		private void SerialDataReceived(object sender, SerialDataReceivedEventArgs e)
		{
			this.rxBuffer += this.comm.ReadExisting();
			while (this.IsReading(this.rxBuffer))
			{
				// we have a reading (or partial reading), so let's process
				EnergyReading rdg = this.ExtractReading(ref this.rxBuffer);
				if (rdg.IsValid)
				{
					this.OnReadingReceived(rdg);
				}
			}
		}

		protected virtual void OnReadingReceived(EnergyReading rdg)
		{
			var handler = this.ReadingReceived;
			if (handler != null)
			{
				handler(this, rdg);
			}
		}
	}
}