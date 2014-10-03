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
    public class TrainRoute : IEquatable<TrainRoute>
    {
        private char start;
        private char end;
        private List<char> allStops;
        private int distance;

        public TrainRoute(List<char> allStops, int distance, char start, char end)
        {
            this.allStops = allStops;
            this.distance = distance;
            this.start = start;
            this.end = end;
        }

        public TrainRoute(List<char> allStops, int distance)
        {
            //better to use this constructor; less likely to have data mismatch mistakes
            this.allStops = allStops;
            this.distance = distance;
            this.start = allStops[0];
            this.end = allStops[allStops.Count-1];
        }

        public char getStart()
        {
            return this.start;
        }

        public char getEnd()
        {
            return this.end;
        }

        public List<char> getAllStops()
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
            for (int i = 0; i < allStops.Count; i++)
            {
                info += allStops[i];
            }
            info += " | total distance: " + this.distance;
            return info;
        }

        public bool Equals(TrainRoute other)
        {
            if (other == null)
                return false;
            if(this.distance != other.getDistance())
                return false;
            for (int i = 0; i < this.getAllStops().Count; i++)
            {
                if(this.allStops[i] != other.allStops[i])
                    return false;
            }
            return true;
        }

        public override bool Equals(Object obj)
        {
            if (obj == null)
                return false;

            TrainRoute route = obj as TrainRoute;
            if (route == null)
                return false;
            else
                return Equals(route);
        }   


        public static bool operator ==(TrainRoute route1, TrainRoute route2)
        {
            if ((object)route1 == null || ((object)route2) == null)
                return Object.Equals(route1, route2);

            return route1.Equals(route2);
        }

        public static bool operator !=(TrainRoute route1, TrainRoute route2)
        {
            if (route1 == null || route2 == null)
                return !Object.Equals(route1, route2);

            return !(route1.Equals(route2));
        }
    }
}
