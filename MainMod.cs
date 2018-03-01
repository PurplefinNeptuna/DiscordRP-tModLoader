using Terraria;
using Terraria.ModLoader;
using System;

namespace DiscordRP
{
	public class MainMod : Mod
	{
		public static uint? prevCount;
		public static bool pauseUpdate = false;
		public static void UpdaterLoad()
		{
			Main.OnTick += RPUpdate;
		}

		public static void UpdaterUnload()
		{
			Main.OnTick -= RPUpdate;
		}

		public MainMod()
		{
			Properties = new ModProperties()
			{
				Autoload = true,
				AutoloadGores = true,
				AutoloadSounds = true
			};
		}


		public override void Load()
		{
			RPControl.Enable();
			RPControl.presence.details = string.Format("In Main Menu");
			RPControl.presence.largeImageKey = string.Format("payload_test");
			RPControl.presence.largeImageText = string.Format("Terraria");

			DateTime date = DateTime.Now;
			DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			long timenow = Convert.ToInt64((date.ToUniversalTime() - epoch).TotalSeconds);

			RPControl.presence.startTimestamp = timenow;

			RPControl.Update();
		}


		public override void PreSaveAndQuit()
		{
			RPControl.presence.details = string.Format("In Main Menu");
			RPControl.presence.state = null;
			RPControl.presence.largeImageKey = string.Format("payload_test");
			RPControl.presence.largeImageText = string.Format("Terraria");
			RPControl.presence.smallImageKey = null;
			RPControl.presence.smallImageText = null;
			RPControl.Update();
			UpdaterUnload();
		}

		public override void Unload()
		{
			RPControl.Disable();
		}

		public static void RPUpdate()
		{
			if (!Main.dedServ && !Main.gameMenu)
			{
				Player RPlayer = Main.player[Main.myPlayer];
				if ((prevCount == null || prevCount + 180 <= Main.GameUpdateCount) || (Main.gamePaused && !pauseUpdate))
				{
					if (Main.gamePaused)
					{
						pauseUpdate = true;
					}
					prevCount = Main.GameUpdateCount;
					//Main.NewText(prevCount);
					RPUtility.player = RPlayer;
					RPUtility.Update();
				}
				else if (!Main.gamePaused)
				{
					pauseUpdate = false;
				}
				else return;
			}
			else return;
		}
	}
}