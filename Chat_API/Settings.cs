using System.Net;

namespace ChatAPI
{
	public static class Settings
	{
		public static IPAddress ServerAddress = IPAddress.Loopback; // Dns.GetHostAddresses("rtx.milkenm.net")[0];
		public static short ServerPort = 666; // 4725

		public static int UsernameMinimumLength = 3;
		public static int UsernameMaximumLength = 12;
		public static string UsernameAllowedCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_0123456789";
	}
}