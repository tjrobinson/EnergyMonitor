using System;
using System.Diagnostics;
using System.Xml;

namespace EnergyMonitorAgent.Model
{
	public class HistoryReading : EnergyReading
	{

		public float Energy { get; private set; }
		public float Temperature { get; private set; }


		public HistoryReading(string readingData)
		{
			this.TimeStamp = DateTime.MinValue;
			this.IsValid = false;

			try
			{
				// parse the xml based reading
				XmlDocument xDoc = new XmlDocument();
				xDoc.LoadXml(readingData);

				this.RawData = xDoc.OuterXml;

				XmlNode xNode = xDoc.SelectSingleNode("/msg/time");
				this.TimeStamp = System.Xml.XmlConvert.ToDateTime(xNode.InnerText);
				xNode = xDoc.SelectSingleNode("/msg/tmpr");
				if (null == xNode)
				{
					// try the tmprF element instead
					xNode = xDoc.SelectSingleNode("/msg/tmprF");
				}
				if (xNode != null)
						this.Temperature = System.Xml.XmlConvert.ToSingle(xNode.InnerText);
				xNode = xDoc.SelectSingleNode("/msg/ch1/watts");
				if ((null != xNode) && (string.Empty != xNode.InnerText))
				{
					this.Energy = System.Xml.XmlConvert.ToInt32(xNode.InnerText);
				}

				xNode = xDoc.SelectSingleNode("/msg/ch2/watts");
				if ((null != xNode) && (string.Empty != xNode.InnerText))
				{
					this.Energy += System.Xml.XmlConvert.ToInt32(xNode.InnerText);
				}

				xNode = xDoc.SelectSingleNode("/msg/ch3/watts");
				if ((null != xNode) && (string.Empty != xNode.InnerText))
				{
					this.Energy += System.Xml.XmlConvert.ToInt32(xNode.InnerText);
				}

				this.IsValid = true;
			}
			catch (Exception ex)
			{
				// invalid reading
				Debug.WriteLine("Error parsing xml data to create HistoryReading: " + ex.Message);
			}
		}

		public override string ToString()
		{
			return this.TimeStamp.ToLongTimeString() + " - " + this.Energy.ToString("N") + " Watts - " + this.Temperature.ToString("N") + " Degrees";
		}
	}
}