using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trains
{
    /// <summary>
    /// Representation of train route information. Start represents the starting 
    /// city on the route, end is the ending city on the route, allStops is an 
    /// array of all cities along the route, and distance is the sum of all path 
    /// weights (distances between cities) along the route.
    /// </summary>
    public class TrainRoute
    {
        private char start;
        private char end;
        private char[] allStops;
        private int distance;

        public TrainRoute(char[] allStops, int distance, char start, char end)
        {
            this.allStops = allStops;
            this.distance = distance;
            this.start = start;
            this.end = end;
        }

        public char getStart()
        {
            return this.start;
        }

        public char getEnd()
        {
            return this.end;
        }

        public char[] getAllStops()
        {
            return this.allStops;
        }

        public int getDistance()
        {
            return this.distance;
        }

        public string toString()
        {
            string info = "starting town: " + this.start + " | ending town: " + this.end + " | all stops on route: ";
            for (int i = 0; i < allStops.Length; i++)
            {
                info += allStops[i];
            }
            info += " | total distance: " + this.distance;
            return info;
        }
    }
}
