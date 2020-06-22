using System.ComponentModel;
using DiscordRPC;
using Terraria;
using Terraria.ModLoader.Config;

namespace DiscordRP {
	[Label("Config")]
	public class ClientConfig : ModConfig {
		public override ConfigScope Mode => ConfigScope.ClientSide;

		private DiscordRpcClient Client => DiscordRPMod.Instance?.Client;

		[Header("Rich Presence Settings")]
		[Label("Enable Rich Presence")]
		[DefaultValue(true)]
		public bool enable;

		[Label("Rich Presence Update Time")]
		[Tooltip("How many seconds elapsed between Rich Presence update")]
		[Range(1u, 60u)]
		[DefaultValue(1u)]
		[Slider]
		public uint timer;

		[Header("Visibility Settings")]
		[Label("Display Time Elapsed")]
		[DefaultValue(true)]
		public bool showTime;
		[Label("Display Day/Night")]
		[DefaultValue(true)]
		public bool showTimeCycle;
		[Label("Display Player Health")]
		[DefaultValue(true)]
		public bool showHealth;
		[Label("Display Player Mana")]
		[DefaultValue(true)]
		public bool showMana;
		[Label("Display Player Defense")]
		[DefaultValue(true)]
		public bool showDefense;
		[Label("Display Weapon Damage")]
		[DefaultValue(true)]
		public bool showDamage;

		public override void OnLoaded() {
			DiscordRPMod.Instance.config = this;
		}

		public override void OnChanged() {
			if (DiscordRPMod.Instance == null || DiscordRPMod.Instance.Client == null) {
				return;
			}

			if (enable) {
				if (Client.IsDisposed) {
					DiscordRPMod.Instance.CreateNewDiscordRPCRichPresenceInstance();
				}
				else if (!Client.IsInitialized) {
					Client.Initialize();
				}

				string currentClient = DiscordRPMod.Instance.currentClient;
				DiscordRPMod.Instance.currentClient = "default";
				DiscordRPMod.Instance.ChangeDiscordClient(currentClient);
				if (!Main.gameMenu) {
					DiscordRPMod.Instance.ClientUpdatePlayer();
				}
				DiscordRPMod.Instance.ClientForceUpdate();
			}
			else if (!Client.IsDisposed) {
				Client.Dispose();
			}
		}
	}
}
