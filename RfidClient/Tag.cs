using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace RfidClient
{
	public class Tag
	{
		[JsonIgnore]
		public long firstTick { get; private set; }

		[JsonIgnore]
		public long lastTick { get; private set; }

		[JsonIgnore]
		public static long currentTick
		{
			get
			{
				return Environment.TickCount;
			}
		}

		public bool hasBroadcasted { get; set; }

		public string epc { get; set; }

		public Tag()
		{
			this.firstTick = Tag.currentTick;
			this.Tick();
		}

		public void Tick()
		{
			this.lastTick = Tag.currentTick;
		}

		public static string ByteArrayToString(byte[] b)
		{
			return BitConverter.ToString(b).Replace("-", "");
		}
	}
}
