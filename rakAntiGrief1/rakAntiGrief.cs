using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using rakAntiGrief;
using Terraria_Server.Plugins;
using Terraria_Server.Definitions;
using Terraria_Server;
using Terraria_Server.Commands;
using Terraria_Server.Logging;
using System.IO;

namespace rakAntiGrief
{
    public class rakAntiGrief : BasePlugin
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
        public bool configBlockChatSpam = false;
        public bool configBlockMovementNoclip = false;
        public bool configBlockMovementTeleport = false;
        public int configBlockMovementTeleportDistance = 0;
        public bool configSpawnProtection = false;
        public int configSpawnProtectionType = 0;
        public int configSpawnProtectionRange = 0;

        static HashSet<PlayerState> states;
        static System.Threading.Timer timer;
        static System.Threading.Timer timer2;

        public rakAntiGrief (){
            Name = "rakAntiGrief";
            Description = "Attempts to stop common griefing attempts";
            Author = "rakiru";
            Version = "0.4.44";
            TDSMBuild = 31; //Current Release - Working
        }
        
        protected override void Initialized(object state)
        {
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
            configBlockChatSpam = properties.BlockChatSpam;
            configBlockMovementNoclip = properties.BlockMovementNoclip;
            configBlockMovementTeleport = properties.BlockMovementTeleport;
            configBlockMovementTeleportDistance = properties.BlockMovementTeleportDistance;
            configSpawnProtection = properties.SpawnProtection;
            configSpawnProtectionType = properties.SpawnProtectionType;
            configSpawnProtectionRange = properties.SpawnProtectionRange;
            properties.Save();

            states = new HashSet<PlayerState>();
            timer = new System.Threading.Timer(RepeatingTask);
            timer.Change(1000, 1000);
            timer2 = new System.Threading.Timer(RepeatingTask2);
            timer2.Change(10000, 10000);
        }

        protected override void Enabled()
        {
            ProgramLog.Plugin.Log(base.Name + " " + base.Version + " enabled.");
        }

        protected override void Disabled()
        {
            ProgramLog.Plugin.Log(base.Name + " " + base.Version + " disabled.");
        }

        [Hook(HookOrder.LATE)]
        void OnLiquidFlow(ref HookContext ctx, ref HookArgs.LiquidFlowReceived args)
        {
            if (!configBlockLavaFlow && !configBlockWaterFlow)
            {
                return;
            }

            Player player = ctx.Sender as Player;
            if (player == null) return;

            if (player.Op) return;

            int x = (int)args.X;
            int y = (int)args.Y;

            if (args.Lava && configBlockLavaFlow)
            {
                ctx.SetResult(HookResult.IGNORE);
                player.sendMessage("You are not allowed to use lava on this server.", 255, 255, 0, 0);
                return;
            }
            else if (!args.Lava && configBlockWaterFlow)
            {
                ctx.SetResult(HookResult.IGNORE);
                player.sendMessage("You are not allowed to use water on this server.", 255, 255, 0, 0);
                return;
            }
            else if (inSpawn(x, y))
            {
                ctx.SetResult(HookResult.IGNORE);
                player.sendMessage("You are not allowed to edit spawn on this server.", 255, 255, 0, 0);
                return;
            }
            else if (x < 0 || y < 0 || x > Main.maxTilesX || y > Main.maxTilesY || distance(player.Location.X, player.Location.Y, x, y) > configExtendedReachRange)
            {
                //ProgramLog.Debug.Log("[" + base.Name + "]: Cancelled out of reach {1} flow by {0}.", player.Name ?? player.whoAmi.ToString(), Event.Lava ? "lava" : "water");
                ctx.SetResult(HookResult.IGNORE);
                return;
            }
        }

        [Hook(HookOrder.EARLY)]
        void OnAlter(ref HookContext ctx, ref HookArgs.PlayerWorldAlteration args)
        {
            if (configExtendedReach == false)
            {
                return;
            }
            else
            {
                Player player = ctx.Sender as Player;

                if (player == null || player.Op == true)
                {
                    return;
                }

                int x = (int)args.X;
                int y = (int)args.Y;

                if (inSpawn(x, y))
                {
                    ctx.SetResult(HookResult.IGNORE);
                    player.sendMessage("You are not allowed to edit spawn on this server.", 255, 255, 0, 0);
                    return;
                }
                else if (x < 0 || y < 0 || x > Main.maxTilesX || y > Main.maxTilesY || distance(player.Location.X, player.Location.Y, x, y) > configExtendedReachRange)
                {
                    ctx.SetResult(HookResult.IGNORE);
                    return;
                }
            }
        }

