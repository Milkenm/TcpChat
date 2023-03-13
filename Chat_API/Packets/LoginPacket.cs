using System;

namespace ChatAPI.Packets
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
