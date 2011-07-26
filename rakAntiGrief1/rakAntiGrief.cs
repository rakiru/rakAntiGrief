using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using rakAntiGrief;
using Terraria_Server.Plugin;
using Terraria_Server.Definitions;
using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Events;
using Terraria_Server.Logging;
using System.IO;

namespace rakAntiGrief
{
    public class rakAntiGrief : Plugin
    {

        // tConsole is used for when logging Output to the console & a log file.

        public Properties properties;
        public string pluginFolder;
        public bool configDoorChange = false;
        public bool configTileChange = false;
        public bool configSignEdit = false;
        public bool configPlayerProjectile = false;
        public int configRange = 0;
		public string configAdminTileTypeString = "";
		public int[] configAdminTileType;
		public bool configAdminTile = false;
		public bool configLavaFlow = false;
		public bool configWaterFlow = false;
        public bool isEnabled = false;

		static HashSet<PlayerState> states;
		static System.Threading.Timer timer;

        public override void Load()
        {
            Name = "rakAntiGrief";
            Description = "Attempts to stop common griefing attempts";
            Author = "rakiru";
            Version = "0.1.17";
            TDSMBuild = 28; //Current Release - Working

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + Name;
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            //setup a new properties file
            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + Name + ".properties");
            properties.Load();

            //read properties data
            configDoorChange = properties.DoorChange;
            configTileChange = properties.TileChange;
            configSignEdit = properties.SignEdit;
            configPlayerProjectile = properties.PlayerProjectile;
            configRange = properties.Range;
			configAdminTileTypeString = properties.AdminTileType;
			configAdminTile = properties.AdminTile;
			configLavaFlow = properties.LavaFlow;
			configWaterFlow = properties.WaterFlow;
			if (configAdminTile)
			{
				String[] temp = configAdminTileTypeString.Split(',');
				if (temp.Length == 0)
				{
					ProgramLog.Error.Log("[" + base.Name + "]: No admin tiles specified.  Disabling admin tile protection.");
					configAdminTile = false;
				}
				configAdminTileType = new int[temp.Length];
				try
				{
					for (int i = 0; i < temp.Length; i++)
					{
						configAdminTileType[i] = Int32.Parse(temp[i]);
					}
				}
				catch
				{
					ProgramLog.Error.Log("[" + base.Name + "]: Error parsing admin tiles.  Disabling admin tile protection.");
					configAdminTile = false;
				}
			}
			states = new HashSet<PlayerState>();
			timer = new System.Threading.Timer(ExpireProjectiles);
			timer.Change(1000, 1000);
			properties.Save();

			AddCommand("killproj").Calls(KillProjCommand);
        }

        public override void Enable()
        {
            Program.tConsole.WriteLine(base.Name + " " + base.Version + " enabled.");
            isEnabled = true;
			//Register Hooks
			this.registerHook(Hooks.PLAYER_TILECHANGE);
			this.registerHook(Hooks.PLAYER_EDITSIGN);
			this.registerHook(Hooks.PLAYER_PROJECTILE);
			this.registerHook(Hooks.DOOR_STATECHANGE);
			this.registerHook(Hooks.PLAYER_FLOWLIQUID);
        }

        public override void Disable()
        {
            Program.tConsole.WriteLine(base.Name + " disabled.");
            isEnabled = false;
        }

		public override void onPlayerFlowLiquid(PlayerFlowLiquidEvent ev)
		{
			if (isEnabled == false || (!configLavaFlow && !configWaterFlow))
			{
				return;
			}

			Player player = ev.Sender as Player;
			if (player == null) return;

			if (player.Op) return;

			int x = (int)ev.Position.X;
			int y = (int)ev.Position.Y;

			if (x < 0 || y < 0 || x > Main.maxTilesX || y > Main.maxTilesY || (Math.Sqrt(Math.Pow((player.Location.X / 16 - x), 2) + Math.Pow((player.Location.Y / 16 - y), 2)) > configRange))
			{
				if ((ev.Lava && configLavaFlow) || (!ev.Lava && configWaterFlow))
				{
					ProgramLog.Debug.Log("[" + base.Name + "]: Cancelled out of reach {1} flow by {0}", player.Name ?? player.whoAmi.ToString(), ev.Lava ? "lava" : "water");
					ev.Cancelled = true;
					return;
				}
			}

			// blue bricks are admin bricks
			var tile = Main.tile.At(x, y);
			bool adminTile = false;
			if (configAdminTile)
			{
				for (int i = 0; i < configAdminTileType.Length; i++)
				{
					if (tile.Type == configAdminTileType[i])
					{
						adminTile = true;
						break;
					}
				}

				if (adminTile)
				{
					if ((ev.Lava && configLavaFlow) || (!ev.Lava && configWaterFlow))
					{
						ProgramLog.Debug.Log("[" + base.Name + "]: Cancelled {1} flow by {0} in admin area", player.Name ?? player.whoAmi.ToString(), ev.Lava ? "lava" : "water");
						ev.Cancelled = true;
						return;
					}
				}
			}
		}

