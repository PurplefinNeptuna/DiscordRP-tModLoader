using Terraria;
using Terraria.ModLoader;

namespace DiscordRP
{
    class Testplayer : ModPlayer
    {
        public int updateTick = 0;
        public int maxUpdateTick = 180;
        
        public override void OnEnterWorld(Player someone)
        {
            string wName = Main.worldName;
            bool expert = Main.expertMode;
            string wDiff = (expert) ? "(Expert)" : "(Normal)";
            RPControl.presence.details = string.Format("Playing {0} {1}", wName, wDiff);
            updateTick = 180;
            RPControl.Update();
        }
            
        public override void PostUpdate()
        {
            RPUtility.life = player.statLife;
            RPUtility.mana = player.statMana;
            RPUtility.def = player.statDefense;

            RPUtility.zone1 = player.zone1;
            RPUtility.zone2 = player.zone2;
            RPUtility.zone3 = player.zone3;

            if (updateTick < maxUpdateTick)
            {
                updateTick++;
            }
            else
            {
                updateTick = 0;
                RPUtility.Update();
            }
        }

    }
}
