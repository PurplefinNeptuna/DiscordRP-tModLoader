using DiscordRPC;
using Terraria;
using Terraria.Social;
using Terraria.Net;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using Steamworks;

namespace DiscordRP {

	internal class DRPStatus {
		public string details = "", additionalDetails = "";
		public string largeKey = "", largeImage = "";
		public string smallKey = "", smallImage = "";
		public string GetState() => additionalDetails;
		public string GetDetails() => details;
	}

	internal class BiomeStatus {
		public Func<bool> checker = null;
		public string largeKey = "biome_placeholder";
		public string largeText = "???";
	}

	public class DiscordRP : Mod {

		//Mod Helper Issues report
		public static string GithubUserName => "PurplefinNeptuna";
		public static string GithubProjectName => "DiscordRP-tModLoader";

		internal static DiscordRpcClient Client {
			get; private set;
		}

		internal static RichPresence Instance {
			get; private set;
		}

		internal static uint? prevCount;
		internal static bool pauseUpdate = false;

		internal static List<int> bossID = new List<int>();

		internal static Dictionary<int, (string, string)> exBossIDtoDetails = new Dictionary<int, (string, string)>();

		internal static DRPStatus customStatus = null;

		internal static List<BiomeStatus> exBiomeStatus = new List<BiomeStatus>();

		public DiscordRP() {
			Properties = new ModProperties() {
				Autoload = true,
				AutoloadBackgrounds = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public override void Load() {
			exBiomeStatus = new List<BiomeStatus>();
			exBossIDtoDetails = new Dictionary<int, (string, string)>();
			bossID = new List<int>() {
			50,
			4,
			13,14,15,
			266,
			222,
			35,
			113,
			125,126,
			134,
			127,
			262,
			245,
			370,
			439,
			396,397,398
		};
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
		}

		public override void AddRecipes() {
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
			if(customStatus == null) {
				ClientSetStatus("", "In Main Menu", "payload_test", "tModLoader");
			}
			else {
				ClientSetStatus(customStatus.GetState(), customStatus.GetDetails(),
				customStatus.largeKey, customStatus.largeImage,
				customStatus.smallKey, customStatus.smallImage);
			}

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
			if(!Main.gameMenu) {
				if(Main.gamePaused || Main.gameInactive) {
					pauseUpdate = true;
				}
				else {
					pauseUpdate = false;
				}

				if((prevCount == null || prevCount + 180 <= Main.GameUpdateCount) && !pauseUpdate) {
					prevCount = Main.GameUpdateCount;
					ClientForceUpdate();
				}
			}
		}

		public override void Unload() {
			Main.OnTick -= ClientUpdate;
			Client.Dispose();
			Instance = null;
			Client = null;
			prevCount = null;
			bossID = null;
			exBossIDtoDetails = null;
			customStatus = null;
			exBiomeStatus = null;
		}

		public override object Call(params object[] args) {
			return DRPX.Call(args);
		}

	}
}
