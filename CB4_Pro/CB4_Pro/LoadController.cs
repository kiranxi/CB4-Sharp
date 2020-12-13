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
		public void Raise()
		{


			if (Level < 65535 - 655)
				Level = (ushort)(Level + 655);
			else
				Level = 65535;
		}
		public void Lower()
		{
			if (Level > 655)
				Level = (ushort)(Level - 655);
			else
				Level = 0;

		}

	}
}
