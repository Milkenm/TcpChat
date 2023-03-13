using System;
using System.Collections.Generic;

namespace ChatAPI.Packets
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
