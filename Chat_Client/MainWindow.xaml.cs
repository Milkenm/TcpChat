using System.Net;
using System.Windows;

using Chat_API.Packets;

using ScriptsLibV2;

using static ScriptsLibV2.TcpClient;

namespace Chat_Client
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private TcpClient Client = new TcpClient();
		private string Username;

		public MainWindow()
		{
			InitializeComponent();

		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			Client.OnDataReceived += this.Client_OnDataReceived;
		}

		private void Client_OnDataReceived(EndPoint source, byte[] data)
		{
			MessageBox.Show("Packet received.");
			IPacket packet = ScriptsLibV2.Extensions.ByteExtensions.ToObject<IPacket>(data);
			if (packet is ServerMessagePacket serverMessagePacket)
			{
				Dispatcher.Invoke(() =>
				{
					listBox_chat.Items.Add($"{serverMessagePacket.Username}: {serverMessagePacket.Message}");
				});
			}
		}

		private void button_send_Click(object sender, RoutedEventArgs e)
		{
			string message = textBox_message.Text;
			if (string.IsNullOrEmpty(message)) { return; }

			ClientMessagePacket messagePacket = new ClientMessagePacket(message);
			Client.Send(messagePacket);

			listBox_chat.Items.Add($"{Username}: {message}");
			textBox_message.Text = string.Empty;
			textBox_message.Focus();
		}

		private void button_login_Click(object sender, RoutedEventArgs e)
		{
			Client.Connect(IPAddress.Loopback, 4725);

			Username = textBox_username.Text;
			if (string.IsNullOrEmpty(Username)) { return; }

			LoginPacket loginPacket = new LoginPacket(Username);
			Client.Send(loginPacket, new DataCallbackEvent((data) =>
			{
				IPacket packet = (IPacket)data;
				if (packet is LoginResultPacket loginResult)
				{
					if (loginResult.Success)
					{
						MessageBox.Show("Logged in successfully.");
						Dispatcher.Invoke(() =>
						{
							textBox_username.IsEnabled = false;
							button_login.IsEnabled = false;
						});
					}
					else
					{
						MessageBox.Show("There was a problem logging in.");
					}
				}
			}), true);
		}
	}
}
