using System.Net;
using System.Windows;
using System.Windows.Controls;

using Chat_API.Packets;

using static ScriptsLibV2.TcpClient;

namespace Chat_Client.Pages
{
	/// <summary>
	/// Interaction logic for LoginPage.xaml
	/// </summary>
	public partial class LoginPage : Page
	{
		public LoginPage()
		{
			InitializeComponent();
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			try
			{
				NetworkClient.Client.Connect(IPAddress.Loopback, 4725);


				NetworkClient.Client.Send<ServerStatusPacket>(new RequestServerStatusPacket(), new DataReceivedCallback<ServerStatusPacket>((statusPacket) =>
				{
					label_status.Content = "Server Status: " + statusPacket.Status.ToString();
				}), true);
				label_status.Content = "Server Status: Online";
			}
			catch
			{
				label_status.Content = "Server Status: Offline";
			}
		}
	}
}
