using System;
using System.Diagnostics;

namespace TrackHours.Domain
{
    public class Logger
    {
        public static void Info(string message)
        {
            Trace.WriteLine($"TrackHours: {DateTime.Now.ToShortTimeString()} - {message}");
            Console.WriteLine($"TrackHours: {DateTime.Now.ToShortTimeString()} - {message}");
        }
    }
}
