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
        
        private const String EXTENDED_REACH_DOOR = "ExtendedReachDoor";
        private const String EXTENDED_REACH = "ExtendedReach";
        private const String EXTENDED_REACH_SIGN = "ExtendedReachSign";
        private const String BLOCK_EXPLOSIVES = "BlockExplosives";
        private const String EXTENDED_REACH_RANGE = "ExtendedReachRange";
        private const String SPAM_EXPLOSIVES_KICK = "SpamExplosivesKick";
		private const String BLOCK_LAVA_FLOW = "LavaFlow";
		private const String BLOCK_WATER_FLOW = "WaterFlow";

        public Properties(String propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = ExtendedReachDoor;
            temp = ExtendedReach;
            temp = ExtendedReachSign;
            temp = BlockExplosives;
            temp = ExtendedReachRange;
        }

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
    }
}
