using drpc;

namespace DiscordRP
{
	public static class RPControl
	{
		public static drpc.DiscordRP.RichPresence presence;
		public static string applicationId = "404654478072086529";

		public static drpc.DiscordRP.EventHandlers handlers;

		public static void ReadyCallback()
		{
		}

		public static void DisconnectedCallback(int errorCode, string message)
		{
		}

		public static void ErrorCallback(int errorCode, string message)
		{
		}

		public static void JoinCallback(string secret)
		{
		}

		public static void SpectateCallback(string secret)
		{
		}

		public static void RequestCallback(ref drpc.DiscordRP.JoinRequest request)
		{
		}

		public static void Enable()
		{
			handlers = new drpc.DiscordRP.EventHandlers();
			handlers.readyCallback = ReadyCallback;
			handlers.disconnectedCallback += DisconnectedCallback;
			handlers.errorCallback += ErrorCallback;
			handlers.joinCallback += JoinCallback;
			handlers.spectateCallback += SpectateCallback;
			handlers.requestCallback += RequestCallback;
			drpc.DiscordRP.Initialize(applicationId, ref handlers, true, null);
		}

		public static void Disable()
		{
			drpc.DiscordRP.Shutdown();
		}

		public static void Update()
		{
			drpc.DiscordRP.UpdatePresence(ref presence);
		}
	}
}