		public override void onPlayerTileChange(PlayerTileChangeEvent Event)
		{
			if (isEnabled == false || configTileChange == false)
			{
				return;
			}
			else
			{
				Player player = Event.Sender as Player;
				if (player == null) return;

				if (player.Op) return;

				int x = (int)Event.Position.X;
				int y = (int)Event.Position.Y;

				if (x < 0 || y < 0 || x > Main.maxTilesX || y > Main.maxTilesY || (Math.Sqrt(Math.Pow((player.Location.X / 16 - Event.Position.X), 2) + Math.Pow((player.Location.Y / 16 - Event.Position.Y), 2)) > configRange))
				{
					Event.Cancelled = true;
					return;
				}

				// blue bricks are admin bricks
				if (configAdminTile)
				{
					if (Event.Tile.Type == 41 || Event.Tile.Wall == 7)
					{
						ProgramLog.Admin.Log("[" + base.Name + "]: Prevented {0} from editing admin bricks.", player.Name);
						Event.Cancelled = true;
						return;
					}

					var tile = Main.tile.At(x, y);
					if (tile.Type == 41 || tile.Type == 7)
					{
						ProgramLog.Admin.Log("[" + base.Name + "]: Prevented {0} from editing admin bricks.", player.Name);
						Event.Cancelled = true;
						return;
					}
				}
			}
		}

		public override void onPlayerEditSign(PlayerEditSignEvent Event)
		{
			if (isEnabled == false || configSignEdit == false)
			{
				return;
			}
			else
			{
				Player Player = Event.Sender as Player;

				if (Player != null && Player.AuthenticatedAs == null)
				{
					Event.Cancelled = true;
				}
				if (Player != null && Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.Sign.x), 2) + Math.Pow((Player.Location.Y / 16 - Event.Sign.y), 2)) > configRange)
				{
					Event.Cancelled = true;
					Player.sendMessage("You are too far away to edit that sign.", 255, 255, 0, 0);
					Program.tConsole.WriteLine("[" + base.Name + "]: Cancelled Sign Edit of Player: " + Player.Name);
				}
			}
		}

		public override void onDoorStateChange(DoorStateChangeEvent Event)
		{
			if (isEnabled == false || configDoorChange == false)
			{
				return;
			}
			else
			{
				Player Player = Event.Sender as Player;
				if (Player != null && Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.X), 2) + Math.Pow((Player.Location.Y / 16 - Event.Y), 2)) > configRange)
				{
					Event.Cancelled = true;
					Player.sendMessage("You are too far away to " + Event.Direction + " that door", 255, 255, 0, 0);
					ProgramLog.Admin.Log("[" + base.Name + "]: Cancelled Door Change of Player: " + Player.Name);
				}
			}
		}

		class PlayerState
		{
			public int projectiles;
		}

		public void ExpireProjectiles(object dummy)
		{
			foreach (var state in states)
			{
				state.projectiles = 0;
			}
		}

		public override void onPlayerProjectileUse(PlayerProjectileEvent Event)
		{
			if (isEnabled == false || configPlayerProjectile == false)
			{
				return;
			}
			else
			{
				Player player = Event.Sender as Player;
				if (player == null || player.Op) return;

				PlayerState state = null;
				if (!player.PluginData.Contains(this))
				{
					state = new PlayerState();
					player.PluginData[this] = state;
					states.Add(state);
				}
				else
					state = (PlayerState)player.PluginData[this];

				ProjectileType type = Event.Projectile.type;

				state.projectiles += 1;

				if (player.AuthenticatedAs == null)
				{
					Event.Cancelled = true;
					player.sendMessage("You are not allowed to use projectiles as a guest.", 255, 255, 0, 0);
					if (state.projectiles >= 9)
					{
						state.projectiles -= 9;
						ProgramLog.Admin.Log("[" + base.Name + "]: Stopped projectile {0} spam from guest {1}.", type, player.Name ?? "<null>");
					}
					return;
				}

				if (state.projectiles >= 9)
				{
					Event.Cancelled = true;
					state.projectiles -= 9;
					ProgramLog.Admin.Log("[" + base.Name + "]: Stopped projectile {0} spam from {1}.", type, player.Name ?? "<null>");
					return;
				}


				//Program.tConsole.WriteLine("[" + base.Name + "] " + Event.Sender.Name + " Launched Projectile of type " + type + "(" + Event.Projectile.Name + ")");
				if (player != null && type == ProjectileType.DYNAMITE || type == ProjectileType.GRENADE || type == ProjectileType.BOMB || type == ProjectileType.BOMB_STICKY)
				{
					Event.Cancelled = true;
					player.sendMessage("You are not allowed to use explosives on this server.", 255, 255, 0, 0);
					ProgramLog.Admin.Log("[" + base.Name + "]: Cancelled Projectile Use of Player: " + ((Player)Event.Sender).Name);
				}
			}
		}

		private void KillProjCommand(Server server, ISender sender, ArgumentList args)
		{
			ProgramLog.Admin.Log("[" + base.Name + "]: Erasing all projectiles.");
			var msg = NetMessage.PrepareThreadInstance();
			msg.PlayerChat(255, "<Server> Erasing all projectiles.", 255, 180, 100);
			foreach (var projectile in Main.projectile)
			{
				projectile.Active = false;
				projectile.type = ProjectileType.UNKNOWN;

				msg.Projectile(projectile);
			}
			msg.Broadcast();
		}

		private static void CreateDirectory(string dirPath)
		{
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}
		}
	}
}