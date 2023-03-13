using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

using Chat_API.Packets;

using ScriptsLibV2.Extensions;

namespace Chat_Client.Pages
{
	/// <summary>
	/// Interaction logic for ChatPage.xaml
	/// </summary>
	public partial class ChatPage : Page
	{
		public ChatPage()
		{
			InitializeComponent();
			NetworkClient.Client.OnDataReceived += Client_OnDataReceived;
			NetworkClient.Client.OnDisconnect += Client_OnDisconnect;
		}

		private void Page_Loaded(object sender, RoutedEventArgs e)
		{
			// This is handled by the default packet handler
			NetworkClient.Client.Send(new RequestUsersInRoomPacket());
		}

		private void Client_OnDisconnect()
		{
			Dispatcher.Invoke(() => Client.GetInstance().SetPage(new LoginPage()));
		}

		private void Client_OnDataReceived(EndPoint source, byte[] data)
		{
			IPacket packet = data.ToObject<IPacket>();
			switch (packet)
			{
				case ServerMessagePacket serverMessagePacket:
					{
						Dispatcher.Invoke(() =>
						{
							listBox_chat.Items.Add($"{serverMessagePacket.Username}: {serverMessagePacket.Message}");
							ScrollToEnd();
						});
						break;
					}
				case UsersInRoomPacket usersInRoomPacket:
					{
						Dispatcher.Invoke(() =>
						{
							listBox_users.Items.Clear();
							foreach (string userInRoom in usersInRoomPacket.UsersInRoom)
							{
								listBox_users.Items.Add(userInRoom);
							}
						});
						break;
					}
			}
		}

		private void ScrollToEnd()
		{
			if (VisualTreeHelper.GetChildrenCount(listBox_chat) > 0)
			{
				Border border = (Border)VisualTreeHelper.GetChild(listBox_chat, 0);
				ScrollViewer scrollViewer = (ScrollViewer)VisualTreeHelper.GetChild(border, 0);
				scrollViewer.ScrollToBottom();
			}
		}

		private void button_send_Click(object sender, RoutedEventArgs e)
		{
			SendMessage();
		}

		private void textBox_chat_KeyDown(object sender, KeyEventArgs e)
		{
			if (e.Key == Key.Enter)
			{
				SendMessage();
			}
		}

		private void SendMessage()
		{
			// Focus textBox
			textBox_chat.Focus();

			// Validate
			string message = textBox_chat.Text;
			if (message.IsEmpty()) { return; }

			// Send message
			ClientMessagePacket messagePacket = new ClientMessagePacket(message);
			NetworkClient.Client.Send(messagePacket);
			listBox_chat.Items.Add($"YOU: {message}");
			ScrollToEnd();

			// Clear textbox
			textBox_chat.Text = string.Empty;
		}
	}
}
