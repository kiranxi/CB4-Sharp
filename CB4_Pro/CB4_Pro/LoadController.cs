using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CB4_Pro
{
    class LoadController
    {
        public bool ON;
        public bool OFF;
		public ushort Level;

		public LoadController()
		{
			if (ON)
			{
				Level = 65535;
				OFF = false;
			}
			if (OFF)
			{
				Level = 0;
				ON = false;
			}
		}

	}
}
