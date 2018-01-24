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
            }
            else if (item.ranged)
            {
                RPUtility.type = RPUtility.ATKType.ranged;
            }
            else if (item.magic)
            {
                RPUtility.type = RPUtility.ATKType.magic;
            }
            else if (item.thrown)
            {
                RPUtility.type = RPUtility.ATKType.thrown;
            }
            else if (item.summon)
            {
                RPUtility.type = RPUtility.ATKType.summon;
            }
        }
    }
}
