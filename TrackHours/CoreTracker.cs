using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.ServiceProcess;
using System.Threading;

namespace TrackHours
{
    public class CoreTracker : ServiceBase
    {
        static long totalTimeTillDate = 0;
        static string appDataFolder = Environment.SpecialFolder.LocalApplicationData.ToString();
        static string dirPath = $@"{appDataFolder}\dm101956";
        static FileInfo fileInfo = new FileInfo($@"{appDataFolder}\dm101956\time1.txt");
        static int timerInterval = 1 * 60 * 1000;

        static Timer t;

        public static void IncrementTime(object state)
        {
            var isLoggedIn = !Process.GetProcessesByName("logonui").Any();

            if (isLoggedIn)
            {
                PrintWorkInfo();
                totalTimeTillDate += timerInterval;

                File.WriteAllText(fileInfo.FullName, totalTimeTillDate.ToString());

                t.Change(timerInterval, Timeout.Infinite);
            } else
            {
                PrintWorkInfo();
                // Lets run every 5s to check if user logs in
                t.Change(5 * 1000, Timeout.Infinite);
            }

        }

        protected override void OnStart(string[] args)
        {   
            Logger.Info("Service has been started");

            try
            {
                if (fileInfo.Exists)
                {
                    totalTimeTillDate = long.Parse(File.ReadAllText(fileInfo.FullName));
                }
                else
                {
                    if (!Directory.Exists(dirPath))
                    {
                        Logger.Info("Created directory - " + dirPath);
                        Directory.CreateDirectory(dirPath);
                    }

                    Logger.Info("Created file to track time - " + fileInfo.FullName);

                    File.WriteAllText(fileInfo.FullName, "0");
                    totalTimeTillDate = 0;
                    Logger.Info("Full path is - " + fileInfo.FullName);
                }

                PrintWorkInfo();
                t = new Timer(IncrementTime, totalTimeTillDate, Timeout.Infinite, Timeout.Infinite);
            }
            catch(Exception ex)
            {
                Logger.Info(ex.ToString());
                throw;
            }

            Logger.Info("Timer is starting");
            t.Change(timerInterval, Timeout.Infinite);

            base.OnStart(args);
        }

        private static void PrintWorkInfo()
        {
            int daysSinceFileCreated = (int) DateTime.UtcNow.Subtract(fileInfo.CreationTimeUtc).TotalDays;
            int expectedHoursTillDate = 8 * (daysSinceFileCreated < 1 ? 1 : daysSinceFileCreated);
            var totalWorkHoursPerLogs = Humanizer.NumberToTimeSpanExtensions.Milliseconds(totalTimeTillDate).Hours;
            var extraHours = totalWorkHoursPerLogs - expectedHoursTillDate;
            Logger.Info($"Actual hours - {totalWorkHoursPerLogs}, Expected - {expectedHoursTillDate}, Extra hours - {extraHours}, IsLoggedIn - {Process.GetProcessesByName("logonui").Any()}, Total msec - {totalTimeTillDate}");
        }

        protected override void OnStop()
        {
            Logger.Info("Service has been stopped");
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
