using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace DiscordRP {
	public static class DRPX {
		public static List<int> ObjectToListInt(object data) => data is List<int> ? data as List<int> : (data is int ? new List<int>() { Convert.ToInt32(data) } : null);

		public static void NewMenuStatus(string details, string additionalDetails, (string, string) largeImage, (string, string) smallImage) {
			DiscordRP.customStatus = new DRPStatus() {
				details = details,
				additionalDetails = additionalDetails,
				largeKey = (largeImage.Item1 == null || largeImage.Item1 == "") ? "mod_placeholder" : largeImage.Item1,
				largeImage = largeImage.Item2,
				smallKey = (smallImage.Item1 == null || smallImage.Item1 == "") ? null : smallImage.Item1,
				smallImage = smallImage.Item2,
			};
		}

		/// <summary>
		/// Add Bosses to Discord Rich Presence
		/// </summary>
		/// <param name="ids">NPC id</param>
		/// <param name="imageKey">image key, and image name</param>
		public static void AddBoss(List<int> ids, (string, string) imageKey) {
			foreach(int id in ids) {
				if(imageKey.Item1 == null || imageKey.Item1 == "") {
					imageKey.Item1 = "boss_placeholder";
				}
				if(DiscordRP.exBossIDtoDetails != null || DiscordRP.exBossIDtoDetails?.Count > 0) {
					DiscordRP.exBossIDtoDetails.Add(id, imageKey);
				}
				else {
					Main.NewText("Failed to add boss custom status info, report to Purplefin Neptuna", Color.Red);
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="checker"></param>
		/// <param name="imageKey"></param>
		public static void AddBiome(Func<bool> checker, (string, string) imageKey) {
			if(imageKey.Item1 == null || imageKey.Item1 == "") {
				imageKey.Item1 = "boss_placeholder";
			}
			//DiscordRP.exBossIDtoDetails.Add(id, imageKey);
			if(DiscordRP.exBiomeStatus != null || DiscordRP.exBiomeStatus?.Count > 0) {
				DiscordRP.exBiomeStatus.Add(new BiomeStatus() { checker = checker, largeKey = imageKey.Item1, largeText = imageKey.Item2 });
			}
			else {
				Main.NewText("Failed to add custom biome status info, report to Purplefin Neptuna", Color.Red);
			}
		}

		/// <summary>
		///	Call
		/// </summary>
		/// <param name="args"></param>
		/// <returns>String of call results</returns>
		public static object Call(params object[] args) {
			Array.Resize(ref args, 15);
			try {
				string message = args[0] as string;
				switch(message) {
					//e.g Call("AddBoss", List<int>Id, "Angry Slimey", "boss_placeholder")
					case "AddBoss": {
						List<int> Id = ObjectToListInt(args[1]);
						(string, string) ImageKey = (args[3] as string, args[2] as string);
						AddBoss(Id, ImageKey);
						return "Success";
					}

					//e.g Call("AddBiome", Func<bool> checker, "SlimeFire Biome", "biome_placeholder")
					case "AddEvent":
					case "AddBiome": {
						Func<bool> checker = args[1] as Func<bool>;
						(string, string) ImageKey = (args[3] as string, args[2] as string);
						AddBiome(checker, ImageKey);
						return "Success";
					}

					//e.g. Call("MainMenu", "details", "belowDetails", "mod_placeholder", "modName")
					case "MainMenu":
					case "MainMenuOverride": {
						string details = args[1] as string;
						string additionalDetails = args[2] as string;
						(string, string) largeImage = (args[3] as string, args[4] as string);
						(string, string) smallImage = (args[5] as string, args[6] as string);
						NewMenuStatus(details, additionalDetails, largeImage, smallImage);
						return "Success";
					}
					default:
						break;
				};
			}
			catch {
			}
			return "Failure";
		}

		public static (string, string) GetBiome(BitsByte zone1, BitsByte zone2, BitsByte zone3) {
			string largeImageKey;
			string largeImageText;

			if(DiscordRP.exBiomeStatus != null || DiscordRP.exBiomeStatus?.Count > 0) {
				foreach(BiomeStatus biome in DiscordRP.exBiomeStatus) {
					if(biome.checker()) {
						return (biome.largeKey, biome.largeText);
					}
				}
			}

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

		public static (string, string) GetBoss(BitsByte zone1, BitsByte zone2, BitsByte zone3) {
			string largeImageKey = null;
			string largeImageText = null;

			//Main.NewText(DiscordRP.exBossIDtoDetails.Count);
			if(DiscordRP.exBossIDtoDetails != null || DiscordRP.exBossIDtoDetails?.Count > 0) {
				NPC bossNPCExtra = Main.npc?.Take(200).Where(npc => npc.active && DiscordRP.exBossIDtoDetails.ContainsKey(npc.type)).LastOrDefault();
				if(bossNPCExtra != null) {
					return DiscordRP.exBossIDtoDetails[bossNPCExtra.type];
				}
			}

			if(DiscordRP.bossID != null || DiscordRP.bossID?.Count > 0) {
				NPC bossNPC = Main.npc?.Take(200).Where(npc => npc.active && (DiscordRP.bossID.Contains(npc.type) || npc.boss)).LastOrDefault();
				if(bossNPC == null)
					return GetBiome(zone1, zone2, zone3);
				switch(bossNPC.type) {
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
						(largeImageKey, largeImageText) = GetBiome(zone1, zone2, zone3);
						break;
				};
			}

			return (largeImageKey, largeImageText);
		}
	}
}
