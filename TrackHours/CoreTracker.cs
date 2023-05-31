using System.ServiceProcess;
using TrackHours.Domain;

namespace TrackHours
{
    public class CoreTracker : ServiceBase
    {
        protected override void OnStart(string[] args)
        {
            Tracker.OnStart(args);
            base.OnStart(args);
        }

        protected override void OnStop()
        {
            Tracker.OnStop();
            base.OnStop();
        }

        private void InitializeComponent()
        {
            // 
            // CoreTracker
            // 
            this.CanHandlePowerEvent = true;
            this.CanHandleSessionChangeEvent = true;
            this.CanPauseAndContinue = true;
            this.CanShutdown = true;
            this.ServiceName = "CoreTracker";

        }
    }
}
