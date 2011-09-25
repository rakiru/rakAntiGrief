using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace rakAntiGrief
{
    public class Properties : PropertiesFile
    {
        private const bool DEFAULT_EXTENDED_REACH_DOOR = true; //Toggling doors from far away
        private const bool DEFAULT_EXTENDED_REACH = true; //Breaking/placing blocks from far away
        private const bool DEFAULT_EXTENDED_REACH_SIGN = true; //Toggling doors from far away
        private const bool DEFAULT_BLOCK_EXPLOSIVES = true; //Block explosives from being used
        private const int DEFAULT_EXTENDED_REACH_RANGE = 6; //Max distance to edit tile/door/sign
        private const bool DEFAULT_SPAM_EXPLOSIVES_KICK = true; //Kick user if they spam explosives (using a modded client)
        private const bool DEFAULT_BLOCK_LAVA_FLOW = true;
        private const bool DEFAULT_BLOCK_WATER_FLOW = true;
        private const bool DEFAULT_BLOCK_CHAT_SPAM = true;
        private const bool DEFAULT_BLOCK_MOVEMENT_NOCLIP = true;
        private const bool DEFAULT_BLOCK_MOVEMENT_TELEPORT = true;
        private const int DEFAULT_BLOCK_MOVEMENT_TELEPORT_DISTANCE = 160;
        private const bool DEFAULT_SPAWN_PROTECTION = true;
        private const int DEFAULT_SPAWN_PROTECTION_TYPE = 0;
        private const int DEFAULT_SPAWN_PROTECTION_RANGE = 25;

        private const String EXTENDED_REACH_DOOR = "ExtendedReachDoor";
        private const String EXTENDED_REACH = "ExtendedReach";
        private const String EXTENDED_REACH_SIGN = "ExtendedReachSign";
        private const String BLOCK_EXPLOSIVES = "BlockExplosives";
        private const String EXTENDED_REACH_RANGE = "ExtendedReachRange";
        private const String SPAM_EXPLOSIVES_KICK = "SpamExplosivesKick";
        private const String BLOCK_LAVA_FLOW = "BlockLavaFlow";
        private const String BLOCK_WATER_FLOW = "BlockWaterFlow";
        private const String BLOCK_MOVEMENT_NOCLIP = "BlockMovementNoclip";
        private const String BLOCK_MOVEMENT_TELEPORT = "BlockMovementTeleport";
        private const String BLOCK_MOVEMENT_TELEPORT_DISTANCE = "BlockMovementTeleportDistance";
        private const String BLOCK_CHAT_SPAM = "BlockChatSpam";
        private const String SPAWN_PROTECTION = "SpawnProtection";
        private const String SPAWN_PROTECTION_TYPE = "SpawnProtectionType";
        private const String SPAWN_PROTECTION_RANGE = "SpawnProtectionRange";

        public Properties(String propertiesPath) : base(propertiesPath) { }

        public bool ExtendedReachDoor
        {
            get
            {
                return getValue(EXTENDED_REACH_DOOR, DEFAULT_EXTENDED_REACH_DOOR);
            }
        }

        public bool ExtendedReach
        {
            get
            {
                return getValue(EXTENDED_REACH, DEFAULT_EXTENDED_REACH);
            }
        }

        public bool ExtendedReachSign
        {
            get
            {
                return getValue(EXTENDED_REACH_SIGN, DEFAULT_EXTENDED_REACH_SIGN);
            }
        }

        public bool BlockExplosives
        {
            get
            {
                return getValue(BLOCK_EXPLOSIVES, DEFAULT_BLOCK_EXPLOSIVES);
            }
        }

        public int ExtendedReachRange
        {
            get
            {
                return getValue(EXTENDED_REACH_RANGE, DEFAULT_EXTENDED_REACH_RANGE);
            }
        }

        public bool BlockLavaFlow
        {
            get
            {
                return getValue(BLOCK_LAVA_FLOW, DEFAULT_BLOCK_LAVA_FLOW);
            }
        }

        public bool BlockWaterFlow
        {
            get
            {
                return getValue(BLOCK_WATER_FLOW, DEFAULT_BLOCK_WATER_FLOW);
            }
        }

        public bool BlockChatSpam
        {
            get
            {
                return getValue(BLOCK_CHAT_SPAM, DEFAULT_BLOCK_CHAT_SPAM);
            }
        }

        public bool BlockMovementNoclip
        {
            get
            {
                return getValue(BLOCK_MOVEMENT_NOCLIP, DEFAULT_BLOCK_MOVEMENT_NOCLIP);
            }
        }

        public bool BlockMovementTeleport
        {
            get
            {
                return getValue(BLOCK_MOVEMENT_TELEPORT, DEFAULT_BLOCK_MOVEMENT_TELEPORT);
            }
        }

        public int BlockMovementTeleportDistance
        {
            get
            {
                return getValue(BLOCK_MOVEMENT_TELEPORT_DISTANCE, DEFAULT_BLOCK_MOVEMENT_TELEPORT_DISTANCE) * 16;
            }
        }

        public bool SpawnProtection
        {
            get
            {
                return getValue(SPAWN_PROTECTION, DEFAULT_SPAWN_PROTECTION);
            }
        }

        public int SpawnProtectionType
        {
            get
            {
                return getValue(SPAWN_PROTECTION_TYPE, DEFAULT_SPAWN_PROTECTION_TYPE);
            }
        }

        public int SpawnProtectionRange
        {
            get
            {
                return getValue(SPAWN_PROTECTION_RANGE, DEFAULT_SPAWN_PROTECTION_RANGE);
            }
        }
    }
}
