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
            updateTick = maxUpdateTick;
        }
            
        public override void PostUpdate()
        {
            if (updateTick < maxUpdateTick)
            {
                updateTick++;
            }
            else
            {
                updateTick = 0;
                RPUtility.player = player;
                RPUtility.Update();
            }
        }

    }
}