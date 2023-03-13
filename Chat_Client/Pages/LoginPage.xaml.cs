using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using ChatAPI;
using ChatAPI.Packets;

using ScriptsLibV2.Extensions;

using static ScriptsLibV2.TcpClient;

namespace ChatClient.Pages
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
			Client.GetInstance().SetTitle(null);

			Task.Factory.StartNew(() =>
			{
				SetLoginPanelStatus(false);

				try
				{
					ChatClientInfo.TcpClient.Connect(Settings.ServerAddress, Settings.ServerPort);
					ChatClientInfo.TcpClient.Send(new RequestServerStatusPacket(), new DataReceivedCallback<ServerStatusPacket>((statusPacket) =>
					{
						SetStatusLabel(statusPacket.Status);

						if (statusPacket.Status == EServerStatus.ONLINE)
						{
							SetLoginPanelStatus(true);
						}
					}), true);
				}
				catch
				{
					SetStatusLabel(EServerStatus.OFFLINE);
				}
			});
		}

		private void SetLoginPanelStatus(bool isEnabled)
		{
			Dispatcher.Invoke(() =>
			{
				textBox_username.IsEnabled = isEnabled;
				button_login.IsEnabled = isEnabled;
			});
		}

		private void SetStatusLabel(EServerStatus status)
		{
			Dispatcher.Invoke(() => label_status.Content = $"Server Status: {status.ToString().Capitalize()}");
		}

		private void button_login_Click(object sender, RoutedEventArgs e)
		{
			Login();
		}

		private void textBox_username_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				Login();
			}
		}

		private void Login()
		{
			string username = textBox_username.Text;

			if (username.IsEmpty())
			{
				MessageBox.Show("You must choose a username!");
				return;
			}

			ChatClientInfo.TcpClient.Send(new LoginPacket(username), new DataReceivedCallback<LoginResultPacket>((loginResult) =>
			{
				if (loginResult.Result == ELoginResult.USERNAME_TAKEN)
				{
					MessageBox.Show("Someone with that username is already connected.", "Error logging in", MessageBoxButton.OK, MessageBoxImage.Error);
					return;
				}

				Dispatcher.Invoke(() =>
				{
					ChatClientInfo.Username = username;
					Client.GetInstance().SetPage(new ChatPage());
				});
			}), true);
		}
	}
}
