using System.Net;

namespace Chat_API
{
	public class ChatUserInfo
	{
		public long Id { get; }
		public string Username { get; private set; }
		public EndPoint EndPoint { get; }

		public ChatUserInfo(long id, EndPoint endPoint)
		{
			Id = id;
			EndPoint = endPoint;
		}

		public void SetName(string name)
		{
			Username = name;
		}
	}
}
