using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;

namespace DiscordRP {
	public static class DRPX {
		public static List<int> ObjectToListInt(object data) => data is List<int> ? data as List<int> : (data is int ? new List<int>() { Convert.ToInt32(data) } : null);
		public static float ObjectToFloat(object data, float def) => data is float single ? single : def;
		static Player LPlayer => Main.LocalPlayer;
		static ClientPlayer LCPlayer => Main.LocalPlayer?.GetModPlayer<ClientPlayer>();

		static ILog Logger => DiscordRP.Instance.Logger;

		/// <summary>
		/// Override main menu status
		/// </summary>
		/// <param name="details">upper text</param>
		/// <param name="additionalDetails">lower text</param>
		/// <param name="largeImage">key and text for large image</param>
		/// <param name="smallImage">key and text for small image</param>
		public static void NewMenuStatus(string details, string additionalDetails, (string, string) largeImage, (string, string) smallImage) {
			DiscordRP.Instance.customStatus = new DRPStatus() {
				details = details,
				additionalDetails = additionalDetails,
				largeKey = (string.IsNullOrWhiteSpace(largeImage.Item1)) ? "mod_placeholder" : largeImage.Item1,
				largeImage = largeImage.Item2,
				smallKey = (string.IsNullOrWhiteSpace(smallImage.Item1)) ? null : smallImage.Item1,
				smallImage = smallImage.Item2,
			};
		}

		/// <summary>
		/// Add new npc to boss list to detect
		/// </summary>
		/// <param name="ids">the npc ids</param>
		/// <param name="imageKey">image key</param>
		/// <param name="priority">priority</param>
		/// <param name="client">discord app id key</param>
		public static void AddBoss(List<int> ids, (string, string) imageKey, float priority = 16f, string client = "default") {
			if(ids == null)
				return;

			Logger.Info($"Adding boss {imageKey.Item2} in {client} Instance...");
			if(!DiscordRP.Instance.savedDiscordAppId.ContainsKey(client)) {
				Logger.Error($"Instance {client} not found, redirected to default Instance!");
				client = "default";
			}
			foreach(int id in ids) {
				if(string.IsNullOrWhiteSpace(imageKey.Item1)) {
					imageKey.Item1 = "boss_placeholder";
				}
				if(DiscordRP.Instance.exBossIDtoDetails != null || DiscordRP.Instance.exBossIDtoDetails?.Count > 0) {
					DiscordRP.Instance.exBossIDtoDetails.Add(id, (imageKey.Item1, imageKey.Item2, client, priority));
				}
				else {
					Logger.Error($"Failed to add boss {imageKey.Item2} custom status info, report to Purplefin Neptuna");
				}
			}
		}

		/// <summary>
		/// Add new biome/event to detect
		/// </summary>
		/// <param name="checker">function to check, detect biome/event if returns true</param>
		/// <param name="imageKey">image key</param>
		/// <param name="priority">priority</param>
		/// <param name="client">discord app id key</param>
		public static void AddBiome(Func<bool> checker, (string, string) imageKey, float priority = 50f, string client = "default") {
			if(string.IsNullOrWhiteSpace(imageKey.Item1)) {
				imageKey.Item1 = "biome_placeholder";
			}
			Logger.Info($"Adding biome {imageKey.Item2} in {client} Instance...");
			if(!DiscordRP.Instance.savedDiscordAppId.ContainsKey(client)) {
				Logger.Error($"Instance {client} not found, redirected to default Instance!");
				client = "default";
			}
			if(DiscordRP.Instance.exBiomeStatus != null || DiscordRP.Instance.exBiomeStatus?.Count > 0) {
				DiscordRP.Instance.exBiomeStatus.Add(new BiomeStatus() { checker = checker, largeKey = imageKey.Item1, largeText = imageKey.Item2, priority = priority, client = client });
			}
			else {
				Logger.Error($"Failed to add biome {imageKey.Item2} custom status info, report to Purplefin Neptuna");
			}
		}

