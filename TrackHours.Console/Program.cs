using TrackHours.Domain;

namespace TrackHours.Console
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Tracker.OnStart(new string[0]);
            Tracker.PrintWorkInfo();
            System.Console.ReadLine();
        }
    }
}
