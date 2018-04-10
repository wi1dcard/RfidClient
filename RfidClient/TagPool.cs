using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RfidClient
{
	public class TagEventArgs : EventArgs
	{
		public List<Tag> Tags;
	}

	public class TagPool : Dictionary<string, Tag>
	{
		public EventHandler<TagEventArgs> OnConnected;

		public EventHandler<TagEventArgs> OnDisconnected;

		public void Throw(List<byte[]> epcList)
		{
			foreach (var epcBytes in epcList)
			{
				var epcString = Tag.ByteArrayToString(epcBytes);
				if (!base.ContainsKey(epcString))
				{
					base[epcString] = new Tag()
					{
						epc = epcString
					};
				}
				base[epcString].Tick();
			}
		}

		private void InvokeEvent(EventHandler<TagEventArgs> eventHandler, KeyValuePair<string, Tag>[] kvTags, Action<KeyValuePair<string, Tag>> enumAction)
		{
			var tagList = new List<Tag>();
			foreach (var kv in kvTags)
			{
				tagList.Add(kv.Value);
				enumAction(kv);
			}

			if (tagList.Count > 0)
			{
				eventHandler?.Invoke(this, new TagEventArgs()
				{
					Tags = tagList
				});
			}
		}

		public void Check(int connectTicks, int disconnectTicks)
		{
			var disconnectedTags = this.Where(
				(kv) => Tag.currentTick - kv.Value.lastTick >= disconnectTicks
			).ToArray();
			this.InvokeEvent(this.OnDisconnected, disconnectedTags, (kv) => base.Remove(kv.Key));

			var connectedTags = this.Where(
				(kv) => !kv.Value.hasBroadcasted && Tag.currentTick - kv.Value.firstTick >= connectTicks
			).ToArray();
			this.InvokeEvent(this.OnConnected, connectedTags, (kv)=> kv.Value.hasBroadcasted = true);
		}
	}
}
