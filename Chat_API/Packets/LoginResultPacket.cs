using System;

namespace ChatAPI.Packets
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
