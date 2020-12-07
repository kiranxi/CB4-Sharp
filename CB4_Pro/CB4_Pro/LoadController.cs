using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CB4_Pro
{
    class LoadController
    {

		public ushort Level;

		public LoadController()
		{
			
		}
		public void LoadON()
        {
			Level = 65535;

        }
		public void LoadOFF()
		{
			Level = 0;

		}

	}
}
