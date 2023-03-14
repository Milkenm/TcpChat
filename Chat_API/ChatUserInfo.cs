using System.Net;

using ScriptsLibV2.Extensions;

namespace ChatAPI
{
	public class ChatUserInfo
	{
		public long Id { get; }
		public string Username { get; private set; }
		public EndPoint EndPoint { get; }

		public ChatUserInfo(long id, string username, EndPoint endPoint)
		{
			Id = id;
			Username = username;
			EndPoint = endPoint;
		}

		public override string ToString()
		{
			string username = !Username.IsEmpty() ? Username : "<no_username>";
			return $"{EndPoint}/{Username}#{Id}";
		}
	}
}
