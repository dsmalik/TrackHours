using System;
using System.Diagnostics;

namespace TrackHours.Lib
{
    public class Logger
    {
        public static void Info(string message)
        {
            Debug.WriteLine($"TrackHours: {DateTime.Now.ToString()} - {message}");
        }
    }
}
