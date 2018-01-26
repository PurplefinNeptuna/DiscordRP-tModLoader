using System.Timers;

namespace DiscordRP
{
    public static class RPUtility
    {
        public enum Biomes
        {
            corrupt,
            crimson,
            jungle,
            desert,
            snow,
            ocean,
            forest
        };

        public enum ATKType
        {
            melee,
            ranged,
            magic,
            thrown,
            summon
        };

        public static Biomes biome = Biomes.forest;

        public static int life = 0;
        public static int mana = 0;
        public static int def = 0;

        public static int atk = 0;
        public static ATKType type = ATKType.melee;

        public static void Update()
        {
            RPControl.presence.state = string.Format("HP: {0} MP: {1} DEF: {2}", life, mana, def);
            
            switch (type)
            {
                case ATKType.melee:
                    RPControl.presence.smallImageKey = string.Format("atk_melee");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Melee)", atk);
                    break;
                case ATKType.ranged:
                    RPControl.presence.smallImageKey = string.Format("atk_range");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Ranged)", atk);
                    break;
                case ATKType.magic:
                    RPControl.presence.smallImageKey = string.Format("atk_magic");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Magic)", atk);
                    break;
                case ATKType.thrown:
                    RPControl.presence.smallImageKey = string.Format("atk_throw");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Thrown)", atk);
                    break;
                case ATKType.summon:
                    RPControl.presence.smallImageKey = string.Format("atk_summon");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Summon)", atk);
                    break;
            }

            switch (biome)
            {
                case Biomes.corrupt:
                    RPControl.presence.largeImageKey = string.Format("biome_corrupt");
                    RPControl.presence.largeImageText = string.Format("Biome: Corruption");
                    break;
                case Biomes.crimson:
                    RPControl.presence.largeImageKey = string.Format("biome_crimson");
                    RPControl.presence.largeImageText = string.Format("Biome: Crimson");
                    break;
                case Biomes.jungle:
                    RPControl.presence.largeImageKey = string.Format("biome_jungle");
                    RPControl.presence.largeImageText = string.Format("Biome: Jungle");
                    break;
                case Biomes.desert:
                    RPControl.presence.largeImageKey = string.Format("biome_desert");
                    RPControl.presence.largeImageText = string.Format("Biome: Desert");
                    break;
                case Biomes.snow:
                    RPControl.presence.largeImageKey = string.Format("biome_snow");
                    RPControl.presence.largeImageText = string.Format("Biome: Snow");
                    break;
                case Biomes.ocean:
                    RPControl.presence.largeImageKey = string.Format("biome_ocean");
                    RPControl.presence.largeImageText = string.Format("Biome: Ocean");
                    break;
                case Biomes.forest:
                    RPControl.presence.largeImageKey = string.Format("biome_forest");
                    RPControl.presence.largeImageText = string.Format("Biome: Forest");
                    break;
            }

            RPControl.Update();
        }

    }
}
