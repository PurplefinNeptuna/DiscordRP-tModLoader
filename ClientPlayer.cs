using DiscordRPC;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;

namespace DiscordRP {
	class ClientPlayer : ModPlayer {
		private bool dead = false;
		private string worldStaticInfo = "";

		private List<int> bossID = new List<int>() {
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
			NPC bossNPC = Main.npc.Take(200).Where(npc => npc.active && (bossID.Contains(npc.type) || npc.boss)).LastOrDefault();
			(string itemKey, string itemText) = GetItemStat();
			(string bigKey, string bigText) = (bossNPC==null) ? GetBiome(player.zone1, player.zone2, player.zone3) : GetBoss(bossNPC.type);

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

		private (string, string) GetBiome(BitsByte zone1, BitsByte zone2, BitsByte zone3) {
			string largeImageKey;
			string largeImageText;
			if(zone1[3]) {
				largeImageKey = string.Format("biome_meteor");
				largeImageText = string.Format("Meteor ({0})", Main.dayTime ? "Day" : "Night");
			}
			else if(zone3[4]) {
				largeImageKey = string.Format("biome_hell");
				largeImageText = string.Format("Underworld ({0})", Main.dayTime ? "Day" : "Night");
			}
			else if(zone3[0]) {
				largeImageKey = string.Format("biome_sky");
				largeImageText = string.Format("Space ({0})", Main.dayTime ? "Day" : "Night");
			}
			else if(zone1[6]) {
				if(zone3[3]) {
					largeImageKey = string.Format("biome_ucrimson");
					largeImageText = string.Format("Underground Crimson ({0})", Main.dayTime ? "Day" : "Night");
				}
				else {
					largeImageKey = string.Format("biome_crimson");
					largeImageText = string.Format("Crimson ({0})", Main.dayTime ? "Day" : "Night");
				}
			}
			else if(zone1[1]) {
				if(zone3[3]) {
					largeImageKey = string.Format("biome_ucorrupt");
					largeImageText = string.Format("Underground Corruption ({0})", Main.dayTime ? "Day" : "Night");
				}
				else {
					largeImageKey = string.Format("biome_corrupt");
					largeImageText = string.Format("Corruption ({0})", Main.dayTime ? "Day" : "Night");
				}
			}
			else if(zone1[2]) {
				if(zone3[3]) {
					largeImageKey = string.Format("biome_uholy");
					largeImageText = string.Format("Underground Hollow ({0})", Main.dayTime ? "Day" : "Night");
				}
				else {
					largeImageKey = string.Format("biome_holy");
					largeImageText = string.Format("Hollow ({0})", Main.dayTime ? "Day" : "Night");
				}
			}
			else if(zone1[0]) {
				largeImageKey = string.Format("biome_dungeon");
				largeImageText = string.Format("Dungeon ({0})", Main.dayTime ? "Day" : "Night");
			}
			else if(zone1[5]) {
				if(zone3[3]) {
					largeImageKey = string.Format("biome_usnow");
					largeImageText = string.Format("Underground Snow ({0})", Main.dayTime ? "Day" : "Night");
				}
				else {
					largeImageKey = string.Format("biome_snow");
					largeImageText = string.Format("Snow ({0})", Main.dayTime ? "Day" : "Night");
				}
			}
			else if(zone2[7]) {
				largeImageKey = string.Format("biome_udesert");
				largeImageText = string.Format("Underground Desert ({0})", Main.dayTime ? "Day" : "Night");
			}
			else if(zone2[5]) {
				largeImageKey = string.Format("biome_desert");
				largeImageText = string.Format("Desert ({0})", Main.dayTime ? "Day" : "Night");
			}
			else if(zone1[4]) {
				if(zone3[3] || zone3[2]) {
					largeImageKey = string.Format("biome_ujungle");
					largeImageText = string.Format("Underground Jungle ({0})", Main.dayTime ? "Day" : "Night");
				}
				else {
					largeImageKey = string.Format("biome_jungle");
					largeImageText = string.Format("Jungle ({0})", Main.dayTime ? "Day" : "Night");
				}
			}
			else if(zone2[6]) {
				if(zone3[3] || zone3[2]) {
					largeImageKey = string.Format("biome_umushroom");
					largeImageText = string.Format("Underground Mushroom ({0})", Main.dayTime ? "Day" : "Night");
				}
				else {
					largeImageKey = string.Format("biome_mushroom");
					largeImageText = string.Format("Mushroom ({0})", Main.dayTime ? "Day" : "Night");
				}
			}
			else if(zone3[5]) {
				largeImageKey = string.Format("biome_ocean");
				largeImageText = string.Format("Ocean ({0})", Main.dayTime ? "Day" : "Night");
			}
			else if(zone3[3]) {
				largeImageKey = string.Format("biome_cavern");
				largeImageText = string.Format("Cavern ({0})", Main.dayTime ? "Day" : "Night");
			}
			else if(zone3[2]) {
				largeImageKey = string.Format("biome_underground");
				largeImageText = string.Format("Underground ({0})", Main.dayTime ? "Day" : "Night");
			}
			else {
				largeImageKey = string.Format("biome_forest");
				largeImageText = string.Format("Forest ({0})", Main.dayTime ? "Day" : "Night");
			}

			return (largeImageKey, largeImageText);
		}

		private (string, string) GetBoss(int bossType) {
			string largeImageKey;
			string largeImageText;
			switch(bossType) {
				case (50):
					largeImageKey = string.Format("boss_kingslime");
					largeImageText = string.Format("King Slime");
					break;
				case (4):
					largeImageKey = string.Format("boss_eoc");
					largeImageText = string.Format("Eye of Cthulhu");
					break;
				case (13):
				case (14):
				case (15):
					largeImageKey = string.Format("boss_eow");
					largeImageText = string.Format("Eater of Worlds");
					break;
				case (266):
					largeImageKey = string.Format("boss_boc");
					largeImageText = string.Format("Brain of Cthulhu");
					break;
				case (222):
					largeImageKey = string.Format("boss_queenbee");
					largeImageText = string.Format("Queen Bee");
					break;
				case (35):
					largeImageKey = string.Format("boss_skeletron");
					largeImageText = string.Format("Skeletron");
					break;
				case (113):
					largeImageKey = string.Format("boss_wof");
					largeImageText = string.Format("Wall of Flesh");
					break;
				case (125):
				case (126):
					largeImageKey = string.Format("boss_twins");
					largeImageText = string.Format("The Twins");
					break;
				case (134):
					largeImageKey = string.Format("boss_destroyer");
					largeImageText = string.Format("The Destroyer");
					break;
				case (127):
					largeImageKey = string.Format("boss_prime");
					largeImageText = string.Format("Skeletron Prime");
					break;
				case (262):
					largeImageKey = string.Format("boss_plantera");
					largeImageText = string.Format("Plantera");
					break;
				case (245):
					largeImageKey = string.Format("boss_golem");
					largeImageText = string.Format("Golem");
					break;
				case (370):
					largeImageKey = string.Format("boss_fishron");
					largeImageText = string.Format("Duke Fishron");
					break;
				case (439):
					largeImageKey = string.Format("boss_lunatic");
					largeImageText = string.Format("Lunatic Cultist");
					break;
				case (396):
				case (397):
				case (398):
					largeImageKey = string.Format("boss_moonlord");
					largeImageText = string.Format("Moon Lord");
					break;
				default:
					(largeImageKey, largeImageText) = GetBiome(player.zone1, player.zone2, player.zone3);
					break;
			};

			return (largeImageKey,largeImageText);
		}
	}
}
