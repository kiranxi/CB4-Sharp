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
			
		}
		void LoadON()
        {
			Level = 65535;

        }
		void LoadOFF()
		{
			Level = 0;

		}

	}
}
