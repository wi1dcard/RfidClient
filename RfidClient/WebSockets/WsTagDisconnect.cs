using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebSocketSharp;
using WebSocketSharp.Server;

namespace RfidClient
{
	public class WsTagDisconnect : WebSocketBehavior
	{
		protected override void OnMessage(MessageEventArgs e)
		{
			//Sessions.Broadcast("haha" + e.Data);
		}
	}
}
