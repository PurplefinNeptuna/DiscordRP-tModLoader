using Terraria;
using Terraria.ModLoader;

namespace DiscordRP
{
    class Testplayer : ModPlayer
    {        

        public override void OnEnterWorld(Player someone)
        {
            string wName = Main.worldName;
            bool expert = Main.expertMode;
            string wDiff = (expert) ? "(Expert)" : "(Normal)";
            RPControl.presence.details = string.Format("Playing {0} {1}", wName, wDiff);
            RPControl.canUpdate = true;
            RPControl.Update();
            RPUtility.inWorld = true;
            RPUtility.StartTimer();
        }

        public override void PostUpdate()
        {
            RPUtility.life = player.statLife;
            RPUtility.mana = player.statMana;
            RPUtility.def = player.statDefense;

            if (player.ZoneCorrupt)
                RPUtility.biome = RPUtility.Biomes.corrupt;
            else if (player.ZoneCrimson)
                RPUtility.biome = RPUtility.Biomes.crimson;
            else if (player.ZoneJungle)
                RPUtility.biome = RPUtility.Biomes.jungle;
            else if (player.ZoneDesert)
                RPUtility.biome = RPUtility.Biomes.desert;
            else if (player.ZoneSnow)
                RPUtility.biome = RPUtility.Biomes.snow;
            else if (player.ZoneBeach)
                RPUtility.biome = RPUtility.Biomes.ocean;
            else
                RPUtility.biome = RPUtility.Biomes.forest;
        }

    }
}