		/// <summary>
		///	Call
		/// </summary>
		/// <param name="args"></param>
		/// <returns>String of call results</returns>
		public static object Call(params object[] args) {
			if(DiscordRP.Instance == null) {
				return "Failure";
			}
			int arglen = args.Length;
			Array.Resize(ref args, 15);
			try {
				string message = args[0] as string;
				switch(message) {
					//e.g Call("AddBoss", List<int>Id, "Angry Slimey", "boss_placeholder", float default:16f, client="default")
					case "AddBoss": {
						List<int> Id = ObjectToListInt(args[1]);
						(string, string) ImageKey = (args[3] as string, args[2] as string);
						float priority = ObjectToFloat(args[4], 16f);
						string client = args[5] is string ? args[5] as string : "default";

						AddBoss(Id, ImageKey, priority, client);
						return "Success";
					}

					//e.g Call("AddBiome", Func<bool> checker, "SlimeFire Biome", "biome_placeholder", float default:50f/150f, client="default")
					case "AddEvent":
					case "AddBiome": {
						Func<bool> checker = args[1] as Func<bool>;
						(string, string) ImageKey = (args[3] as string, args[2] as string);
						float priority = ObjectToFloat(args[4], message == "AddBiome" ? 50f : 150f);
						string client = args[5] is string ? args[5] as string : "default";
						AddBiome(checker, ImageKey, priority, client);
						return "Success";
					}

					//e.g. Call("MainMenu", "details", "belowDetails", "mod_placeholder", "modName")
					case "MainMenu":
					case "MainMenuOverride": {
						if(!DiscordRP.Instance.canCreateClient) {
							return "Failure";
						}
						string details = args[1] as string;
						string additionalDetails = args[2] as string;
						(string, string) largeImage = (args[3] as string, args[4] as string);
						(string, string) smallImage = (args[5] as string, args[6] as string);
						NewMenuStatus(details, additionalDetails, largeImage, smallImage);
						return "Success";
					}

					//e.g. Call("AddClient", "716207249902796810", "angryslimey")
					case "AddClient":
					case "AddNewClient":
					case "NewClient":
					case "AddDiscordClient": {
						if(arglen != 3 || !DiscordRP.Instance.canCreateClient) {
							return "Failure";
						}
						string newID = args[1] as string;
						string idKey = args[2] as string;
						DiscordRP.Instance?.AddDiscordAppID(idKey, newID);
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

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public static (string, string, string) GetBiome() {
			string largeImageKey = null;
			string largeImageText = null;
			string selectedClient = "default";

			if(DiscordRP.Instance.exBiomeStatus != null || DiscordRP.Instance.exBiomeStatus?.Count > 0) {
				float lastHighestPriority = -1f;
				foreach(BiomeStatus biome in DiscordRP.Instance.exBiomeStatus) {
					if(biome.checker() && biome.priority >= lastHighestPriority) {
						lastHighestPriority = biome.priority;
						largeImageKey = biome.largeKey;
						largeImageText = "In " + biome.largeText + $" ({(Main.dayTime ? "Day" : "Night")})";
						selectedClient = biome.client;
					}
				}
			}
			//DiscordRP.Instance.ChangeDiscordClient(selectedClient);
			return (largeImageKey, largeImageText, selectedClient);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public static (string, string, string) GetBoss() {
			string largeImageKey = null;
			string largeImageText = null;
			string selectedClient = "default";
			bool getAnyBosses = false;

			if(DiscordRP.Instance.exBossIDtoDetails != null || DiscordRP.Instance.exBossIDtoDetails?.Count > 0) {
				//new way with priority support
				float lastHighestPriority = -1f;
				List<int> bossNPCs = Main.npc?.Take(200).Where(npc => npc.active && DiscordRP.Instance.exBossIDtoDetails.ContainsKey(npc.type)).Select(x => x.type).ToList();
				foreach(int bossType in bossNPCs) {
					(string, string, string, float) details = DiscordRP.Instance.exBossIDtoDetails[bossType];
					if(details.Item4 >= lastHighestPriority) {
						getAnyBosses = true;
						(largeImageKey, largeImageText, selectedClient, _) = details;
						lastHighestPriority = details.Item4;
					}
				}
			}

			if(getAnyBosses) {
				largeImageText = "Fighting " + largeImageText;
				//DiscordRP.Instance.ChangeDiscordClient(selectedClient);
			}
			else {
				(largeImageKey, largeImageText, selectedClient) = GetBiome();
			}
			return (largeImageKey, largeImageText, selectedClient);
		}

		public static void AddVanillaEvents() {
			AddBiome(
				() => LCPlayer?.nearbyNPC >= 3 && BirthdayParty.PartyIsUp,
				("event_party", "Party"), 90f
			);
			AddBiome(
				() => Sandstorm.Happening && LPlayer.ZoneDesert,
				("event_sandstorm", "Sandstorm"), 91f
			);
			AddBiome(
				() => Main.slimeRain && LPlayer.ZoneOverworldHeight,
				("event_slime", "Slime Rain"), 91f
			);
			AddBiome(
				() => DD2Event.Ongoing && LPlayer.ZoneOverworldHeight,
				("event_oldone", "Old One's Army"), 95f
			);
			AddBiome(
				() => Main.invasionType == 1 && Main.invasionSize > 0 && LPlayer.ZoneOverworldHeight,
				("event_goblin", "Goblin Invasion"), 95f
			);
			AddBiome(
				() => Main.bloodMoon && LPlayer.ZoneOverworldHeight && !Main.dayTime,
				("event_bloodmoon", "Blood Moon"), 96f
			);
			AddBiome(
				() => Main.invasionType == 2 && Main.invasionSize > 0 && LPlayer.ZoneOverworldHeight,
				("event_frostlegion", "Frost Legion"), 100f
			);
			AddBiome(
				() => Main.invasionType == 3 && Main.invasionSize > 0 && LPlayer.ZoneOverworldHeight,
				("event_pirate", "Pirate Invasion"), 110f
			);
			AddBiome(
				() => Main.pumpkinMoon && LPlayer.ZoneOverworldHeight && !Main.dayTime,
				("event_pumpkinmoon", "Pumpkin Moon"), 115f
			);
			AddBiome(
				() => Main.snowMoon && LPlayer.ZoneOverworldHeight && !Main.dayTime,
				("event_frostmoon", "Frost Moon"), 115f
			);
			AddBiome(
				() => Main.eclipse && LPlayer.ZoneOverworldHeight && Main.dayTime,
				("event_eclipse", "Solar Eclipse"), 115f
			);
			AddBiome(
				() => Main.invasionType == 4 && Main.invasionSize > 0 && LPlayer.ZoneOverworldHeight,
				("event_martian", "Martian Madness"), 120f
			);
			AddBiome(
				() => LPlayer.ZoneTowerSolar,
				("event_solarmoon", "Solar Pillar area"), 130f
			);
			AddBiome(
				() => LPlayer.ZoneTowerVortex,
				("event_vortexmoon", "Vortex Pillar area"), 130f
			);
			AddBiome(
				() => LPlayer.ZoneTowerNebula,
				("event_nebulamoon", "Nebula Pillar area"), 130f
			);
			AddBiome(
				() => LPlayer.ZoneTowerStardust,
				("event_stardustmoon", "Stardust Pillar area"), 130f
			);
		}

		public static void AddVanillaBiomes() {
			AddBiome(
				() => true,
				("biome_forest", "Forest"), 0f
			);
			AddBiome(
				() => LPlayer.ZoneDirtLayerHeight,
				("biome_underground", "Underground"), 1f
			);
			AddBiome(
				() => LPlayer.ZoneRockLayerHeight,
				("biome_cavern", "Cavern"), 2f
			);
			AddBiome(
				() => LPlayer.ZoneBeach,
				("biome_ocean", "Ocean"), 3f
			);
			AddBiome(
				() => LPlayer.ZoneGlowshroom,
				("biome_mushroom", "Mushroom"), 4f
			);
			AddBiome(
				() => LPlayer.ZoneJungle,
				("biome_jungle", "Jungle"), 5f
			);
			AddBiome(
				() => LPlayer.ZoneJungle && (LPlayer.ZoneDirtLayerHeight || LPlayer.ZoneRockLayerHeight),
				("biome_ujungle", "Underground Jungle"), 6f
			);
			AddBiome(
				() => LPlayer.ZoneDesert,
				("biome_desert", "Desert"), 7f
			);
			AddBiome(
				() => LPlayer.ZoneUndergroundDesert,
				("biome_udesert", "Underground Desert"), 8f
			);
			AddBiome(
				() => LPlayer.ZoneSnow,
				("biome_snow", "Snow"), 9f
			);
			AddBiome(
				() => LPlayer.ZoneSnow && LPlayer.ZoneRockLayerHeight,
				("biome_usnow", "Underground Snow"), 10f
			);
			AddBiome(
				() => LPlayer.ZoneHoly,
				("biome_holy", "Hollow"), 11f
			);
			AddBiome(
				() => LPlayer.ZoneHoly && LPlayer.ZoneRockLayerHeight,
				("biome_uholy", "Underground Hollow"), 12f
			);
			AddBiome(
				() => LPlayer.ZoneCorrupt,
				("biome_corrupt", "Corruption"), 13f
			);
			AddBiome(
				() => LPlayer.ZoneCorrupt && LPlayer.ZoneRockLayerHeight,
				("biome_ucorrupt", "Underground Corruption"), 14f
			);
			AddBiome(
				() => LPlayer.ZoneCrimson,
				("biome_crimson", "Crimson"), 15f
			);
			AddBiome(
				() => LPlayer.ZoneCrimson && LPlayer.ZoneRockLayerHeight,
				("biome_ucrimson", "Underground Crimson"), 16f
			);
			AddBiome(
				() => ClientWorld.spiderCave,
				("biome_spider", "Spider Cave"), 17f
			);
			AddBiome(
				() => LPlayer.ZoneGlowshroom && (LPlayer.ZoneDirtLayerHeight || LPlayer.ZoneRockLayerHeight),
				("biome_umushroom", "Underground Mushroom"), 18f
			);
			AddBiome(
				() => ClientWorld.graniteCave,
				("biome_granite", "Granite Cave"), 19f
			);
			AddBiome(
				() => ClientWorld.marbleCave,
				("biome_marble", "Marble Cave"), 20f
			);
			AddBiome(
				() => ClientWorld.beeHive,
				("biome_beehive", "Bee Hive"), 21f
			);
			AddBiome(
				() => LPlayer.ZoneDungeon,
				("biome_dungeon", "Dungeon"), 22f
			);
			AddBiome(
				() => ClientWorld.jungleTemple,
				("biome_temple", "Jungle Temple"), 23f
			);
			AddBiome(
				() => LPlayer.ZoneSkyHeight,
				("biome_sky", "Space"), 24f
			);
			AddBiome(
				() => ClientWorld.cloud,
				("biome_skylake", "Floating Lake"), 25f
			);
			AddBiome(
				() => ClientWorld.cloud && ClientWorld.dirt,
				("biome_skyisland", "Floating Island"), 26f
			);
			AddBiome(
				() => LPlayer.ZoneUnderworldHeight,
				("biome_hell", "Underworld"), 27f
			);
			AddBiome(
				() => LPlayer.ZoneMeteor,
				("biome_meteor", "Meteor"), 28f
			);
			AddBiome(
				() => LCPlayer?.nearbyNPC >= 3,
				("biome_town", "Town"), 29f
			);
		}

		public static void AddVanillaBosses() {
			AddBoss(new List<int>() {
				NPCID.KingSlime
			}, ("boss_kingslime", "King Slime"), 1f);
			AddBoss(new List<int>() {
				NPCID.EyeofCthulhu
			}, ("boss_eoc", "Eye of Cthulhu"), 2f);
			AddBoss(new List<int>() {
				NPCID.EaterofWorldsHead,
				NPCID.EaterofWorldsBody,
				NPCID.EaterofWorldsTail
			}, ("boss_eow", "Eater of Worlds"), 3f);
			AddBoss(new List<int>() {
				NPCID.BrainofCthulhu
			}, ("boss_boc", "Brain of Cthulhu"), 4f);
			AddBoss(new List<int>() {
				NPCID.QueenBee
			}, ("boss_queenbee", "Queen Bee"), 5f);
			AddBoss(new List<int>() {
				NPCID.SkeletronHead
			}, ("boss_skeletron", "Skeletron"), 6f);
			AddBoss(new List<int>() {
				NPCID.WallofFlesh
			}, ("boss_wof", "Wall of Flesh"), 7f);
			//hardmode
			AddBoss(new List<int>() {
				NPCID.Retinazer,
				NPCID.Spazmatism
			}, ("boss_twins", "The Twins"), 8f);
			AddBoss(new List<int>() {
				NPCID.TheDestroyer
			}, ("boss_destroyer", "The Destroyer"), 9f);
			AddBoss(new List<int>() {
				NPCID.SkeletronPrime
			}, ("boss_prime", "Skeleton Prime"), 10f);
			AddBoss(new List<int>() {
				NPCID.Plantera
			}, ("boss_plantera", "Plantera"), 11f);
			AddBoss(new List<int>() {
				NPCID.Golem
			}, ("boss_golem", "Golem"), 12f);
			AddBoss(new List<int>() {
				NPCID.DukeFishron
			}, ("boss_fishron", "Duke Fishron"), 13f);
			AddBoss(new List<int>() {
				NPCID.CultistBoss
			}, ("boss_lunatic", "Lunatic Cultist"), 14f);
			AddBoss(new List<int>() {
				NPCID.MoonLordHead,
				NPCID.MoonLordHand,
				NPCID.MoonLordCore
			}, ("boss_moonlord", "Moon Lord"), 15f);
		}
	}
}
