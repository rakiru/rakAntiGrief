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
        public bool configExtendedReachDoor = false;
        public bool configExtendedReach = false;
        public bool configExtendedReachSign = false;
        public bool configBlockExplosives = false;
        public int configExtendedReachRange = 0;
        public bool configBlockLavaFlow = false;
        public bool configBlockWaterFlow = false;
        public bool isEnabled = false;

        static HashSet<PlayerState> states;
        static System.Threading.Timer timer;

        public override void Load()
        {
            Name = "rakAntiGrief";
            Description = "Attempts to stop common griefing attempts";
            Author = "rakiru";
            Version = "0.2.5";
            TDSMBuild = 29; //Current Release - Working

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + Name;
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            //setup a new properties file
            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + Name + ".properties");
            properties.Load();

            //read properties data
            configExtendedReachDoor = properties.ExtendedReachDoor;
            configExtendedReach = properties.ExtendedReach;
            configExtendedReachSign = properties.ExtendedReachSign;
            configBlockExplosives = properties.BlockExplosives;
            configExtendedReachRange = properties.ExtendedReachRange;
            configBlockLavaFlow = properties.BlockLavaFlow;
            configBlockWaterFlow = properties.BlockWaterFlow;
            properties.Save();

            states = new HashSet<PlayerState>();
            timer = new System.Threading.Timer(ExpireProjectiles);
            timer.Change(1000, 1000);
        }

        public override void Enable()
        {
            ProgramLog.Admin.Log(base.Name + " " + base.Version + " enabled.");
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
            ProgramLog.Admin.Log(base.Name + " " + base.Version + " disabled.");
            isEnabled = false;
        }

        public override void onPlayerFlowLiquid(PlayerFlowLiquidEvent Event)
        {
            if (isEnabled == false || (!configBlockLavaFlow && !configBlockWaterFlow))
            {
                return;
            }

            Player player = Event.Sender as Player;
            if (player == null) return;

            if (player.Op) return;

            int x = (int)Event.Position.X;
            int y = (int)Event.Position.Y;

            if (Event.Lava && configBlockLavaFlow)
            {
                Event.Cancelled = true;
                player.sendMessage("You are not allowed to use lava on this server.", 255, 255, 0, 0);
                return;
            }
            else if (!Event.Lava && configBlockWaterFlow)
            {
                Event.Cancelled = true;
                player.sendMessage("You are not allowed to use water on this server.", 255, 255, 0, 0);
                return;
            }
            else if (x < 0 || y < 0 || x > Main.maxTilesX || y > Main.maxTilesY || (Math.Sqrt(Math.Pow((player.Location.X / 16 - x), 2) + Math.Pow((player.Location.Y / 16 - y), 2)) > configExtendedReachRange))
            {
                ProgramLog.Debug.Log("[" + base.Name + "]: Cancelled out of reach {1} flow by {0}", player.Name ?? player.whoAmi.ToString(), Event.Lava ? "lava" : "water");
                Event.Cancelled = true;
                return;
            }
        }

        public override void onPlayerTileChange(PlayerTileChangeEvent Event)
        {
            if (isEnabled == false || configExtendedReach == false)
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

                if (x < 0 || y < 0 || x > Main.maxTilesX || y > Main.maxTilesY || (Math.Sqrt(Math.Pow((player.Location.X / 16 - Event.Position.X), 2) + Math.Pow((player.Location.Y / 16 - Event.Position.Y), 2)) > configExtendedReachRange))
                {
                    Event.Cancelled = true;
                    return;
                }
            }
        }

        public override void onPlayerEditSign(PlayerEditSignEvent Event)
        {
            if (isEnabled == false || configExtendedReachSign == false)
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
                if (Player != null && Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.Sign.x), 2) + Math.Pow((Player.Location.Y / 16 - Event.Sign.y), 2)) > configExtendedReachRange)
                {
                    Event.Cancelled = true;
                    Player.sendMessage("You are too far away to edit that sign.", 255, 255, 0, 0);
                    ProgramLog.Admin.Log("[" + base.Name + "]: Cancelled Sign Edit of Player: " + Player.Name);
                }
            }
        }

        public override void onDoorStateChange(DoorStateChangeEvent Event)
        {
            if (isEnabled == false || configExtendedReachDoor == false)
            {
                return;
            }
            else
            {
                Player Player = Event.Sender as Player;
                if (Player != null && Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.X), 2) + Math.Pow((Player.Location.Y / 16 - Event.Y), 2)) > configExtendedReachRange)
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
            if (isEnabled == false || configBlockExplosives == false || Event.Projectile.type==ProjectileType.ORB_OF_LIGHT || Event.Projectile.type==ProjectileType.FLAMELASH )
            {
                return;
            }
            else
            {
                Player player = Event.Sender as Player;
                if (player == null) return;

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

                if (state.projectiles >= 9)
                {
                    Event.Cancelled = true;
                    state.projectiles -= 9;
					ProgramLog.Admin.Log("[" + base.Name + "]: Stopped projectile {0} spam from {1}.", type, player.Name ?? "<null>");
                    return;
                }


                //Program.tConsole.WriteLine("[" + base.Name + "] " + Event.Sender.Name + " Launched Projectile of type " + type + "(" + Event.Projectile.Name + ")");
                if (player != null && (type == ProjectileType.DYNAMITE || type == ProjectileType.GRENADE || type == ProjectileType.BOMB || type == ProjectileType.BOMB_STICKY))
                {
                    Event.Cancelled = true;
                    player.sendMessage("You are not allowed to use explosives on this server.", 255, 255, 0, 0);
                    ProgramLog.Admin.Log("[" + base.Name + "]: Cancelled Projectile Use of Player: " + ((Player)Event.Sender).Name);
                }
            }
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
