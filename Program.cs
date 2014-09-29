using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trains
{
    public class RoutePlanner
    {
        private const string Greeting = "Welcome to the Kiwiland Railroad route planner.\nPlease enter your route.";
        private const string ShortestRoute = "s";
        private const string DistanceLessThan = "d";
        private const string MaxNumberStops = "m";
        private const string ExactNumberStops = "x";
        private const int TotalNumberOfTowns = 5;
        static void Main(string[] args)
        {
            string[] allPaths = parseArgs(args);
            int[,] routeMatrix = buildRoutesAdjacencyMatrix(allPaths);
            if (routeMatrix == null) return;
            List<TrainRoute> allRoutes = findAllTrainRoutes(routeMatrix); 

            Console.WriteLine(Greeting);
            string userInput = Console.ReadLine();
            parseUserInput(userInput); //TODO: finish writing this method
        }

        /// <summary>
        /// Reads in the command line argument of a text file with all the route information.
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <returns>string array with the start town, end town, and distance between them at each index or null if there was an error</returns>
        public static string[] parseArgs(string[] args)
        {
            if (verifyArgCount(args) == 0) return null;

            try
            {
                using (StreamReader sr = new StreamReader(args[0]))
                {
                    String allRoutes = sr.ReadToEnd();
                    char[] delimiterChars = { ' ', ',' };
                    return allRoutes.Split(delimiterChars);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("The given file could not be read:");
                Console.WriteLine(e.Message);
                return null;
            }
        }

        /// <summary>
        /// Verifies that one argument was passed to the console.
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <returns>1 if one argument was passed, returns 0 if the number was not one.</returns>
        public static int verifyArgCount(string[] args)
        {
            if (args.Length != 1)
            {
                Console.WriteLine("Error: Run this program with a command line argument of a text file with the rail lines.");
                return 0;
            }
            return 1;
        }
        
        /// <summary>
        /// Builds an adjacency matrix of all train routes. Rows are start towns, columns are end towns.
        /// Assumes there are a max of five towns, per the problem prompt.
        /// </summary>
        /// <param name="allPaths">all train paths as parsed in from the command line arg</param>
        /// <returns>adjacency matrix for all town to town route's distances or null if there was an error</returns>
        public static int[,] buildRoutesAdjacencyMatrix(string[] allPaths)
        {
            int[,] routeMatrix = new int[TotalNumberOfTowns, TotalNumberOfTowns]; //Assumes there are a max of five towns, per the problem prompt.
            string currPath;
            int startTown;
            int endTown;
            int distance;
            for (int i = 0; i < allPaths.Length; i++)
            {
                currPath = allPaths[i];
                if (currPath.Length < 3 || !Char.IsLetter(currPath[0]) || !Char.IsLetter(currPath[1])) //3 = start town + end town + distance
                {
                    Console.WriteLine("Error: The input file with route information is in the incorrect format.");
                    return null;
                }
                for (int j = 2; j < currPath.Length; j++) //start at 2 to skip start and end towns
                {
                    if (!Char.IsDigit(currPath[j]))
                    {
                        Console.WriteLine("Error: The input file with route information is in the incorrect format.");
                        return null;
                    }
                }
                startTown = townCharToInt(currPath[0]);
                endTown = townCharToInt(currPath[1]);
                if (startTown == -1 || endTown == -1) return null;
                string tempDist = null;
                for (int j = 2; j < currPath.Length; j++) //start at 2 to skip start and end towns
                { 
                    tempDist += currPath[j];
                }
                distance = Int32.Parse(tempDist);
                if (distance < 0) //negative route weight between towns
                {
                    Console.WriteLine("Error: Distances between towns cannot be negative.");
                    return null;
                }
                routeMatrix[startTown, endTown] = distance;
            }
            return routeMatrix;
        }

        /// <summary>
        /// Converts a char representing a town to an int representing the matrix index of the town
        /// </summary>
        /// <param name="x">the town char to be converted</param>
        /// <returns>the town's int conversion or -1 if not recognized</returns>
        public static int townCharToInt(char x)
        {
            if (x == 'A' || x == 'a') return 0;
            else if (x == 'B' || x == 'b') return 1;
            else if (x == 'C' || x == 'c') return 2;
            else if (x == 'D' || x == 'd') return 3;
            else if (x == 'E' || x == 'e') return 4;
            else
            {
                Console.WriteLine("Error: A town in the route information input file was not recognized.");
                return -1;
            }
        }

        /// <summary>
        /// Constructs an array of all train routes. Each route contains no more than one cycle
        /// (max one repeat of only one town).
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix containing route and weight data</param>
        /// <returns>array of all train routes</returns>
        public static List<TrainRoute> findAllTrainRoutes(int[,] routeMatrix)
        {
            List<TrainRoute> allTrainRoutes = new List<TrainRoute>();
            int[] hasTownBeenSeen;
            int routeDistance;
            string allStops;
            for (int i = 0; i < TotalNumberOfTowns; i++) //traverse all start towns (rows)
            {
                hasTownBeenSeen = new int[TotalNumberOfTowns];
                routeDistance = 0;
                allStops = null;
                findAllPathsFromStartTown(routeMatrix, ref allTrainRoutes, i, 0, hasTownBeenSeen, routeDistance, allStops);
            }
            return allTrainRoutes;
        }

        /// <summary>
        /// Finds all paths from a given starting town with a max of one repeat of a sigle town. Once a
        /// town has been seen twice on the route, the function returns.
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix of towns and distances</param>
        /// <param name="allTrainRoutes">list of all train routes</param>
        /// <param name="startingTown">char representing starting town</param>
        /// <param name="currentTown">char representing next town to check</param>
        /// <param name="hasTownBeenSeen">array indicating which towns have been visited from starting town so far</param>
        /// <param name="routeDistance">total length of current route</param>
        /// <param name="allStops">array of all towns along the current route</param>
        /// <returns>the list of all the train routes</returns>
        public static List<TrainRoute> findAllPathsFromStartTown(int[,] routeMatrix, ref List<TrainRoute> allTrainRoutes, int startingTown, int currentTown, int[] hasTownBeenSeen, int routeDistance, string allStops)
        {
            for (int i = currentTown; i < TotalNumberOfTowns; i++)
            {
                if (routeMatrix[startingTown, currentTown] != 0) //assuming all routes are > 0 based on problem prompt
                {
                    routeDistance += routeMatrix[startingTown, currentTown];
                    allStops += currentTown;
                    TrainRoute newRoute = new TrainRoute(allStops.ToCharArray(), routeDistance, townIntToChar(startingTown), townIntToChar(currentTown));
                    allTrainRoutes.Add(newRoute);
                    if (hasTownBeenSeen[currentTown] == 1)
                    {
                        return allTrainRoutes;
                    }
                    else
                    {
                        hasTownBeenSeen[currentTown] = 1;
                        findAllPathsFromStartTown(routeMatrix, ref allTrainRoutes, startingTown, currentTown++, hasTownBeenSeen, routeDistance, allStops);
                    }               
                }
            }
            return allTrainRoutes;
        }

        /// <summary>
        /// Converts a town int (index in adjacency matrix) to char representation of the town's name
        /// </summary>
        /// <param name="x">int representation of the town</param>
        /// <returns>char representation of the town as a letter or '0' if there was an error</returns>
        public static char townIntToChar(int x){
            if (x == 0) return 'A';
            else if (x == 1) return 'B';
            else if (x == 2) return 'C';
            else if (x == 3) return 'D';
            else if (x == 4) return 'E';
            else //should not hit this case because of previous char value checking in townCharToInt()
            {
                Console.WriteLine("Error: A town index was not recognized.");
                return '0';
            }
        }

        /// <summary>
        /// Parses the user input for what kind of information they want to have the program return.
        /// </summary>
        /// <param name="userInput">user's console input</param>
        public static void parseUserInput(string userInput)
        {
            if (userInput.Contains(ShortestRoute.ToLower()))
            {

            }
            else if (userInput.Contains(DistanceLessThan.ToLower()))
            {

            }
            else if (userInput.Contains(MaxNumberStops.ToLower()))
            {

            }
            else if (userInput.Contains(ExactNumberStops.ToLower()))
            {

            }
            else if (userInput.Length == 3 && userInput.Contains("-"))
            {

            }
            else //exact route (A-B-C etc.) or error
            {

            }
        }
    }
}
