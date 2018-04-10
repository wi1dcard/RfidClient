using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using UHFReader.Readers;
using WebSocketSharp.Server;

namespace RfidClient
{
	public partial class MainForm : Form
	{
		/** Members Begin **/
		public TagPool tagPool;
		public UHFReader.Reader reader;
		public WebSocketServer webSocket;
		/** Members End **/

		/** Constants Begin **/
		public const string wsServer = "ws://0.0.0.0";
		public const string wsTagPool = "/tag-pool";
		public const string wsTagConnect = "/tag-connect";
		public const string wsTagDisconnect = "/tag-disconnect";
		public const int poolInterval = 1000;
		public const int readInterval = 100;
		public const int connectTicks = 0;
		public const int disconnectTicks = 2000;
		/** Constants End **/

		public MainForm()
		{
			InitializeComponent();
		}

		private void MainForm_Load(object sender, EventArgs e)
		{
			//启动Websocket服务器
			try
			{
				this.webSocket = new WebSocketServer(wsServer);
				webSocket.AddWebSocketService<WsTagConnect>(wsTagConnect);
				webSocket.AddWebSocketService<WsTagDisconnect>(wsTagDisconnect);
				webSocket.AddWebSocketService<WsTagPool>(wsTagPool);
				webSocket.Start();
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error - Start Websocket Server");
				Application.Exit();
				return;
			}

			//获取ServiceHosts
			WebSocketServiceHost hostTagConnect;
			WebSocketServiceHost hostTagDisconnect;
			if (!webSocket.WebSocketServices.TryGetServiceHost(wsTagConnect, out hostTagConnect) ||
				!webSocket.WebSocketServices.TryGetServiceHost(wsTagDisconnect, out hostTagDisconnect))
			{
				MessageBox.Show("Please restart and try again.", "Error - Get Websocket ServiceHost");
				Application.Exit();
				return;
			}

			//挂载Tag Connect、Disconnect
			this.tagPool = new TagPool();
			this.tagPool.OnConnected += (tp, te) => wsBroadcast(te.Tags, hostTagConnect);
			this.tagPool.OnDisconnected += (tp, te) => wsBroadcast(te.Tags, hostTagDisconnect);

			//尝试连接读卡设备
			this.notifyIcon.Icon = Properties.Resources.IconWarning;
			btnConnect_Click(sender, e);
		}

		private void wsBroadcast(object obj, WebSocketServiceHost host)
		{
			var json = JToken.FromObject(obj);
			var str = json.ToString(Newtonsoft.Json.Formatting.None);
			host.Sessions.BroadcastAsync(str, null);
		}

		private void btnConnect_Click(object sender, EventArgs e)
		{
			try
			{
				this.reader = new ComReader(5);
				//this.reader = new NetReader(new System.Net.IPEndPoint(System.Net.IPAddress.Parse("192.168.50.52"), 6000));
				this.btnConnect.Enabled = false;
				this.notifyIcon.Icon = Properties.Resources.IconOK;
				this.Hide();
				timerRead.Interval = readInterval;
				timerRead.Enabled = true;
				timerPool.Interval = poolInterval;
				timerPool.Enabled = true;
			}
			catch (Exception ex)
			{
				MessageBox.Show(ex.Message, "Error - Connect to Reader");
			}
		}

		private void timerRead_Tick(object sender, EventArgs e)
		{
			//Task.Factory.StartNew(() => {
				List<byte[]> epcList;
				try
				{
					epcList = reader.Inventory_G2(0, 0, 0);
				}
				catch
				{
					return;
				}
				tagPool.Throw(epcList);
				tagPool.Check(connectTicks, disconnectTicks);
			//});
		}

		private void timerPool_Tick(object sender, EventArgs e)
		{
			WebSocketServiceHost hostTagPool;
			if (webSocket.WebSocketServices.TryGetServiceHost(wsTagPool, out hostTagPool))
			{
				wsBroadcast(tagPool.Values, hostTagPool);
			}
		}

		private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
		{
			if (e.CloseReason == CloseReason.UserClosing)
			{
				e.Cancel = true;
				this.Hide();
			}
		}

		private void btnOpen_Click(object sender, EventArgs e)
		{
			this.Show();
		}

		private void btnExit_Click(object sender, EventArgs e)
		{
			Application.Exit();
		}

		private void notifyIcon_MouseDoubleClick(object sender, MouseEventArgs e)
		{
			this.btnOpen_Click(sender, e);
		}
	}
}
