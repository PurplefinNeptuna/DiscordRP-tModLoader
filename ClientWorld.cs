using Terraria.ID;
using Terraria.ModLoader;

namespace DiscordRP {
	public class ClientWorld : ModWorld {

		public static bool beeHive;
		public static bool graniteCave;
		public static bool jungleTemple;
		public static bool marbleCave;
		public static bool spiderCave;
		public static bool cloud;
		public static bool dirt;

		public override void TileCountsAvailable(int[] tileCounts) {
			beeHive = tileCounts[TileID.Hive] > 40;
			graniteCave = tileCounts[TileID.Granite] > 100;
			jungleTemple = tileCounts[TileID.LihzahrdBrick] > 100;
			marbleCave = tileCounts[TileID.Marble] > 100;
			spiderCave = tileCounts[TileID.Cobweb] > 100;
			cloud = (tileCounts[TileID.Cloud] + tileCounts[TileID.RainCloud]) > 40;
			dirt = tileCounts[TileID.Dirt] > 20;
		}
	}
}
