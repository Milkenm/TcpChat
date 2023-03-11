using System;
using System.Collections.Generic;

namespace Chat_API.Packets
{
	[Serializable]
	public class UsersInRoomPacket : IPacket
	{
		public List<string> UsersInRoom = new List<string>();

		public UsersInRoomPacket(List<string> usersInRoom)
		{
			if (usersInRoom != null)
			{
				UsersInRoom.AddRange(usersInRoom);
			}
		}
	}
}