        [Hook(HookOrder.EARLY)]
        void OnSignTextSet(ref HookContext ctx, ref HookArgs.SignTextSet args)
        {
            if (configExtendedReachSign == false)
            {
                return;
            }
            else
            {
                Player player = ctx.Sender as Player;

                if (player == null || player.Op == true)
                {
                    return;
                }

                int x = (int)args.X;
                int y = (int)args.Y;

                if (inSpawn(x, y))
                {
                    ctx.SetResult(HookResult.IGNORE);
                    player.sendMessage("You are not allowed to edit spawn on this server.", 255, 255, 0, 0);
                    return;
                }
                else if (distance(player.Location.X, player.Location.Y, x, y) > configExtendedReachRange)
                {
                    ctx.SetResult(HookResult.IGNORE);
                    player.sendMessage("You are too far away to edit that sign.", 255, 255, 0, 0);
                    ProgramLog.Admin.Log("[" + base.Name + "]: Cancelled Sign Edit of Player: " + player.Name);
                }
            }
        }

        [Hook(HookOrder.EARLY)]
        void OnDoorStateChanged(ref HookContext ctx, ref HookArgs.DoorStateChanged args)
        {
            if (configExtendedReachDoor == false)
            {
                return;
            }
            else
            {
                Player player = ctx.Sender as Player;

                if (player == null || player.Op == true)
                {
                    return;
                }

                if (distance(player.Location.X, player.Location.Y, args.X, args.Y) > configExtendedReachRange)
                {
                    ctx.SetResult(HookResult.IGNORE);
                    player.sendMessage("You are too far away to " + args.Direction + " that door", 255, 255, 0, 0);
                    ProgramLog.Admin.Log("[" + base.Name + "]: Cancelled Door Change of Player: " + player.Name);
                }
            }
        }

        [Hook(HookOrder.EARLY)]
        void OnProjectile(ref HookContext ctx, ref HookArgs.ProjectileReceived args)
        {
            if (configBlockExplosives == false || args.Type==ProjectileType.ORB_OF_LIGHT || args.Type==ProjectileType.FLAMELASH )
            {
                return;
            }
            else
            {
                Player player = ctx.Sender as Player;
                if (player == null) return;

                PlayerState state = null;
                if (!player.PluginData.Contains(this))
                {
                    state = new PlayerState();
                    player.PluginData[this] = state;
                    states.Add(state);
                }
                else
                {
                    state = (PlayerState)player.PluginData[this];
                }

                ProjectileType type = args.Type;

                if (!(type == ProjectileType.ORB_OF_LIGHT || type == ProjectileType.FLAMELASH || type == ProjectileType.MISSILE_MAGIC))
                {
                    state.projectiles += 1;
                }

                if (state.projectiles >= 9)
                {
                    ctx.SetResult(HookResult.IGNORE);
                    state.projectiles -= 9;
					ProgramLog.Admin.Log("[" + base.Name + "]: Stopped projectile {0} spam from {1}.", type, player.Name ?? "<null>");
                    return;
                }

                if (player != null && (type == ProjectileType.DYNAMITE || type == ProjectileType.GRENADE || type == ProjectileType.BOMB || type == ProjectileType.BOMB_STICKY || type == ProjectileType.ARROW_HELLFIRE))
                {
                    ctx.SetResult(HookResult.IGNORE);
                    player.sendMessage("You are not allowed to use explosives on this server.", 255, 255, 0, 0);
                    ProgramLog.Admin.Log("[" + base.Name + "]: Cancelled Projectile Use of Player: " + ((Player)ctx.Sender).Name);
                }
            }
        }

        [Hook(HookOrder.TERMINAL)]
        void onPlayerChat(ref HookContext ctx, ref HookArgs.PlayerChat args)
        {
            if (configBlockChatSpam == false)
            {
                return;
            }
            else
            {
                Player player = ctx.Sender as Player;
                if (player == null || player.Op) return;

                PlayerState state = null;
                if (!player.PluginData.Contains(this))
                {
                    state = new PlayerState();
                    player.PluginData[this] = state;
                    states.Add(state);
                }
                else
                {
                    state = (PlayerState)player.PluginData[this];
                }

                state.messages += 1;

                if (state.messages >= 9)
                {
                    ctx.SetResult(HookResult.IGNORE);
                    state.messages -= 9;
                    ProgramLog.Admin.Log("[" + base.Name + "]: Stopped chat spam from {1}.", player.Name ?? "<null>");
                    player.Kick("Do not spam chat.");
                    return;
                }
            }
        }

        [Hook(HookOrder.TERMINAL)]
        void onPlayerTeleport(ref HookContext ctx, ref HookArgs.PlayerTeleport args)
        {
            if (configBlockMovementTeleport == false)
            {
                return;
            }
            else
            {
                Player player = ctx.Sender as Player;
                if (player == null || player.Op) return;

                PlayerState state = null;
                if (!player.PluginData.Contains(this))
                {
                    state = new PlayerState();
                    player.PluginData[this] = state;
                    states.Add(state);
                }
                else
                {
                    state = (PlayerState)player.PluginData[this];
                }
                state.teleported = DateTime.UtcNow;
            }
        }

