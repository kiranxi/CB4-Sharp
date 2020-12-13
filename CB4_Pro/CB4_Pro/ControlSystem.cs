using System;
using Crestron.SimplSharp;                          	// For Basic SIMPL# Classes
using Crestron.SimplSharpPro;                       	// For Basic SIMPL#Pro classes
using Crestron.SimplSharpPro.CrestronThread;        	// For Threading
using Crestron.SimplSharpPro.Diagnostics;		    	// For System Monitor Access
using Crestron.SimplSharpPro.DeviceSupport;         	// For Generic Device Support
using Crestron.SimplSharpPro.UI;
using Crestron.SimplSharpPro.Lighting;
using Timer = System.Timers.Timer;
using System.Timers;



namespace CB4_Pro
{
    


    public class ControlSystem : CrestronControlSystem
    {
        private Tsw1070 UI;
        LoadController Room1 = new LoadController();
        public Timer loopTimer;
        public int TempDevId;
        /// <summary>
        /// ControlSystem Constructor. Starting point for the SIMPL#Pro program.
        /// Use the constructor to:
        /// * Initialize the maximum number of threads (max = 400)
        /// * Register devices
        /// * Register event handlers
        /// * Add Console Commands
        /// 
        /// Please be aware that the constructor needs to exit quickly; if it doesn't
        /// exit in time, the SIMPL#Pro program will exit.
        /// 
        /// You cannot send / receive data in the constructor
        /// </summary>
        public ControlSystem()
            : base()
        {
            
            try
            {   
                loopTimer = new Timer();
                loopTimer.Interval = 100;
                loopTimer.Enabled = false;
                loopTimer.Elapsed += LoopTimerEvent;
                loopTimer.AutoReset = true;



                Thread.MaxNumberOfUserThreads = 20;
                ErrorLog.Error("CB4 Started");

                UI= new Tsw1070(06, this);
                UI.SigChange += UI_SigChange;
                UI.Register();

                //Subscribe to the controller events (System, Program, and Ethernet)
                CrestronEnvironment.SystemEventHandler += new SystemEventHandler(_ControllerSystemEventHandler);
                CrestronEnvironment.ProgramStatusEventHandler += new ProgramStatusEventHandler(_ControllerProgramEventHandler);
                CrestronEnvironment.EthernetEventHandler += new EthernetEventHandler(_ControllerEthernetEventHandler);



            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in the constructor: {0}", e.Message);
            }
        }

        private void LoopTimerEvent(Object source, ElapsedEventArgs e)
        {

            if (TempDevId == 3)
                Room1.Raise();
            else if (TempDevId == 4)
                Room1.Lower();

            UI.UShortInput[1].UShortValue = Room1.Level;
        }
        private void UI_SigChange(BasicTriList currentDevice, SigEventArgs args)
        {

            switch (args.Sig.Type)
            {
                case eSigType.Bool:
                    {
                        if (args.Sig.BoolValue) /// ON OFF using a Class
                        {
                            if (args.Sig.Number == 1)
                            {
                                Room1.LoadON();
                            }
                            if (args.Sig.Number == 2)
                            {
                                Room1.LoadOFF();
                            }
                            if (args.Sig.Number == 3)
                            {
                                TempDevId = 3;
                                loopTimer.Enabled = true;

                            }
                            if (args.Sig.Number == 4)
                            {
                                TempDevId = 4;
                                loopTimer.Enabled = true;
                            }



                        }
                        else if (args.Sig.BoolValue == false)
                        {
                            loopTimer.Enabled = false;
                        }

                        UI.UShortInput[1].UShortValue = Room1.Level;

                        break;
                    }
                case eSigType.UShort:
                    {
                        UI.UShortInput[1].UShortValue = UI.UShortOutput[1].UShortValue;
                        Room1.Level = UI.UShortOutput[1].UShortValue;
                        break;

                    }
                case eSigType.String:
                    {
                        break;
                    }

            }

        }
        



      
        /// <summary>
        /// InitializeSystem - this method gets called after the constructor 
        /// has finished. 
        /// 
        /// Use InitializeSystem to:
        /// * Start threads
        /// * Configure ports, such as serial and verisports
        /// * Start and initialize socket connections
        /// Send initial device configurations
        /// 
        /// Please be aware that InitializeSystem needs to exit quickly also; 
        /// if it doesn't exit in time, the SIMPL#Pro program will exit.
        /// </summary>
        public override void InitializeSystem()
        {
            try
            {

            }
            catch (Exception e)
            {
                ErrorLog.Error("Error in InitializeSystem: {0}", e.Message);
            }
        }

        /// <summary>
        /// Event Handler for Ethernet events: Link Up and Link Down. 
        /// Use these events to close / re-open sockets, etc. 
        /// </summary>
        /// <param name="ethernetEventArgs">This parameter holds the values 
        /// such as whether it's a Link Up or Link Down event. It will also indicate 
        /// wich Ethernet adapter this event belongs to.
        /// </param>
        void _ControllerEthernetEventHandler(EthernetEventArgs ethernetEventArgs)
        {
            switch (ethernetEventArgs.EthernetEventType)
            {//Determine the event type Link Up or Link Down
                case (eEthernetEventType.LinkDown):
                    //Next need to determine which adapter the event is for. 
                    //LAN is the adapter is the port connected to external networks.
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {
                        //
                    }
                    break;
                case (eEthernetEventType.LinkUp):
                    if (ethernetEventArgs.EthernetAdapter == EthernetAdapterType.EthernetLANAdapter)
                    {

                    }
                    break;
            }
        }

        /// <summary>
        /// Event Handler for Programmatic events: Stop, Pause, Resume.
        /// Use this event to clean up when a program is stopping, pausing, and resuming.
        /// This event only applies to this SIMPL#Pro program, it doesn't receive events
        /// for other programs stopping
        /// </summary>
        /// <param name="programStatusEventType"></param>
        void _ControllerProgramEventHandler(eProgramStatusEventType programStatusEventType)
        {
            switch (programStatusEventType)
            {
                case (eProgramStatusEventType.Paused):
                    //The program has been paused.  Pause all user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Resumed):
                    //The program has been resumed. Resume all the user threads/timers as needed.
                    break;
                case (eProgramStatusEventType.Stopping):
                    //The program has been stopped.
                    //Close all threads. 
                    //Shutdown all Client/Servers in the system.
                    //General cleanup.
                    //Unsubscribe to all System Monitor events
                    break;
            }

        }

        /// <summary>
        /// Event Handler for system events, Disk Inserted/Ejected, and Reboot
        /// Use this event to clean up when someone types in reboot, or when your SD /USB
        /// removable media is ejected / re-inserted.
        /// </summary>
        /// <param name="systemEventType"></param>
        void _ControllerSystemEventHandler(eSystemEventType systemEventType)
        {

           


            switch (systemEventType)
            {
                case (eSystemEventType.DiskInserted):
                    //Removable media was detected on the system
                    break;
                case (eSystemEventType.DiskRemoved):
                    //Removable media was detached from the system
                    break;
                case (eSystemEventType.Rebooting):
                    //The system is rebooting. 
                    //Very limited time to preform clean up and save any settings to disk.
                    break;
            }

        }
    }
}
