using System;

namespace ChatAPI.Packets
{
	[Serializable]
	public class ServerMessagePacket : IPacket
	{
		public string Username { get; }
		public string Message { get; }

		public ServerMessagePacket(string username, string message)
		{
			Username = username;
			Message = message;
		}
	}
}
