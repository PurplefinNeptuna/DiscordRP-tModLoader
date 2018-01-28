using System.Timers;
using Terraria;

namespace DiscordRP
{
    public static class RPUtility
    {
        public static BitsByte zone1 = new BitsByte();
        public static BitsByte zone2 = new BitsByte();
        public static BitsByte zone3 = new BitsByte();

        public enum ATKType
        {
            melee,
            ranged,
            magic,
            thrown,
            summon
        };

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

            if (zone1[3])
            {
                RPControl.presence.largeImageKey = string.Format("biome_meteor");
                RPControl.presence.largeImageText = string.Format("Meteor");
            }
            else if (zone3[4])
            {
                RPControl.presence.largeImageKey = string.Format("biome_hell");
                RPControl.presence.largeImageText = string.Format("Underworld");
            }
            else if (zone3[0])
            {
                RPControl.presence.largeImageKey = string.Format("biome_sky");
                RPControl.presence.largeImageText = string.Format("Space");
            }
            else if (zone1[6])
            {
                if (zone3[3])
                {
                    RPControl.presence.largeImageKey = string.Format("biome_ucrimson");
                    RPControl.presence.largeImageText = string.Format("Underground Crimson");
                }
                else
                {
                    RPControl.presence.largeImageKey = string.Format("biome_crimson");
                    RPControl.presence.largeImageText = string.Format("Crimson");
                }
            }
            else if (zone1[1])
            {
                if (zone3[3])
                {
                    RPControl.presence.largeImageKey = string.Format("biome_ucorrupt");
                    RPControl.presence.largeImageText = string.Format("Underground Corruption");
                }
                else
                {
                    RPControl.presence.largeImageKey = string.Format("biome_corrupt");
                    RPControl.presence.largeImageText = string.Format("Corruption");
                }
            }
            else if (zone1[2])
            {
                if (zone3[3])
                {
                    RPControl.presence.largeImageKey = string.Format("biome_uholy");
                    RPControl.presence.largeImageText = string.Format("Underground Hollow");
                }
                else
                {
                    RPControl.presence.largeImageKey = string.Format("biome_holy");
                    RPControl.presence.largeImageText = string.Format("Hollow");
                }
            }
            else if (zone1[0])
            {
                RPControl.presence.largeImageKey = string.Format("biome_dungeon");
                RPControl.presence.largeImageText = string.Format("Dungeon");
            }
            else if (zone1[5])
            {
                if (zone3[3])
                {
                    RPControl.presence.largeImageKey = string.Format("biome_usnow");
                    RPControl.presence.largeImageText = string.Format("Underground Snow");
                }
                else
                {
                    RPControl.presence.largeImageKey = string.Format("biome_snow");
                    RPControl.presence.largeImageText = string.Format("Snow");
                }
            }
            else if (zone2[7])
            {
                RPControl.presence.largeImageKey = string.Format("biome_udesert");
                RPControl.presence.largeImageText = string.Format("Underground Desert");
            }
            else if (zone2[5])
            {
                RPControl.presence.largeImageKey = string.Format("biome_desert");
                RPControl.presence.largeImageText = string.Format("Desert");
            }
            else if (zone1[4])
            {
                if (zone3[3]||zone3[2])
                {
                    RPControl.presence.largeImageKey = string.Format("biome_ujungle");
                    RPControl.presence.largeImageText = string.Format("Underground Jungle");
                }
                else
                {
                    RPControl.presence.largeImageKey = string.Format("biome_jungle");
                    RPControl.presence.largeImageText = string.Format("Jungle");
                }
            }
            else if (zone2[6])
            {
                if (zone3[3] || zone3[2])
                {
                    RPControl.presence.largeImageKey = string.Format("biome_umushroom");
                    RPControl.presence.largeImageText = string.Format("Underground Mushroom");
                }
                else
                {
                    RPControl.presence.largeImageKey = string.Format("biome_mushroom");
                    RPControl.presence.largeImageText = string.Format("Mushroom");
                }
            }
            else if (zone3[5])
            {
                RPControl.presence.largeImageKey = string.Format("biome_ocean");
                RPControl.presence.largeImageText = string.Format("Ocean");
            }
            else if (zone3[3])
            {
                RPControl.presence.largeImageKey = string.Format("biome_cavern");
                RPControl.presence.largeImageText = string.Format("Cavern");
            }
            else if (zone3[2])
            {
                RPControl.presence.largeImageKey = string.Format("biome_underground");
                RPControl.presence.largeImageText = string.Format("Underground");
            }
            else
            {
                RPControl.presence.largeImageKey = string.Format("biome_forest");
                RPControl.presence.largeImageText = string.Format("Forest");
            }


            RPControl.Update();
        }

    }
}
