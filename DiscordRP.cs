using DiscordRPC;
using Terraria;
using Terraria.Social;
using Terraria.Net;
using Terraria.ModLoader;
using Terraria.GameContent.Events;
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
		public float priority = 0f;
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

		internal static uint prevCount = 0;
		internal static bool pauseUpdate = false;

		internal static Dictionary<int, (string, string, float)> exBossIDtoDetails = new Dictionary<int, (string, string, float)>();

		internal static DRPStatus customStatus = null;

		internal static List<BiomeStatus> exBiomeStatus = new List<BiomeStatus>();

		internal static string worldStaticInfo = null;

		internal static bool PartyEvent => BirthdayParty.PartyIsUp;
		internal static bool SandstormEvent => Sandstorm.Happening;

		public DiscordRP() {
			Properties = new ModProperties() {
				Autoload = true,
				AutoloadBackgrounds = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}

		public override void Load() {
			pauseUpdate = false;
			exBiomeStatus = new List<BiomeStatus>();
			exBossIDtoDetails = new Dictionary<int, (string, string, float)>();
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
			DRPX.AddVanillaBosses();
			DRPX.AddVanillaBiomes();
			DRPX.AddVanillaEvents();
		}

		private void ClientOnJoin(object sender, DiscordRPC.Message.JoinMessage args) {
			//this is empty lol
			SocialAPI.Network.Connect(new SteamAddress(new CSteamID(Convert.ToUInt64(args.Secret))));
		}

		private void ClientOnJoinRequested(object sender, DiscordRPC.Message.JoinRequestMessage args) {
			Client.Respond(args, false);
		}

		private void ClientOnMainMenu() {
			pauseUpdate = false;
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
			if(!Main.gameMenu && !Main.dedServ) {
				if(Main.gamePaused || Main.gameInactive) {
					pauseUpdate = true;
				}
				else {
					prevCount++;
					pauseUpdate = false;
				}

				if((prevCount % 120u == 0) && !pauseUpdate) {
					ClientUpdatePlayer();
					ClientForceUpdate();
				}
			}
		}

		public override void Unload() {
			Main.OnTick -= ClientUpdate;
			Client.Dispose();
			Instance = null;
			Client = null;
			exBossIDtoDetails = null;
			customStatus = null;
			exBiomeStatus = null;
			worldStaticInfo = null;
		}

		public override object Call(params object[] args) {
			return DRPX.Call(args);
		}

		internal static void UpdateLobbyInfo() {
			if(Main.LobbyId != 0UL) {
				//string sId = SteamUser.GetSteamID().ToString();
				ClientSetParty(null, Main.LocalPlayer.name, Main.ActivePlayersCount);
			}
		}

		internal static void ClientUpdatePlayer() {
			if(Main.LocalPlayer != null) {
				(string itemKey, string itemText) = GetItemStat();
				(string bigKey, string bigText) = DRPX.GetBoss();

				string state;
				if(!Main.LocalPlayer.GetModPlayer<ClientPlayer>().dead) {
					state = string.Format("HP: {0} MP: {1} DEF: {2}", Main.LocalPlayer.statLife, Main.LocalPlayer.statMana, Main.LocalPlayer.statDefense);
				}
				else {
					state = string.Format("Dead");
				}

				//DiscordRP.ClientSetStatus(state, worldStaticInfo, bigKey, bigText, itemKey, itemText);
				ClientSetStatus(state, bigText, bigKey, worldStaticInfo, itemKey, itemText);
				UpdateLobbyInfo();

				if(Main.LocalPlayer.GetModPlayer<ClientPlayer>().dead)
					ClientForceUpdate();
			}
		}

		internal static (string, string) GetItemStat() {
			int atk;
			Item item = Main.LocalPlayer?.HeldItem;
			if(item != null) {
				if(item.melee) {
					atk = (int)Math.Ceiling(item.damage * Main.LocalPlayer.meleeDamage);
					return (string.Format("atk_melee"), string.Format("{0} ({1} Melee)", item.Name, atk));
				}
				else if(item.ranged) {
					atk = (int)Math.Ceiling(item.damage * Main.LocalPlayer.rangedDamage);
					return (string.Format("atk_range"), string.Format("{0} ({1} Ranged)", item.Name, atk));
				}
				else if(item.magic) {
					atk = (int)Math.Ceiling(item.damage * Main.LocalPlayer.magicDamage);
					return (string.Format("atk_magic"), string.Format("{0} ({1} Magic)", item.Name, atk));
				}
				else if(item.thrown) {
					atk = (int)Math.Ceiling(item.damage * Main.LocalPlayer.thrownDamage);
					return (string.Format("atk_throw"), string.Format("{0} ({1} Thrown)", item.Name, atk));
				}
				else if(item.summon) {
					atk = (int)Math.Ceiling(item.damage * Main.LocalPlayer.minionDamage);
					return (string.Format("atk_summon"), string.Format("{0} ({1} Summon)", item.Name, atk));
				}
			}
			return (null, null);
		}
	}
}
