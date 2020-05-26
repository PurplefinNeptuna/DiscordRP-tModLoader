using DiscordRPC;
using Terraria;
using Terraria.Social;
using Terraria.Net;
using Terraria.ModLoader;
using System;
using Steamworks;

namespace DiscordRP {
	public class DiscordRP : Mod {

		//Mod Helper Issues report
		public static string GithubUserName => "PurplefinNeptuna";
		public static string GithubProjectName => "DiscordRP-tModLoader";

		public static DiscordRpcClient Client {
			get; private set;
		}

		public static RichPresence Instance {
			get; private set;
		}

		public static uint? prevCount;
		public static bool pauseUpdate = false;

		public DiscordRP() {
			Properties = new ModProperties() {
				Autoload = true,
				AutoloadBackgrounds = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public override void Load() {
			Instance = new RichPresence {
				Secrets = new Secrets()
			};
			Client = new DiscordRpcClient(applicationID: "404654478072086529", autoEvents: false);

			Main.OnTick += ClientUpdate;

			bool failedToRegisterScheme = false;

			try {
				Client.RegisterUriScheme("1281930");
			}
			catch(Exception) {
				failedToRegisterScheme = true;
			}

			if(!failedToRegisterScheme) {
				Client.OnJoinRequested += ClientOnJoinRequested;
				Client.OnJoin += ClientOnJoin;
			}

			Client.Initialize();
			Instance.Timestamps = Timestamps.Now;

			ClientOnMainMenu();
		}

		private void ClientOnJoin(object sender, DiscordRPC.Message.JoinMessage args) {
			//this is empty lol
			SocialAPI.Network.Connect(new SteamAddress(new CSteamID(Convert.ToUInt64(args.Secret))));
		}

		private void ClientOnJoinRequested(object sender, DiscordRPC.Message.JoinRequestMessage args) {
			Client.Respond(args, false);
		}

		private void ClientOnMainMenu() {
			ClientSetStatus("", "In Main Menu", "payload_test", "tModLoader");
			ClientSetParty();
			Client.SetPresence(Instance);
		}

		public override void PreSaveAndQuit() {
			ClientOnMainMenu();
		}

		public static void ClientSetStatus(string state = "", string details = "", string largeImageKey = null, string largeImageText = null, string smallImageKey = null, string smallImageText = null) {
			Instance.Assets = Instance.Assets ?? new Assets();
			Instance.State = state;
			Instance.Details = details;
			if(largeImageKey == null) {
				Instance.Assets.LargeImageKey = null;
				Instance.Assets.LargeImageText = null;
			}
			else {
				Instance.Assets.LargeImageKey = largeImageKey;
				Instance.Assets.LargeImageText = largeImageText;
			}

			if(smallImageKey == null) {
				Instance.Assets.SmallImageKey = null;
				Instance.Assets.SmallImageText = null;
			}
			else {
				Instance.Assets.SmallImageKey = smallImageKey;
				Instance.Assets.SmallImageText = smallImageText;
			}
		}

		public static void ClientSetParty(string secret = null, string id = null, int partysize = 0) {
			if(partysize == 0 || id == null) {
				Instance.Secrets.JoinSecret = null;
				Instance.Party = null;
			}
			else {
				//Instance.Secrets.JoinSecret = secret;
				//Instance.Party = Instance.Party ?? new Party();
				//Instance.Party.Size = partysize;
				//Instance.Party.Max = 256;
				//Instance.Party.ID = id;
				Instance.Secrets.JoinSecret = null;
				Instance.Party = null;
			}
		}

		public static void ClientForceUpdate() {
			Client.SetPresence(Instance);
			Client.Invoke();
		}

		public static void ClientUpdate() {
			if(!Main.dedServ && !Main.gameMenu) {
				if(Main.gamePaused || Main.gameInactive) {
					pauseUpdate = true;
				}
				else {
					pauseUpdate = false;
				}

				if((prevCount == null || prevCount + 180 <= Main.GameUpdateCount) && !pauseUpdate) {
					prevCount = Main.GameUpdateCount;
					Client.SetPresence(Instance);
					Client.Invoke();
				}
			}
		}

		public override void Unload() {
			Main.OnTick -= ClientUpdate;
			Client.Dispose();
			Instance = null;
			Client = null;
			prevCount = null;
		}
	}
}
