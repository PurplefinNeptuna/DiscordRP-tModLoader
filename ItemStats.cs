using System;
using Terraria;
using Terraria.ModLoader;

namespace DiscordRP
{
    class ItemStats : GlobalItem
    {
        public override void HoldItem(Item item, Player player)
        {
            if(item.melee || item.ranged || item.magic || item.thrown || item.summon)
            {
                RPUtility.atk = item.damage;
            }

            if (item.melee)
            {
                RPUtility.type = RPUtility.ATKType.melee;
                RPUtility.atk = (int)Math.Ceiling(RPUtility.atk * player.meleeDamage);
            }
            else if (item.ranged)
            {
                RPUtility.type = RPUtility.ATKType.ranged;
                RPUtility.atk = (int)Math.Ceiling(RPUtility.atk * player.rangedDamage);
            }
            else if (item.magic)
            {
                RPUtility.type = RPUtility.ATKType.magic;
                RPUtility.atk = (int)Math.Ceiling(RPUtility.atk * player.magicDamage);
            }
            else if (item.thrown)
            {
                RPUtility.type = RPUtility.ATKType.thrown;
                RPUtility.atk = (int)Math.Ceiling(RPUtility.atk * player.thrownDamage);
            }
            else if (item.summon)
            {
                RPUtility.type = RPUtility.ATKType.summon;
                RPUtility.atk = (int)Math.Ceiling(RPUtility.atk * player.minionDamage);
            }
        }
    }
}
