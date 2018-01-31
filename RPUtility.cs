using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;

namespace DiscordRP
{
    public static class RPUtility
    {
        public static BitsByte zone1 = new BitsByte();
        public static BitsByte zone2 = new BitsByte();
        public static BitsByte zone3 = new BitsByte();

        public static int life = 0;
        public static int mana = 0;
        public static int def = 0;
        public static int atk = 0;

        public static Item item;
        public static Player player;

        /*boss type
        50             King Slime
        4              EoC
        13/14/15       EoW (H/B/T)
        266            BoC
        222            Queen Bee
        35             Skeletron
        113            WoF
        125/126        The Twins (Retinazer/Spazmatism)
        134            The Destroyer
        127            Skeletron Prime
        262            Plantera
        245            Golem
        370            Duke Fishron
        439            Lunatic Cultist
        396/397/398    Moon Lord
        */
        static List<int> bossID = new List<int>() {
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

        static NPC bossNPC;

        public static void GetItemStat()
        {
            if(item != null)
            {
                if (item.melee)
                {
                    atk = (int)Math.Ceiling(item.damage * player.meleeDamage);
                    RPControl.presence.smallImageKey = string.Format("atk_melee");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Melee)", atk);
                }
                else if (item.ranged)
                {
                    atk = (int)Math.Ceiling(item.damage * player.rangedDamage);
                    RPControl.presence.smallImageKey = string.Format("atk_range");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Ranged)", atk);
                }
                else if (item.magic)
                {
                    atk = (int)Math.Ceiling(item.damage * player.magicDamage);
                    RPControl.presence.smallImageKey = string.Format("atk_magic");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Magic)", atk);
                }
                else if (item.thrown)
                {
                    atk = (int)Math.Ceiling(item.damage * player.thrownDamage);
                    RPControl.presence.smallImageKey = string.Format("atk_throw");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Thrown)", atk);
                }
                else if (item.summon)
                {
                    atk = (int)Math.Ceiling(item.damage * player.minionDamage);
                    RPControl.presence.smallImageKey = string.Format("atk_summon");
                    RPControl.presence.smallImageText = string.Format("ATK: {0} (Summon)", atk);
                }
            }
        }

        public static void GetBiome()
        {
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
                if (zone3[3] || zone3[2])
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
        }

        public static void GetBoss()
        {
            switch (bossNPC.type)
            {
                case (50):
                    RPControl.presence.largeImageKey = string.Format("boss_kingslime");
                    RPControl.presence.largeImageText = string.Format("King Slime");
                    break;
                case (4):
                    RPControl.presence.largeImageKey = string.Format("boss_eoc");
                    RPControl.presence.largeImageText = string.Format("Eye of Cthulhu");
                    break;
                case (13):
                case (14):
                case (15):
                    RPControl.presence.largeImageKey = string.Format("boss_eow");
                    RPControl.presence.largeImageText = string.Format("Eater of Worlds");
                    break;
                case (266):
                    RPControl.presence.largeImageKey = string.Format("boss_boc");
                    RPControl.presence.largeImageText = string.Format("Brain of Cthulhu");
                    break;
                case (222):
                    RPControl.presence.largeImageKey = string.Format("boss_queenbee");
                    RPControl.presence.largeImageText = string.Format("Queen Bee");
                    break;
                case (35):
                    RPControl.presence.largeImageKey = string.Format("boss_skeletron");
                    RPControl.presence.largeImageText = string.Format("Skeletron");
                    break;
                case (113):
                    RPControl.presence.largeImageKey = string.Format("boss_wof");
                    RPControl.presence.largeImageText = string.Format("Wall of Flesh");
                    break;
                case (125):
                case (126):
                    RPControl.presence.largeImageKey = string.Format("boss_twins");
                    RPControl.presence.largeImageText = string.Format("The Twins");
                    break;
                case (134):
                    RPControl.presence.largeImageKey = string.Format("boss_destroyer");
                    RPControl.presence.largeImageText = string.Format("The Destroyer");
                    break;
                case (127):
                    RPControl.presence.largeImageKey = string.Format("boss_prime");
                    RPControl.presence.largeImageText = string.Format("Skeletron Prime");
                    break;
                case (262):
                    RPControl.presence.largeImageKey = string.Format("boss_plantera");
                    RPControl.presence.largeImageText = string.Format("Plantera");
                    break;
                case (245):
                    RPControl.presence.largeImageKey = string.Format("boss_golem");
                    RPControl.presence.largeImageText = string.Format("Golem");
                    break;
                case (370):
                    RPControl.presence.largeImageKey = string.Format("boss_fishron");
                    RPControl.presence.largeImageText = string.Format("Duke Fishron");
                    break;
                case (439):
                    RPControl.presence.largeImageKey = string.Format("boss_lunatic");
                    RPControl.presence.largeImageText = string.Format("Lunatic Cultist");
                    break;
                case (396):
                case (397):
                case (398):
                    RPControl.presence.largeImageKey = string.Format("boss_moonlord");
                    RPControl.presence.largeImageText = string.Format("Moon Lord");
                    break;
                default:
                    GetBiome();
                    break;
            };
        }

        public static void Update()
        {
            life = player.statLife;
            mana = player.statMana;
            def = player.statDefense;

            zone1 = player.zone1;
            zone2 = player.zone2;
            zone3 = player.zone3;

            item = player.HeldItem;

            RPControl.presence.state = string.Format("HP: {0} MP: {1} DEF: {2}", life, mana, def);

            GetItemStat();

            bossNPC = Main.npc.Take(200).Where(npc => npc.active && (bossID.Contains(npc.type) || npc.boss)).LastOrDefault();

            if (bossNPC == null)
                GetBiome();
            else
                GetBoss();

            RPControl.Update();
        }

    }
}