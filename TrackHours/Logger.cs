using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TrackHours
{
    internal class Logger
    {
        public static void Info(string message)
        {
            Trace.WriteLine($"TrackHours: {DateTime.Now.ToShortTimeString()} - {message}");
        }
    }
}
