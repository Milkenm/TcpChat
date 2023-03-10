using System;

namespace Chat_API.Packets
{
	[Serializable]
	public class LoginResultPacket : IPacket
	{
		public bool Success { get; }
		public string ErrorMessage { get; }

		public LoginResultPacket(bool success, string errorMessage)
		{
			Success = success;
			ErrorMessage = errorMessage;
		}
	}
}
