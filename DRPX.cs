using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;

namespace DiscordRP {
	public static class DRPX {
		public static List<int> ObjectToListInt(object data) => data is List<int> ? data as List<int> : (data is int ? new List<int>() { Convert.ToInt32(data) } : null);
		public static int ObjectToInt(object data, int def) => data is int ? (int)data : def;

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
		/// <param name="ids">NPC id list</param>
		/// <param name="imageKey">image key, and image name</param>
		public static void AddBoss(List<int> ids, (string, string) imageKey, int priority = 250) {
			if(ids == null)
				return;

			//imageKey.Item2 = "Fighting " + imageKey.Item2;
			foreach(int id in ids) {
				if(imageKey.Item1 == null || imageKey.Item1 == "") {
					imageKey.Item1 = "boss_placeholder";
				}
				if(DiscordRP.exBossIDtoDetails != null || DiscordRP.exBossIDtoDetails?.Count > 0) {
					DiscordRP.exBossIDtoDetails.Add(id, (imageKey.Item1, imageKey.Item2, priority));
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
				imageKey.Item1 = "biome_placeholder";
			}
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
					//e.g Call("AddBoss", List<int>Id, "Angry Slimey", "boss_placeholder", int default:250)
					case "AddBoss": {
						List<int> Id = ObjectToListInt(args[1]);
						(string, string) ImageKey = (args[3] as string, args[2] as string);
						int priority = ObjectToInt(args[4], 250);
						//int priority = args[4] is int ? (int)args[4] : 250;
						AddBoss(Id, ImageKey, priority);
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
						return (biome.largeKey, "In " + biome.largeText);
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

			largeImageText = "In " + largeImageText;
			return (largeImageKey, largeImageText);
		}

		public static (string, string) GetBoss(BitsByte zone1, BitsByte zone2, BitsByte zone3) {
			string largeImageKey = null;
			string largeImageText = null;

			bool getAnyBosses = false;
			//Main.NewText(DiscordRP.exBossIDtoDetails.Count);
			if(DiscordRP.exBossIDtoDetails != null || DiscordRP.exBossIDtoDetails?.Count > 0) {
				//new way with sort support
				int lastHighestPriority = -1;
				List<int> bossNPCs = Main.npc?.Take(200).Where(npc => npc.active && DiscordRP.exBossIDtoDetails.ContainsKey(npc.type)).Select(x => x.type).ToList();
				foreach(int bossType in bossNPCs) {
					(string, string, int) details = DiscordRP.exBossIDtoDetails[bossType];
					if(details.Item3 >= lastHighestPriority) {
						getAnyBosses = true;
						(largeImageKey, largeImageText, _) = details;
						lastHighestPriority = details.Item3;
					}
				}
			}

			if(getAnyBosses) {
				largeImageText = "Fighting " + largeImageText;
			}
			else {
				(largeImageKey, largeImageText) = GetBiome(zone1, zone2, zone3);
			}
			return (largeImageKey, largeImageText);
		}

		public static void AddVanillaBosses() {
			AddBoss(new List<int>() {
				NPCID.KingSlime
			}, ("boss_kingslime", "King Slime"), 12);
			AddBoss(new List<int>() {
				NPCID.EyeofCthulhu
			}, ("boss_eoc", "Eye of Cthulhu"), 25);
			AddBoss(new List<int>() {
				NPCID.EaterofWorldsHead,
				NPCID.EaterofWorldsBody,
				NPCID.EaterofWorldsTail
			}, ("boss_eow", "Eater of Worlds"), 37);
			AddBoss(new List<int>() {
				NPCID.BrainofCthulhu
			}, ("boss_boc", "Brain of Cthulhu"), 50);
			AddBoss(new List<int>() {
				NPCID.QueenBee
			}, ("boss_queenbee", "Queen Bee"), 62);
			AddBoss(new List<int>() {
				NPCID.SkeletronHead
			}, ("boss_skeletron", "Skeletron"), 75);
			AddBoss(new List<int>() {
				NPCID.WallofFlesh
			}, ("boss_wof", "Wall of Flesh"), 100);
			//hardmode
			AddBoss(new List<int>() {
				NPCID.Retinazer,
				NPCID.Spazmatism
			}, ("boss_twins", "The Twins"), 117);
			AddBoss(new List<int>() {
				NPCID.TheDestroyer
			}, ("boss_destroyer", "The Destroyer"), 133);
			AddBoss(new List<int>() {
				NPCID.SkeletronPrime
			}, ("boss_prime", "Skeleton Prime"), 150);
			AddBoss(new List<int>() {
				NPCID.Plantera
			}, ("boss_plantera", "Plantera"), 167);
			AddBoss(new List<int>() {
				NPCID.Golem
			}, ("boss_golem", "Golem"), 183);
			AddBoss(new List<int>() {
				NPCID.DukeFishron
			}, ("boss_fishron", "Duke Fishron"), 200);
			AddBoss(new List<int>() {
				NPCID.CultistBoss
			}, ("boss_lunatic", "Lunatic Cultist"), 217);
			AddBoss(new List<int>() {
				NPCID.MoonLordHead,
				NPCID.MoonLordHand,
				NPCID.MoonLordCore
			}, ("boss_moonlord", "Moon Lord"), 233);
		}
	}
}
