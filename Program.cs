using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Trains
{
    class Program
    {
        private const string Greeting = "Welcome to the Kiwiland Railroad route planner.\nPlease enter your route.";
        private const string ShortestRoute = "s";
        private const string DistanceLessThan = "d";
        private const string MaxNumberStops = "m";
        private const string ExactNumberStops = "x";
        static void Main(string[] args)
        {
            string[] allPaths = parseArgs(args);
            int[,] routeMatrix = buildRoutesAdjacencyMatrix(allPaths);
            if (routeMatrix == null) return;
            TrainRoute[] allRoutes = findAllTrainRoutes(routeMatrix); 

            Console.WriteLine(Greeting);
            string userInput = Console.ReadLine();
            parseUserInput(userInput); //TODO: finish writing this method
        }

        /// <summary>
        /// Reads in the command line argument of a text file with all the route information.
        /// </summary>
        /// <param name="args">command line arguments</param>
        /// <returns>string array with the start town, end town, and distance between them</returns>
        private static string[] parseArgs(string[] args)
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
        private static int verifyArgCount(string[] args)
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
        /// <returns>adjacency matrix for all town to town route's distances</returns>
        private static int[,] buildRoutesAdjacencyMatrix(string[] allPaths)
        {
            int[,] routeMatrix = new int[5, 5]; //Assumes there are a max of five towns, per the problem prompt.
            string currPath;
            int startTown;
            int endTown;
            int distance;
            for (int i = 0; i < allPaths.Length - 1; i++)
            {
                currPath = allPaths[i];
                if (currPath.Length < 3 || !Char.IsLetter(currPath[0]) || !Char.IsLetter(currPath[1]))
                {
                    Console.WriteLine("Error: The input file with route information is in the incorrect format.");
                    return null;
                }
                for (int j = 2; j < currPath.Length - 1; j++)
                {
                    if (!Char.IsDigit(currPath[j]))
                    {
                        Console.WriteLine("Error: The input file with route information is in the incorrect format.");
                        return null;
                    }
                }
                startTown = charToInt(currPath[0]);
                endTown = charToInt(currPath[1]);
                if (startTown == -1 || endTown == -1) return null;
                string tempDist = null;
                for (int j = 2; j < currPath.Length - 1; j++) 
                { 
                    tempDist += currPath[j];
                }
                distance = Int32.Parse(tempDist);
                routeMatrix[startTown, endTown] = distance;
            }
            return routeMatrix;
        }

        /// <summary>
        /// Converts a char representing a town to an int
        /// </summary>
        /// <param name="x">the town char to be converted</param>
        /// <returns>the town's int conversion</returns>
        private static int charToInt(char x)
        {
            if (x == 'A' || x == 'a') return 0;
            else if (x == 'B' || x == 'b') return 1;
            else if (x == 'C' || x == 'c') return 2;
            else if (x == 'D' || x == 'd') return 3;
            else if (x == 'E' || x == 'e') return 4;
            else
            {
                Console.WriteLine("Error: A town in the route information input file not recognized.");
                return -1;
            }
        }

        /// <summary>
        /// Constructs an array of all train routes. Each route contains no more than one cycle
        /// (max one repeat of only one town).
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix containing route and weight data</param>
        /// <returns>array of all train routes</returns>
        private static TrainRoute[] findAllTrainRoutes(int[,] routeMatrix)
        {
            TrainRoute[] allRoutes = new TrainRoute[10];

            //TODO: finish writing this method
            return allRoutes;
        }

        /// <summary>
        /// Parses the user input for what kind of information they want to have the program return.
        /// </summary>
        /// <param name="userInput">user's console input</param>
        private static void parseUserInput(string userInput)
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
