using System.Linq;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DiscordRP {
	public class ClientPlayer : ModPlayer {
		internal bool dead = false;
		internal string worldStaticInfo = "";
		internal int nearbyNPC = 0;

		public override void OnEnterWorld(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				DiscordRPMod.Instance.pauseUpdate = false;
				string wName = Main.worldName;
				bool expert = Main.expertMode;
				string wDiff = (expert) ? "(Expert)" : "(Normal)";
				DiscordRPMod.Instance.worldStaticInfo = string.Format("Playing {0} {1}", wName, wDiff);
				DiscordRPMod.Instance.ClientUpdatePlayer();
			}
			DiscordRPMod.Instance.UpdateLobbyInfo();
			DiscordRPMod.Instance.ClientForceUpdate();
		}

		public override void PlayerConnect(Player player) {
			DiscordRPMod.Instance.UpdateLobbyInfo();
			DiscordRPMod.Instance.ClientForceUpdate();
		}

		public override void PlayerDisconnect(Player player) {
			DiscordRPMod.Instance.UpdateLobbyInfo();
			DiscordRPMod.Instance.ClientForceUpdate();
		}

		public override void PreUpdate() {
			nearbyNPC = Main.npc.Count(npc => npc.active && npc.townNPC && Vector2.DistanceSquared(npc.position, player.Center) <= 2250000f);
		}

		public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource) {
			if (player.whoAmI == Main.myPlayer) {
				dead = true;
			}
		}

		public override void OnRespawn(Player player) {
			if (player.whoAmI == Main.myPlayer) {
				dead = false;
			}
		}
	}
}
