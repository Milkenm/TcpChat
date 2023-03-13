using System;

namespace ChatAPI.Packets
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
