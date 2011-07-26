using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria_Server.Misc;

namespace rakAntiGrief
{
    public class Properties : PropertiesFile
    {
        private const bool DEFAULT_DOOR_CHANGE = true;
        private const bool DEFAULT_TILE_CHANGE = true;
        private const bool DEFAULT_SIGN_EDIT = true;
        private const bool DEFAULT_PLAYER_PROJECTILE = true;
        private const int DEFAULT_RANGE = 6;
		private const bool DEFAULT_LAVA_FLOW = true;
		private const bool DEFAULT_WATER_FLOW = true;

        private const String DOOR_CHANGE = "DoorChange";
        private const String TILE_CHANGE = "TileChange";
        private const String SIGN_EDIT = "SignEdit";
        private const String PLAYER_PROJECTILE = "PlayerProjectile";
		private const String RANGE = "Range";
		private const String LAVA_FLOW = "LavaFlow";
		private const String WATER_FLOW = "WaterFlow";

        public Properties(String propertiesPath) : base(propertiesPath) { }

        public void pushData()
        {
            object temp = DoorChange;
            temp = TileChange;
            temp = SignEdit;
            temp = PlayerProjectile;
            temp = Range;
        }

        public bool DoorChange
        {
            get
            {
                return getValue(DOOR_CHANGE, DEFAULT_DOOR_CHANGE);
            }
        }

        public bool TileChange
        {
            get
            {
                return getValue(TILE_CHANGE, DEFAULT_TILE_CHANGE);
            }
        }

        public bool SignEdit
        {
            get
            {
                return getValue(SIGN_EDIT, DEFAULT_SIGN_EDIT);
            }
        }

        public bool PlayerProjectile
        {
            get
            {
                return getValue(PLAYER_PROJECTILE, DEFAULT_PLAYER_PROJECTILE);
            }
        }

        public int Range
        {
            get
            {
                return getValue(RANGE, DEFAULT_RANGE);
            }
        }

		public bool LavaFlow
		{
			get
			{
				return getValue(LAVA_FLOW, DEFAULT_LAVA_FLOW);
			}
		}

		public bool WaterFlow
		{
			get
			{
				return getValue(WATER_FLOW, DEFAULT_WATER_FLOW);
			}
		}
    }
}
