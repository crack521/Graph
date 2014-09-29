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
    class TrainRoute
    {
        private char start;
        private char end;
        private char[] allStops;
        private int distance;

        public TrainRoute(char[] route, int distance)
        {
            this.allStops = route;
            this.distance = distance;
        }

        public char getStart()
        {
            return this.start;
        }

        public void setStart(char start)
        {
            this.start = start;
        }

        public char getEnd()
        {
            return this.end;
        }

        public void setEnd(char end){
            this.end = end;
        }

        public char[] getRoute()
        {
            return this.allStops;
        }

        public int getDistance()
        {
            return this.distance;
        }
    }
}
