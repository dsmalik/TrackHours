using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace TrackHours.Domain
{
    public class Tracker
    {
        static long totalTimeTillDate = 0;
        static string appDataFolder = "C:\\";// Environment.SpecialFolder.ApplicationData.ToString();
        static string dirPath = $@"{appDataFolder}\dm";
        static FileInfo fileInfo = new FileInfo($@"{appDataFolder}\dm\time.txt");
        static int timerInterval = 1 * 60 * 1000;

        static Timer t;

        public static void IncrementTime(object state)
        {
            var isLoggedIn = !Process.GetProcessesByName("logonui").Any();

            if (isLoggedIn)
            {
                totalTimeTillDate += timerInterval;

                File.WriteAllText(fileInfo.FullName, totalTimeTillDate.ToString());

                PrintWorkInfo();
                t.Change(timerInterval, Timeout.Infinite);
            }
            else
            {
                PrintWorkInfo();
                // Lets run every 5s to check if user logs in
                t.Change(5 * 1000, Timeout.Infinite);
            }
        }

        public static void OnStart(string[] args)
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
            catch (Exception ex)
            {
                Logger.Info(ex.ToString());
                throw;
            }

            Logger.Info("Timer is starting");
            t.Change(timerInterval, Timeout.Infinite);
        }

        public static void PrintWorkInfo()
        {
            var daysSinceFileCreated = DateTime.UtcNow.Subtract(fileInfo.CreationTimeUtc).TotalDays / 7;
            var expectedHoursTillDate = TimeSpan.FromHours(8 * (daysSinceFileCreated < 1 ? 1 : daysSinceFileCreated));
            var totalWorkHoursPerLogs = TimeSpan.FromMilliseconds(totalTimeTillDate);
            var extraHours = totalWorkHoursPerLogs - expectedHoursTillDate;

            Logger.Info($"Actual hours - {Format(totalWorkHoursPerLogs)}, Expected - {Format(expectedHoursTillDate)}, Extra hours - {Format(extraHours)}, Total msec - {totalTimeTillDate}");
        }

        private static string Format(TimeSpan timeSpan)
        {
            return $"{Math.Floor(timeSpan.TotalHours)}:{Math.Abs(timeSpan.Minutes)}";
        }

        public static void OnStop()
        {
            Logger.Info("Service has been stopped");
        }
    }
}
