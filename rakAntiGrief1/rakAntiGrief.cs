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
        public bool configDoorChange = false;
        public bool configTileChange = false;
        public bool configSignEdit = false;
        public bool configPlayerProjectile = false;
        public int configRange = 0;
        public bool isEnabled = false;

        public override void Load()
        {
            Name = "rakAntiGrief";
            Description = "Attempts to stop common griefing attempts";
            Author = "rakiru";
            Version = "0.1.18";
            TDSMBuild = 28; //Current Release - Working

            string pluginFolder = Statics.PluginPath + Path.DirectorySeparatorChar + Name;
            //Create folder if it doesn't exist
            CreateDirectory(pluginFolder);

            //setup a new properties file
            properties = new Properties(pluginFolder + Path.DirectorySeparatorChar + Name + ".properties");
            properties.Load();
            //properties.pushData(); //Creates default values if needed. [Out-Dated]

            //read properties data
            configDoorChange = properties.ExtendedReachDoor;
            configTileChange = properties.ExtendedReach;
            configSignEdit = properties.ExtendedReachSign;
            configPlayerProjectile = properties.BlockExplosives;
            configRange = properties.ExtendedReachRange;

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
        }

        public override void Disable()
        {
            properties.Save();
            Program.tConsole.WriteLine(base.Name + " disabled.");
            isEnabled = false;
        }

        public override void onPlayerTileChange(PlayerTileChangeEvent Event)
        {
            if (isEnabled == false || configTileChange == false)
            {
                return;
            }
            else
            {
                Player Player = (Player)Event.Sender;
                if (Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.Position.X), 2) + Math.Pow((Player.Location.Y / 16 - Event.Position.Y), 2)) > configRange) //Relatively long task - possibly change to simple square area rather than a circle
                {
                    Event.Cancelled = true;
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
                Player Player = (Player)Event.Sender;
                if (Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.Sign.x), 2) + Math.Pow((Player.Location.Y / 16 - Event.Sign.y), 2)) > configRange)
                {
                    Event.Cancelled = true;
                    Player.sendMessage("You are too far away to edit that sign.",255,255,0,0);
                    Program.tConsole.WriteLine("[" + base.Name + "] Cancelled Sign Edit of Player: " + Player.Name);
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
                Player Player = (Event.Sender as Player);
                if (Player == null)
                {
                    return;
                }
                if (Math.Sqrt(Math.Pow((Player.Location.X / 16 - Event.X), 2) + Math.Pow((Player.Location.Y / 16 - Event.Y), 2)) > configRange)
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
            if (isEnabled == false || configPlayerProjectile == false)
            {
                return;
            }
            else
            {
                Player Player = (Player)Event.Sender;
                ProjectileType type = Event.Projectile.type;
                //Program.tConsole.WriteLine("[" + base.Name + "] " + Event.Sender.Name + " Launched Projectile of type " + type + "(" + Event.Projectile.Name + ")");
                if (type == ProjectileType.DYNAMITE || type == ProjectileType.GRENADE || type == ProjectileType.BOMB || type == ProjectileType.BOMB_STICKY)
                {
                    Event.Cancelled = true;
                    Player.sendMessage("You are not allowed to use explosives on this server.", 255, 255, 0, 0);
                    Program.tConsole.WriteLine("[" + base.Name + "] Cancelled explosives use of Player: " + Player.Name);
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
