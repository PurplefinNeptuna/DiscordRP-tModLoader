using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DiscordRP {
	public class ClientPlayer : ModPlayer {
		private bool dead = false;
		private string worldStaticInfo = "";

		public override void OnEnterWorld(Player player) {
			if(player.whoAmI == Main.myPlayer) {
				if(DiscordRP.Client.IsInitialized) {
					string wName = Main.worldName;
					bool expert = Main.expertMode;
					string wDiff = (expert) ? "(Expert)" : "(Normal)";
					worldStaticInfo = string.Format("Playing {0} {1}", wName, wDiff);
					ClientUpdate();
					DiscordRP.ClientForceUpdate();
				}
			}
			UpdateLobbyInfo();
			DiscordRP.ClientForceUpdate();
		}

		private void UpdateLobbyInfo() {
			if(Main.LobbyId != 0UL) {
				//string sId = SteamUser.GetSteamID().ToString();
				DiscordRP.ClientSetParty(null, player.name, Main.ActivePlayersCount);
			}
		}

		public override void PlayerConnect(Player player) {
			UpdateLobbyInfo();
			DiscordRP.ClientForceUpdate();
		}

		public override void PlayerDisconnect(Player player) {
			UpdateLobbyInfo();
			DiscordRP.ClientForceUpdate();
		}

		private void ClientUpdate() {
			(string itemKey, string itemText) = GetItemStat();
			(string bigKey, string bigText) = DRPX.GetBoss(player.zone1, player.zone2, player.zone3);

			string state;
			if(!dead) {
				state = string.Format("HP: {0} MP: {1} DEF: {2}", player.statLife, player.statMana, player.statDefense);
			}
			else {
				state = string.Format("Dead");
			}

			DiscordRP.ClientSetStatus(state, worldStaticInfo, bigKey, bigText, itemKey, itemText);
			UpdateLobbyInfo();

			if(dead)
				DiscordRP.ClientForceUpdate();
		}

		public override void PostUpdate() {
			if(player.whoAmI == Main.myPlayer) {
				ClientUpdate();
			}
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

		private (string, string) GetItemStat() {
			int atk;
			Item item = player.HeldItem;
			if(item != null) {
				if(item.melee) {
					atk = (int)Math.Ceiling(item.damage * player.meleeDamage);
					return (string.Format("atk_melee"), string.Format("{0} ({1} Melee)", item.Name, atk));
				}
				else if(item.ranged) {
					atk = (int)Math.Ceiling(item.damage * player.rangedDamage);
					return (string.Format("atk_range"), string.Format("{0} ({1} Ranged)", item.Name, atk));
				}
				else if(item.magic) {
					atk = (int)Math.Ceiling(item.damage * player.magicDamage);
					return (string.Format("atk_magic"), string.Format("{0} ({1} Magic)", item.Name, atk));
				}
				else if(item.thrown) {
					atk = (int)Math.Ceiling(item.damage * player.thrownDamage);
					return (string.Format("atk_throw"), string.Format("{0} ({1} Thrown)", item.Name, atk));
				}
				else if(item.summon) {
					atk = (int)Math.Ceiling(item.damage * player.minionDamage);
					return (string.Format("atk_summon"), string.Format("{0} ({1} Summon)", item.Name, atk));
				}
			}
			return (null, null);
		}

	}
}
