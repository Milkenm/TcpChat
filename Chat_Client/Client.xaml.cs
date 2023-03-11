﻿using System;
using System.Windows;
using System.Windows.Controls;

using Chat_Client.Pages;

namespace Chat_Client
{
	/// <summary>
	/// Interaction logic for Client.xaml
	/// </summary>
	public partial class Client : Window
	{
		public static Client GetInstance()
		{
			return Instance ?? new Client();
		}

		private static Client Instance;

		public Client()
		{
			InitializeComponent();
			Instance = this;
		}

		private void Window_Loaded(object sender, RoutedEventArgs e)
		{
			SetPage(new LoginPage());
		}

		public void SetPage(Page page)
		{
			string pageUri = $"Pages/{page.GetType().Name}.xaml";
			frame_page.Source = new Uri(pageUri, UriKind.Relative);
		}
		public void SetTitle(string username)
		{
			if (username != null)
			{
				Title = $"TcpChat: {username}";
			}
			else
			{
				Title = "TcpChat";
			}
		}
	}
}
