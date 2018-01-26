using DiscordRPCsharp;

namespace DiscordRP
{
    public static class RPControl
    {
        public static DiscordRPCsharp.DiscordRP.RichPresence presence;
        public static string applicationId = "404654478072086529";

        public static DiscordRPCsharp.DiscordRP.EventHandlers handlers;

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

        public static void RequestCallback(ref DiscordRPCsharp.DiscordRP.JoinRequest request)
        {
        }

        public static void Enable()
        {
            handlers = new DiscordRPCsharp.DiscordRP.EventHandlers();
            handlers.readyCallback = ReadyCallback;
            handlers.disconnectedCallback += DisconnectedCallback;
            handlers.errorCallback += ErrorCallback;
            handlers.joinCallback += JoinCallback;
            handlers.spectateCallback += SpectateCallback;
            handlers.requestCallback += RequestCallback;
            DiscordRPCsharp.DiscordRP.Initialize(applicationId, ref handlers, true, null);
        }

        public static void Disable()
        {
            DiscordRPCsharp.DiscordRP.Shutdown();
        }

        public static void Update()
        {
            DiscordRPCsharp.DiscordRP.UpdatePresence(ref presence);
        }
    }
}
