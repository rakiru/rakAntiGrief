using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using rakAntiGrief;
using Terraria_Server.Plugin;
using Terraria_Server.Definitions;
using Terraria_Server;
using Terraria_Server.Events;
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
        public bool configBlockLiquids = false;
        public int configExtendedReachRange = 0;
        public bool isEnabled = false;

        public override void Load()
        {
            Name = "rakAntiGrief";
            Description = "Attempts to stop common griefing attempts";
            Author = "rakiru";
            Version = "0.2.0";
            TDSMBuild = 28; //Current Release - Working

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + Name;
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            //setup a new properties file
            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + Name + ".properties");
            properties.Load();
            //properties.pushData(); //Creates default values if needed. [Out-Dated]

            //read properties data
            configExtendedReachDoor = properties.ExtendedReachDoor;
            configExtendedReach = properties.ExtendedReach;
            configExtendedReachSign = properties.ExtendedReachSign;
            configBlockExplosives = properties.BlockExplosives;
            configBlockLiquids = properties.BlockLiquids;
            configExtendedReachRange = properties.ExtendedReachRange;

            properties.Save();
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
            Program.tConsole.WriteLine(base.Name + " " + base.Version + " disabled.");
            isEnabled = false;
        }

        public override void onPlayerTileChange(PlayerTileChangeEvent Event)
        {
            if (isEnabled == false || configExtendedReach == false)
            {
                return;
            }

            Player Player = (Player)Event.Sender;

            if (Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.Position.X), 2) + Math.Pow((Player.Location.Y / 16 - Event.Position.Y), 2)) > configExtendedReachRange) //Relatively long task - possibly change to simple square area rather than a circle
            {
                Event.Cancelled = true;
                return;
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
                if (Player == null)
                {
                    return;
                }
                if (Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.Sign.x), 2) + Math.Pow((Player.Location.Y / 16 - Event.Sign.y), 2)) > configExtendedReachRange)
                {
                    Event.Cancelled = true;
                    Player.sendMessage("You are too far away to edit that sign.",255,255,0,0);
                    Program.tConsole.WriteLine("[" + base.Name + "] Cancelled Sign Edit of Player: " + Player.Name);
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
                if (Player == null)
                {
                    return;
                }
                if (Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.X), 2) + Math.Pow((Player.Location.Y / 16 - Event.Y), 2)) > configExtendedReachRange)
                {
                    Event.Cancelled = true;
                    String direction;
                    if (Event.Direction == 1)
                    {
                        direction = "open";
                    }
                    else
                    {
                        direction = "close";
                    }
                    Player.sendMessage("You are too far away to " + direction + " that door", 255, 255, 0, 0);
                    Program.tConsole.WriteLine("[" + base.Name + "] Cancelled extended reach door change of Player: " + Player.Name);
                }
            }
        }

        public override void onPlayerProjectileUse(PlayerProjectileEvent Event)
        {
            if (isEnabled == false || configBlockExplosives == false)
            {
                return;
            }
            else
            {
                Player Player = Event.Sender as Player;
                if (Player == null)
                {
                    return;
                }
                ProjectileType type = Event.Projectile.type;
                if (type == ProjectileType.DYNAMITE || type == ProjectileType.GRENADE || type == ProjectileType.BOMB || type == ProjectileType.BOMB_STICKY)
                {
                    Event.Cancelled = true;
                    Player.sendMessage("You are not allowed to use explosives on this server.", 255, 255, 0, 0);
                    Program.tConsole.WriteLine("[" + base.Name + "] Cancelled explosives use of Player: " + Player.Name);
                }
            }
        }

        public override void onPlayerFlowLiquid(PlayerFlowLiquidEvent Event)
        {
            if (isEnabled == false || configBlockExplosives == false)
            {
                return;
            }

            Player Player = Event.Sender as Player;
            if (Player == null)
            {
                return;
            }

            if (configBlockLiquids == true || (Math.Pow((Player.Location.X / 16 - Event.Position.X), 2) + Math.Pow((Player.Location.Y / 16 - Event.Position.Y), 2)) > configExtendedReachRange)
            {
                Event.Cancelled = true;
                    Player.sendMessage("You are not allowed to use liquids on this server.", 255, 255, 0, 0);
                    Program.tConsole.WriteLine("[" + base.Name + "] Cancelled liquid use of Player: " + Player.Name);
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
