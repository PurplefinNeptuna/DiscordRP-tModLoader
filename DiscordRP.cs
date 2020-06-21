using DiscordRPC;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace DiscordRP {

	/// <summary>
	/// Rich Presence Status class, currently only used for custom main menu
	/// </summary>
	internal class DRPStatus {
		public string details = "", additionalDetails = "";
		public string largeKey = "", largeImage = "";
		public string smallKey = "", smallImage = "";
		public string GetState() => additionalDetails;
		public string GetDetails() => details;
	}

	/// <summary>
	///	Class for custom biomes
	/// </summary>
	internal class BiomeStatus {
		public Func<bool> checker = null;
		public string largeKey = "biome_placeholder";
		public string largeText = "???";
		public string client = "default";
		public float priority = 0f;
	}

	public class DiscordRP : Mod {

		//Mod Helper Issues report
		public static string GithubUserName => "PurplefinNeptuna";
		public static string GithubProjectName => "DiscordRP-tModLoader";

		public static DiscordRP Instance = null;

		public string currentClient = "default";

		internal DiscordRpcClient Client {
			get; set;
		}

		internal RichPresence RichPresenceInstance {
			get; private set;
		}

		internal uint prevCount = 0;
		internal bool pauseUpdate = false;
		internal bool canCreateClient;

		internal Dictionary<int, (string, string, string, float)> exBossIDtoDetails = new Dictionary<int, (string, string, string, float)>();

		internal DRPStatus customStatus = null;

		internal List<BiomeStatus> exBiomeStatus = new List<BiomeStatus>();

		internal string worldStaticInfo = null;

		//internal static Dictionary<string, DiscordRpcClient> discordRPCs;
		internal Dictionary<string, string> savedDiscordAppId;

		public DiscordRP() {
			Properties = new ModProperties() {
				Autoload = true,
				AutoloadBackgrounds = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
			Instance = this;
		}

		public override void Load() {
			if(!Main.dedServ) {
				currentClient = "default";
				canCreateClient = true;
				pauseUpdate = false;
				exBiomeStatus = new List<BiomeStatus>();
				exBossIDtoDetails = new Dictionary<int, (string, string, string, float)>();

				savedDiscordAppId = new Dictionary<string, string>();

				RichPresenceInstance = new RichPresence {
					Secrets = new Secrets()
				};

				CreateNewDiscordRPCRichPresenceInstance("404654478072086529");
				//CreateNewDiscordRPCRichPresenceInstance("716207249902796810", "angryslimey");
				//AddDiscordAppID("angryslimey", "716207249902796810");
			}
		}

		public override void AddRecipes() {
			if(!Main.dedServ) {
				DRPX.AddVanillaBosses();
				DRPX.AddVanillaBiomes();
				DRPX.AddVanillaEvents();

				Main.OnTick += ClientUpdate;
				RichPresenceInstance.Timestamps = Timestamps.Now;
				//finished
				canCreateClient = false;
				ClientOnMainMenu();
			}
		}

		/// <summary>
		/// Change the Discord App ID, currently takes 3s to change
		/// </summary>
		/// <param name="newClient">New Discord App ID key</param>
		public void ChangeDiscordClient(string newClient) {
			if(newClient == currentClient) {
				return;
			}
			if(!savedDiscordAppId.ContainsKey(newClient)) {
				return;
			}
			currentClient = newClient;
			Client.ApplicationID = savedDiscordAppId[newClient];
		}

		/// <summary>
		/// Create new DiscordRP client, currently only used once
		/// </summary>
		/// <param name="appId">Discord App ID</param>
		/// <param name="key">key for App ID</param>
		private void CreateNewDiscordRPCRichPresenceInstance(string appId, string key = "default") {
			if(!savedDiscordAppId.ContainsKey(key)) {
				savedDiscordAppId.Add(key, appId);
			}

			Client = new DiscordRpcClient(applicationID: appId, autoEvents: false);

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
		}

		/// <summary>
		/// Add other Discord App ID
		/// </summary>
		/// <param name="key">the key</param>
		/// <param name="appID">Discord App ID</param>
		public void AddDiscordAppID(string key, string appID) {
			if(!savedDiscordAppId.ContainsKey(key)) {
				savedDiscordAppId.Add(key, appID);
			}
		}

		/// <summary>
		/// Discord OnJoin event, called on the joiner
		/// </summary>
		private void ClientOnJoin(object sender, DiscordRPC.Message.JoinMessage args) {
			//this is empty lol
			//SocialAPI.Network.Connect(new SteamAddress(new CSteamID(Convert.ToUInt64(args.Secret))));
		}

		/// <summary>
		/// Discord OnJoinRequested event, called on the host, currently deny everything lol
		/// </summary>
		private void ClientOnJoinRequested(object sender, DiscordRPC.Message.JoinRequestMessage args) {
			Client.Respond(args, false);
		}

		/// <summary>
		/// Change the status to main menu
		/// </summary>
		private void ClientOnMainMenu() {
			ChangeDiscordClient("default");
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
			ClientForceUpdate();
		}

		/// <summary>
		///	override this because i can only find this method that called when going to main menu
		/// </summary>
		public override void PreSaveAndQuit() {
			if(!Main.dedServ) {
				ClientOnMainMenu();
			}
		}

		/// <summary>
		/// Change the status
		/// </summary>
		/// <param name="state">lower status string</param>
		/// <param name="details">upper status string</param>
		/// <param name="largeImageKey">key for large image</param>
		/// <param name="largeImageText">text for large image</param>
		/// <param name="smallImageKey">key for small image</param>
		/// <param name="smallImageText">text for small image</param>
		public void ClientSetStatus(string state = "", string details = "", string largeImageKey = null, string largeImageText = null, string smallImageKey = null, string smallImageText = null) {
			RichPresenceInstance.Assets = RichPresenceInstance.Assets ?? new Assets();
			RichPresenceInstance.State = state;
			RichPresenceInstance.Details = details;
			if(largeImageKey == null) {
				RichPresenceInstance.Assets.LargeImageKey = null;
				RichPresenceInstance.Assets.LargeImageText = null;
			}
			else {
				RichPresenceInstance.Assets.LargeImageKey = largeImageKey;
				RichPresenceInstance.Assets.LargeImageText = largeImageText;
			}

			if(smallImageKey == null) {
				RichPresenceInstance.Assets.SmallImageKey = null;
				RichPresenceInstance.Assets.SmallImageText = null;
			}
			else {
				RichPresenceInstance.Assets.SmallImageKey = smallImageKey;
				RichPresenceInstance.Assets.SmallImageText = smallImageText;
			}
		}

		/// <summary>
		/// set the party settings
		/// </summary>
		/// <param name="secret">party secret</param>
		/// <param name="id">party id</param>
		/// <param name="partysize">party current size</param>
		public void ClientSetParty(string secret = null, string id = null, int partysize = 0) {
			if(partysize == 0 || id == null) {
				RichPresenceInstance.Secrets.JoinSecret = null;
				RichPresenceInstance.Party = null;
			}
			else {
				//RichPresenceInstance.Secrets.JoinSecret = secret;
				//RichPresenceInstance.Party = RichPresenceInstance.Party ?? new Party();
				//RichPresenceInstance.Party.Size = partysize;
				//RichPresenceInstance.Party.Max = 256;
				//RichPresenceInstance.Party.ID = id;
				RichPresenceInstance.Secrets.JoinSecret = null;
				RichPresenceInstance.Party = null;
			}
		}

		/// <summary>
		/// Forcing update rich presence
		/// </summary>
		public void ClientForceUpdate() {
			if(Client != null) {
				if(!Client.IsInitialized) {
					Client.Initialize();
				}
				Client.SetPresence(RichPresenceInstance);
				//Main.NewText(Client.ApplicationID);
				Client.Invoke();
			}
		}

		/// <summary>
		///	run this everytick to update
		/// </summary>
		public void ClientUpdate() {
			if(!Main.gameMenu && !Main.dedServ) {
				if(Main.gamePaused || Main.gameInactive) {
					pauseUpdate = true;
				}
				else {
					prevCount++;
					pauseUpdate = false;
				}

				if((prevCount % 60u == 0) && !pauseUpdate) {
					ClientUpdatePlayer();
					ClientForceUpdate();
				}
			}
		}

		public override void Unload() {
			Main.OnTick -= ClientUpdate;
			Client.Dispose();

			Instance = null;
		}

		public override object Call(params object[] args) {
			if(!Main.dedServ) {
				return DRPX.Call(args);
			}
			return "Can't call on server";
		}

		/// <summary>
		/// update the party info
		/// </summary>
		internal void UpdateLobbyInfo() {
			if(Main.LobbyId != 0UL) {
				//string sId = SteamUser.GetSteamID().ToString();
				ClientSetParty(null, Main.LocalPlayer.name, Main.ActivePlayersCount);
			}
		}

		/// <summary>
		/// method for update the status, checking from item to biome/boss/events
		/// </summary>
		internal void ClientUpdatePlayer() {
			if(Main.LocalPlayer != null) {
				(string itemKey, string itemText) = GetItemStat();
				(string bigKey, string bigText, string selectedClient) = DRPX.GetBoss();

				string state;
				if(!Main.LocalPlayer.GetModPlayer<ClientPlayer>().dead) {
					state = string.Format("HP: {0} MP: {1} DEF: {2}", Main.LocalPlayer.statLife, Main.LocalPlayer.statMana, Main.LocalPlayer.statDefense);
				}
				else {
					state = string.Format("Dead");
				}

				ClientSetStatus(state, bigText, bigKey, worldStaticInfo, itemKey, itemText);
				UpdateLobbyInfo();
				Instance.ChangeDiscordClient(selectedClient);

				if(Main.LocalPlayer.GetModPlayer<ClientPlayer>().dead)
					ClientForceUpdate();
			}
		}

		/// <summary>
		/// Get the player's item stat
		/// </summary>
		/// <returns>key and text for small images</returns>
		internal (string, string) GetItemStat() {
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
