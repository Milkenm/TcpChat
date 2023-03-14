using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

using ChatAPI;
using ChatAPI.Packets;

using ScriptsLibV2;
using ScriptsLibV2.Extensions;
using ScriptsLibV2.Util;

namespace ChatServer
{
	internal class Program
	{
		private static TcpServer Server;

		private static long ClientIdCounter;
		private static readonly Dictionary<Socket, ChatUserInfo> ConnectedClients = new Dictionary<Socket, ChatUserInfo>();

		private static void Main(string[] args)
		{
			Server = new TcpServer(Settings.ServerPort);
			Server.OnClientConnect += Server_OnClientConnect;
			Server.OnClientDisconnect += Server_OnClientDisconnect;
			Server.OnDataReceived += Server_OnDataReceived;
			Server.OnLog += Server_OnLog;
			Server.Start();

			Task.Delay(-1).Wait();
		}

		private static void Server_OnLog(string log)
		{
			if (Utils.IsDebugEnabled())
			{
				Console.WriteLine(log);
			}
		}

		private static void Server_OnDataReceived(Socket client, byte[] data)
		{
			IPacket packet = data.ToObject<IPacket>();
			Log($"Received '{packet.GetType()}' packet from client ({GetClientInfo(client)}).");
			switch (packet)
			{
				case RequestServerStatusPacket requestStatusPacket:
					{
						EServerStatus serverStatus = EServerStatus.ONLINE;
						ServerStatusPacket response = new ServerStatusPacket(serverStatus);
						Server.SendObject(client, response);
						Log($"Sent status '{serverStatus}' to client ({GetClientInfo(client)}).");
						break;
					}
				case LoginPacket loginPacket:
					{
						// Check if client is already connected
						if (IsConnected(client))
						{
							Log($"Client ({GetClientInfo(client)}) attempted a second connection.");
							LoginResultPacket result = new LoginResultPacket(ELoginResult.DUPLICATE_CONNECTION);
							Server.SendObject(client, result);
							break;
						}

						// Check if username is already connected
						ChatUserInfo existingUsername = ConnectedClients.Values.FirstOrDefault(c => c.Username == loginPacket.Username);
						if (existingUsername != null)
						{
							Log($"Client ({GetClientInfo(client)} attempted to connect using duplicate name '{loginPacket.Username}'.");
							LoginResultPacket result = new LoginResultPacket(ELoginResult.USERNAME_TAKEN);
							Server.SendObject(client, result);
							break;
						}

						// Check if username is valid
						bool isUsernameValid = (loginPacket.Username.Length >= Settings.UsernameMinimumLength && loginPacket.Username.Length <= Settings.UsernameMaximumLength);
						if (isUsernameValid)
						{
							foreach (char c in loginPacket.Username)
							{
								if (!Settings.UsernameAllowedCharacters.Contains(c))
								{
									isUsernameValid = false;
									break;
								}
							}
						}
						if (!isUsernameValid)
						{
							Log($"Client ({GetClientInfo(client)} attempted to connect with username '{loginPacket.Username}' containing invalid characters.");
							LoginResultPacket result = new LoginResultPacket(ELoginResult.INVALID_CHARACTER);
							Server.SendObject(client, result);
							break;
						}

						// Success
						ChatUserInfo userInfo = ConnectedClients[client];
						userInfo.SetUsername(loginPacket.Username);
						Server.SendObject(client, new LoginResultPacket(ELoginResult.SUCCESS));
						Log($"Client ({GetClientInfo(client)} connected with username '{loginPacket.Username}'.");

						// Update all other clients about the new user
						SendUsersListToAllClients();

						break;
					}
				case ClientMessagePacket messagePacket:
					{
						// Unknown client
						if (!IsConnected(client))
						{
							Log($"Received a packet from an unknown client ({GetClientInfo(client)}).");
							break;
						}

						ChatUserInfo whoSent = ConnectedClients[client];

						// Send message back to all connected clients (except the one who sent it)
						Log($"Chat message from '{whoSent.Username}' ({GetClientInfo(client)}): {messagePacket.Message}");
						if (ConnectedClients.Count - 1 > 0)
						{
							Log($"Sending message to {ConnectedClients.Count - 1} clients...");
						}
						foreach (Socket connectedClient in ConnectedClients.Keys)
						{
							if (connectedClient == client) { continue; }
							Server.SendObject(connectedClient, new ServerMessagePacket(whoSent.Username, messagePacket.Message));
						}
						break;
					}
				case RequestUsersInRoomPacket _:
					{
						Log($"Refresing users in room for client ({GetClientInfo(client)})");
						SendUserListToClient(client);
						break;
					}
			}
		}

		private static void SendUsersListToAllClients()
		{
			if (ConnectedClients.Count == 0) { return; }

			Log($"Send refreshed connected users list to {ConnectedClients.Count} clients.");
			foreach (Socket connectedClient in ConnectedClients.Keys)
			{
				SendUserListToClient(connectedClient);
			}
		}

		private static void SendUserListToClient(Socket client)
		{
			List<string> usersInRoom = new List<string>();

			foreach (ChatUserInfo userInfo in ConnectedClients.Values)
			{
				usersInRoom.Add(userInfo.Username);
			}

			UsersInRoomPacket response = new UsersInRoomPacket(usersInRoom);
			Server.SendObject(client, response);
		}

		private static void Server_OnClientConnect(Socket client)
		{
			// Check if client is already connected
			if (ConnectedClients.ContainsKey(client)) { return; }

			ChatUserInfo connectedClient = new ChatUserInfo(ClientIdCounter, client.RemoteEndPoint);
			ConnectedClients.Add(client, connectedClient);
			Log($"Client ({GetClientInfo(client)}) connected.");
			ClientIdCounter++;
		}

		private static void Server_OnClientDisconnect(Socket client)
		{
			ChatUserInfo disconnectedClient = ConnectedClients[client];
			Log($"Client ({GetClientInfo(client)}) disconnected.");
			ConnectedClients.Remove(client);

			SendUsersListToAllClients();
		}

		private static string GetClientInfo(Socket client)
		{
			if (IsConnected(client))
			{
				ChatUserInfo userInfo = ConnectedClients[client];
				return userInfo.ToString();
			}
			return $"{client.RemoteEndPoint}";
		}

		private static void Log(string message)
		{
			Console.WriteLine($"[{DateTime.Now}] {message}");
		}

		private static bool IsConnected(Socket client)
		{
			return ConnectedClients.ContainsKey(client);
		}
	}
}
