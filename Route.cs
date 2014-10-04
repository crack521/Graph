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

        /// <summary>
        /// Returns this train route's starting town.
        /// </summary>
        /// <returns>starting town</returns>
        public char getStart()
        {
            return this.start;
        }

        /// <summary>
        /// Returns this train route's ending town.
        /// </summary>
        /// <returns>ending town</returns>
        public char getEnd()
        {
            return this.end;
        }

        /// <summary>
        /// Returns this train route's list of stops.
        /// </summary>
        /// <returns>list of all stops</returns>
        public List<char> getAllStops()
        {
            return this.allStops;
        }

        /// <summary>
        /// Returns this train route's total distance.
        /// </summary>
        /// <returns>total distance of the route</returns>
        public int getDistance()
        {
            return this.distance;
        }

        /// <summary>
        /// Convert's this train route's data into a string representation.
        /// </summary>
        /// <returns>string with route's data</returns>
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

        /// <summary>
        /// Equality method comparing this train route to another, passed in train route.
        /// </summary>
        /// <param name="other">another TrainRoute object</param>
        /// <returns>bool showing true if their values are the same</returns>
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

        /// <summary>
        /// Equality method comparing this train route to another object.
        /// </summary>
        /// <param name="other">another TrainRoute object</param>
        /// <returns>bool showing true if their values are the same</returns>
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

        //TODO implement override Object.GetHashCode()

        /// <summary>
        /// Function to determine if two TrainRoute objects are equal using ==
        /// </summary>
        /// <param name="route1">first TrainRoute</param>
        /// <param name="route2">second TrainRoute</param>
        /// <returns>true if they are equal</returns>
        public static bool operator ==(TrainRoute route1, TrainRoute route2)
        {
            if ((object)route1 == null || ((object)route2) == null)
                return Object.Equals(route1, route2);

            return route1.Equals(route2);
        }

        /// <summary>
        /// Function to determine if two TrainRoute objects are not equal using !=
        /// </summary>
        /// <param name="route1">first TrainRoute</param>
        /// <param name="route2">second TrainRoute</param>
        /// <returns>true if they are not equal</returns>
        public static bool operator !=(TrainRoute route1, TrainRoute route2)
        {
            if (route1 == null || route2 == null)
                return !Object.Equals(route1, route2);

            return !(route1.Equals(route2));
        }
    }
}
