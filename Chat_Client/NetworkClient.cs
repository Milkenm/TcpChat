using System.Windows;

using Chat_API.Packets;

using ScriptsLibV2;

using static ScriptsLibV2.TcpClient;

namespace Chat_Client
{
	public class NetworkClient
	{
		public static TcpClient Client = new TcpClient();
	}
}
