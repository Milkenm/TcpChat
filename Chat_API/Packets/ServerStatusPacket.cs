using System;

namespace Chat_API.Packets
{
	[Serializable]
	public class ServerStatusPacket : IPacket
	{
		public EServerStatus Status { get; }

		public ServerStatusPacket(EServerStatus status)
		{
			Status = status;
		}
	}
}