        [Hook(HookOrder.TERMINAL)]
        void onPlayerChat(ref HookContext ctx, ref HookArgs.PlayerTriggeredEvent args)
        {
            Player player = ctx.Sender as Player;
            if (player == null || player.Op) return;

            PlayerState state = null;
            if (!player.PluginData.Contains(this))
            {
                state = new PlayerState();
                player.PluginData[this] = state;
                states.Add(state);
            }
            else
            {
                state = (PlayerState)player.PluginData[this];
            }
            state.movePackets += 1;

            if (state.movePackets >= 29)
            {
                if (++state.movePacketsViolations > 9)
                {
                    ProgramLog.Admin.Log("[" + base.Name + "]: Stopped position update spam from {0}.", player.Name ?? "<null>");
                    player.Kick("Do not spam packets.");
                    return;
                }
            }

            var tile = Main.tile.At((int)args.X / 16, (int)args.Y / 16);
            if (tile.Active && Main.tileSolid[tile.Type] && tile.Type != 19 && ++state.noclips > 3)
            {
                ProgramLog.Admin.Log("[" + base.Name + "]: Noclip detected for {1}.", player.Name ?? "<null>");
                player.Kick("Noclip detected.");
                return;
            }
            return;
        }

        class PlayerState
        {
            public int projectiles;
            public int messages;
            public DateTime teleported;
            public int movePackets;
            public int movementViolations;
            public int movePacketsViolations;
            public int noclips;
            public float x;
            public float y;

            public PlayerState()
            {
                this.x = Main.spawnTileX * 16;
                this.y = Main.spawnTileY * 16;
                this.teleported = DateTime.UtcNow;
            }
        }

        public void RepeatingTask(object dummy)
        {
            DateTime startTime = DateTime.UtcNow;
            foreach (var state in states)
            {
                state.projectiles = 0;
                state.messages = 0;
                state.movePackets = 0;
                state.noclips = 0;
            }

            foreach (Player player in Main.players)
            {
                if (!player.Active) continue;
                int x = (int)player.Location.X;
                int y = (int)player.Location.Y;
                PlayerState state = null;
                if (!player.PluginData.Contains(this))
                {
                    state = new PlayerState();
                    player.PluginData[this] = state;
                    states.Add(state);
                }
                else
                {
                    state = (PlayerState)player.PluginData[this];
                }
                double dist = Math.Sqrt(Math.Pow((x - state.x), 2) + Math.Pow((y - state.y), 2));
                if (dist > configBlockMovementTeleportDistance)
                {
                    double time = DateTime.UtcNow.Subtract(state.teleported).TotalMilliseconds;
                    float distSpawn = distance(x, y, player.SpawnX / 16, player.SpawnY / 16);
                    if (time > 1000 && distSpawn > 16)
                    {
                        ProgramLog.Admin.Log("[" + base.Name + "]: Movement hacking (or high lag) detected for {0}.", player.Name ?? "<null>");
                        //ProgramLog.Debug.Log("[" + base.Name + "]: distance: {0}   confixDistance: {1}   timeDif: {2}   distanceFromSpawn: {3}    {4}, {5}, {6}, {7}", dist, configBlockMovementTeleportDistance, time, distSpawn, x, y, player.SpawnX, player.SpawnY);
                        if (++state.movementViolations > 3)
                        {
                            player.Kick("Movement hacking (or high lag) detected.");
                            continue;
                        }
                    }
                }
                //ProgramLog.Debug.Log("[" + base.Name + "]: Checked {0} at {1},{2} from {3},{4}!", player.Name, x, y, state.x, state.y);
                state.x = x;
                state.y = y;
            }
            //ProgramLog.Debug.Log("[" + base.Name + "]: Time taken on RepeatingTask {0}ms", DateTime.UtcNow.Subtract(startTime).Milliseconds);
        }

        public void RepeatingTask2(object dummy)
        {
            DateTime startTime = DateTime.UtcNow;
            foreach (var state in states)
            {
                state.movementViolations = 0;
                state.movePacketsViolations = 0;
            }
            //ProgramLog.Debug.Log("[" + base.Name + "]: Time taken on RepeatingTask {0}ms", DateTime.UtcNow.Subtract(startTime).Milliseconds);
        }

        private bool inSpawn(int playerX, int playerY)
        {
            if (!configSpawnProtection)
            {
                return false;
            }

            int spawnX = Server.World.SpawnX;
            int spawnY = Server.World.SpawnY;

            switch (configSpawnProtectionType)
            {
                case 0: //Vertical strip spawn protection
                    if (playerX > (spawnX - configSpawnProtectionRange) && playerX < (spawnX + configSpawnProtectionRange))
                    {
                        return true;
                    }
                    break;
                case 1: //Square spawn protection
                    if (playerX > (spawnX - configSpawnProtectionRange) && playerX < (spawnX + configSpawnProtectionRange) && playerY > (spawnY - configSpawnProtectionRange) && playerY < (spawnY + configSpawnProtectionRange))
                    {
                        return true;
                    }
                    break;
                case 2: //Circle spawn protection
                    if (distance(playerX, spawnX, playerY, spawnY) < configSpawnProtectionRange)
                    {
                        return true;
                    }
                    break;
            }
            return false;
        }

        public float distance(float playerX, float playerY, float tileX, float tileY)
        {
            return (float)Math.Sqrt(Math.Pow((playerX / 16 - tileX), 2) + Math.Pow((playerY / 16 - tileY), 2));
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
