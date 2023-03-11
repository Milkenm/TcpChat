using System;

namespace Chat_API.Packets
{
	[Serializable]
	public class LoginResultPacket : IPacket
	{
		public ELoginResult Result;

		public LoginResultPacket(ELoginResult result)
		{
			Result = result;
		}
	}
}
