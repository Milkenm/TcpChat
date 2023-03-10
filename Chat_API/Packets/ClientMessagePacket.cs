using System;

namespace Chat_API.Packets
{
	[Serializable]
	public class ClientMessagePacket : IPacket
	{
		public string Message { get; }

		public ClientMessagePacket(string message)
		{
			Message = message;
		}
	}
}
