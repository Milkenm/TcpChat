using System.Net;

namespace Chat_API
{
	public static class Settings
	{
		public static IPAddress ServerAddress = IPAddress.Loopback; // Dns.GetHostAddresses("rtx.milkenm.net")[0];
		public static short ServerPort = 666; // 4725
	}
}