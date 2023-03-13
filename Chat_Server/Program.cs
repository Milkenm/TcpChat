using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;

using Chat_API;
using Chat_API.Packets;

using ScriptsLibV2;
using ScriptsLibV2.Extensions;

namespace Chat_Server
{
	internal class Program
	{
		private static TcpServer Server;

		private static long ClientId;
		private static readonly Dictionary<Socket, ChatUserInfo> ConnectedClients = new Dictionary<Socket, ChatUserInfo>();

		private static void Main(string[] args)
		{
			Server = new TcpServer(Settings.ServerPort);
			Server.OnClientConnect += Server_OnClientConnect;
			Server.OnClientDisconnect += Server_OnClientDisconnect;
			Server.OnDataReceived += Server_OnDataReceived;
			Server.Start();

			Task.Delay(-1).Wait();
		}

		private static void Server_OnDataReceived(Socket client, byte[] data)
		{
			IPacket packet = data.ToObject<IPacket>();
			switch (packet)
			{
				case RequestServerStatusPacket requestStatusPacket:
					{
						ServerStatusPacket response = new ServerStatusPacket(EServerStatus.ONLINE);
						Server.SendObject(client, response);
						break;
					}
				case LoginPacket loginPacket:
					{
						// Check if username is already connected
						ChatUserInfo existingUsername = ConnectedClients.Values.FirstOrDefault(c => c.Username == loginPacket.Username);
						if (existingUsername != null)
						{
							LoginResultPacket result = new LoginResultPacket(ELoginResult.USERNAME_TAKEN);
							Server.SendObject(client, data);
						}

						// Success
						ChatUserInfo userInfo = ConnectedClients[client];
						userInfo.SetName(loginPacket.Username);
						Server.SendObject(client, new LoginResultPacket(ELoginResult.SUCCESS));

						// Update all other clients about the new user
						SendUsersListToAllClients();

						break;
					}
				case ClientMessagePacket messagePacket:
					{
						Console.WriteLine($"Received new {nameof(ClientMessagePacket)} packet from IP {client.RemoteEndPoint}.");

						// Unknown client
						ChatUserInfo whoSent = ConnectedClients[client];
						if (whoSent == null)
						{
							Console.WriteLine("Unknown client.");
							break;
						}

						// Send message back to all connected clients (except the one who sent it)
						if (ConnectedClients.Count - 1 == 0) { break; }
						Console.WriteLine($"{whoSent.Username}: {messagePacket.Message}");
						Console.WriteLine($"Broadcasting message to {ConnectedClients.Count - 1} clients.");
						foreach (Socket connectedClient in ConnectedClients.Keys)
						{
							if (connectedClient == client) { continue; }
							Server.SendObject(connectedClient, new ServerMessagePacket(whoSent.Username, messagePacket.Message));
							Console.WriteLine("Sent to 1 client.");
						}
						break;
					}
				case RequestUsersInRoomPacket _:
					{
						SendUserListToClient(client);
						break;
					}
			}
		}

		private static void SendUsersListToAllClients()
		{
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

			ChatUserInfo connectedClient = new ChatUserInfo(ClientId, client.RemoteEndPoint);
			Console.WriteLine($"New client (ID #{ClientId}) connected from IP {client.RemoteEndPoint}.");
			ClientId++;

			ConnectedClients.Add(client, connectedClient);
		}

		private static void Server_OnClientDisconnect(Socket client)
		{
			ChatUserInfo disconnectedClient = ConnectedClients[client];
			Console.WriteLine($"Client (ID #{disconnectedClient.Id}, IP {disconnectedClient.EndPoint}) disconnected.");
			ConnectedClients.Remove(client);

			SendUsersListToAllClients();
		}
	}
}
