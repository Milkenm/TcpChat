using System;

namespace Chat_API.Packets
{
	[Serializable]
	public class LoginPacket : IPacket
	{
		public string Username;

		public LoginPacket(string username)
		{
			this.Username = username;
		}
	}
}
