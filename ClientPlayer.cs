using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DiscordRP {
	public class ClientPlayer : ModPlayer {
		internal bool dead = false;
		internal string worldStaticInfo = "";

		public override void OnEnterWorld(Player player) {
			if(player.whoAmI == Main.myPlayer) {
				if(DiscordRP.Client.IsInitialized) {
					string wName = Main.worldName;
					bool expert = Main.expertMode;
					string wDiff = (expert) ? "(Expert)" : "(Normal)";
					DiscordRP.worldStaticInfo = string.Format("Playing {0} {1}", wName, wDiff);
					DiscordRP.ClientUpdatePlayer();
					DiscordRP.ClientForceUpdate();
				}
			}
			DiscordRP.UpdateLobbyInfo();
			DiscordRP.ClientForceUpdate();
		}

		public override void PlayerConnect(Player player) {
			DiscordRP.UpdateLobbyInfo();
			DiscordRP.ClientForceUpdate();
		}

		public override void PlayerDisconnect(Player player) {
			DiscordRP.UpdateLobbyInfo();
			DiscordRP.ClientForceUpdate();
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
			if(player.whoAmI == Main.myPlayer) {
				dead = true;
			}
		}

		public override void OnRespawn(Player player) {
			if(player.whoAmI == Main.myPlayer) {
				dead = false;
			}
		}
	}
}
