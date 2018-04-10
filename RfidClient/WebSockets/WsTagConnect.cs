using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RfidClient
{
	public class WsTagConnect : WebSocketBehavior
	{
		public WsTagConnect()
		{
			//System.Diagnostics.Debug.WriteLine("构造");
		}
		protected override void OnMessage(MessageEventArgs e)
		{
			//Sessions.Broadcast("con" + e.Data);
		}
	}
}
