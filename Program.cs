using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Trains
{
    public class RoutePlanner
    {
        private const string Greeting = "Welcome to the railroad route planner. ";
        private const string LoopGreeting = "Enter your route:";
        private const string ShortestRoute = "s";
        private const string DistanceLessThan = "l";
        private const string MaxNumberStops = "m";
        private const string ExactNumberStops = "x";
        private const char ExactPath = 'p';
        private const int TotalNumberOfTowns = 5;
        static void Main(string[] args)
        {
            string[] allPaths = parseArgs(args);
            int[,] routeMatrix = buildRoutesAdjacencyMatrix(allPaths);
            if (routeMatrix == null) return;

            Console.WriteLine(Greeting);
            string userInput;
            List<char> inputTowns = new List<char>();
            char typeOfOutputToGive;
            int numberUserPasses = 0;
            string infoToPrint = null;
            List<TrainRoute> rts = null;

            while (true)
            {
                rts = null;
                inputTowns.Clear();
                numberUserPasses = 0;
                infoToPrint = "";
                Console.WriteLine();
                Console.WriteLine(LoopGreeting);
                userInput = Console.ReadLine();
                typeOfOutputToGive = parseUserInput(userInput, ref inputTowns, ref numberUserPasses);
                char[] townsAsChars = inputTowns.ToArray(); 
                int[] townsAsInts = new int[townsAsChars.Length];
                for (int i = 0; i < townsAsChars.Length; i++)
                {
                    townsAsInts[i] = townCharToInt(townsAsChars[i]);
                } //builds an int array of all the towns user specifies to be used for indecies in the adj. matrix (converted to correct ints)

                if (typeOfOutputToGive.ToString() == ExactNumberStops){
                    rts = findAllTrainRoutes(routeMatrix, townsAsInts[0], townsAsInts[inputTowns.Count - 1], stopsEqTo: numberUserPasses); 
                } else if (typeOfOutputToGive.ToString() == MaxNumberStops){
                    rts = findAllTrainRoutes(routeMatrix, townsAsInts[0], townsAsInts[inputTowns.Count - 1], stopsLessEqTo: numberUserPasses); 
                } else if (typeOfOutputToGive.ToString() == DistanceLessThan){
                    rts = findAllTrainRoutes(routeMatrix, townsAsInts[0], townsAsInts[inputTowns.Count - 1], distLessThan: numberUserPasses); 
                } else if (typeOfOutputToGive.ToString() == ShortestRoute){
                    infoToPrint = findAllTrainDists(routeMatrix, startAndEndTown: townsAsInts); 
                } else if (typeOfOutputToGive == ExactPath){
                    infoToPrint = findAllTrainDists(routeMatrix, exactPath: townsAsInts);
                }
                else
                {
                    infoToPrint = "Error: type of output that is desired cannot be found.";
                    continue;
                }

                if (rts != null ) 
                {
                    infoToPrint = rts.Count.ToString(); 
                }

                if (infoToPrint.Contains("0")){
                    Console.WriteLine("NO SUCH ROUTE");
                } else {
                    Console.WriteLine(infoToPrint);
                }
            }
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

                using (StreamReader sr = new StreamReader(Path.GetFullPath(args[0])))
                {
                    String allRoutes = sr.ReadToEnd();
                    char[] delimiterChars = { ' ', ',' };
                    string[] allRoutesStrings = allRoutes.Split(delimiterChars, StringSplitOptions.RemoveEmptyEntries);
                    if (allRoutesStrings.Length == 0) return null;
                    return allRoutesStrings;
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
            if (args == null || args.Length != 1)
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
            if (allPaths == null) return null;
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
        /// Constructs an array of TrainRoute objects representing routes that fit the criteria given by the user input.
        /// 
        /// Set a value for either distLessThan, stopsEqTo, or stopsLessEqTo based on user input of what
        /// kind of search should be performed.
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix of train routes and weights</param>
        /// <param name="startTown">starting town (node) from user input</param>
        /// <param name="endTown">target end town from user input</param>
        /// <param name="distLessThan">total distance that all routes should be less than, from user input</param>
        /// <param name="stopsEqTo">total number of stops routes should have, from user input</param>
        /// <param name="stopsLessEqTo">total number of stops route should have less than or be equal to, from user input</param>
        /// <returns>returns a List of all eligible train routes based on user input</returns>
        public static List<TrainRoute> findAllTrainRoutes(int[,] routeMatrix, int startTown, int endTown, int distLessThan = -1, int stopsEqTo = -1, int stopsLessEqTo = -1)
        {
            List<TrainRoute> allTrainRoutes = new List<TrainRoute>();
            List<char> allStops = new List<char>();
            allStops.Add(townIntToChar(startTown));
            int routeDistance = 0;
            if (distLessThan > -1)
            {
                distanceLessThan(routeMatrix, ref allTrainRoutes, startTown, endTown, distLessThan, routeDistance, allStops);
            }
            else if (stopsEqTo > -1)
            {
                stopsEqualTo(routeMatrix, ref allTrainRoutes, startTown, endTown, stopsEqTo, routeDistance, allStops);
            }
            else if (stopsLessEqTo > -1)
            {
                stopsLessThanEqualTo(routeMatrix, ref allTrainRoutes, startTown, endTown, stopsLessEqTo, routeDistance, allStops);
            }
            else
            {
                Console.WriteLine("This function must be passed a single parameter to specify the type of routes to find.");
                return null;
            }
            return allTrainRoutes; //if allTrainRoutes.Count == 0, then there were no routes found
        }

        /// <summary>
        /// Constructs a string with the distance that user wants to know based on input of either
        /// an int array, exactPath or startAndEndTown
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix of train routes and weights</param>
        /// <param name="exactPath">int representations of all towns that should be visited</param>
        /// <param name="twoTowns">int representation of start and end town that should be visited</param>
        /// <returns>string representation of an int showing the distance of the path the user wants to find</returns>
        public static string findAllTrainDists(int[,] routeMatrix, int[] exactPath = null, int[] startAndEndTown = null)
        {
            string infoToPrint;
            if (exactPath != null)
            {
                infoToPrint = findDistGivenExactPath(routeMatrix, exactPath); //ExactPath
            } else if(startAndEndTown != null){
                infoToPrint = findDistGivenStartEnd(routeMatrix, startAndEndTown); //ShortestRoute
            } else {
                Console.WriteLine("This function must be passed a single parameter to specify the route distance to calculate.");
                return null;
            }
            return infoToPrint;
        }

        /// <summary>
        /// Recursive function to fill a list of all routes between two give towns that has
        /// a number of stops less or equal to the given number, from user input.
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix of towns and distances</param>
        /// <param name="allTrainRoutes">list of all train routes</param>
        /// <param name="currTown">starting town</param>
        /// <param name="endTown">goal end town</param>
        /// <param name="stopsLessEqTo">number of stops that routes should have, either equal to or less than</param>
        /// <param name="routeDistance">the total distance of the current route (0 to start)</param>
        /// <param name="allStops">list of all stops that is being filled by this function</param>
        private static void stopsLessThanEqualTo(int[,] routeMatrix, ref List<TrainRoute> allTrainRoutes, int currTown, int endTown, int stopsLessEqTo, int routeDistance, List<char> allStops)
        {
            if (allStops.Count > 1) //handles if start and end town are the same
            {
                if (allStops.Count > stopsLessEqTo + 1)
                {
                    return; //if there are too many stops in the list for this route, this route will never work
                }
                if (townCharToInt(allStops[allStops.Count - 1]) == endTown && allStops.Count <= (stopsLessEqTo + 1))
                {
                    TrainRoute newRoute = new TrainRoute(allStops, routeDistance);
                    if (!allTrainRoutes.Contains(newRoute))
                    {
                        allTrainRoutes.Add(newRoute);
                    }
                }
            }
            for (int i = 0; i < TotalNumberOfTowns; i++)
            {
                if (routeMatrix[currTown, i] != 0) //assuming all routes are > 0 based on problem prompt
                {
                    List<char> newAllStops = new List<char>();
                    int newRouteDistance = routeDistance;
                    for (int j = 0; j < allStops.Count; j++)
                    {
                        newAllStops.Add(allStops[j]);
                    }
                    newAllStops.Add(townIntToChar(i));
                    newRouteDistance += routeMatrix[currTown, i];
                    stopsLessThanEqualTo(routeMatrix, ref allTrainRoutes, i, endTown, stopsLessEqTo, newRouteDistance, newAllStops);
                }
            }
        }

        /// <summary>
        /// Recursive function to fill a list of all routes between two give towns that has
        /// a number of stops equal to the given number, from user input.
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix of towns and distances</param>
        /// <param name="allTrainRoutes">list of all train routes</param>
        /// <param name="currTown">starting town</param>
        /// <param name="endTown">goal end town</param>
        /// <param name="stopsEqTo">exact number of stops that routes should have</param>
        /// <param name="routeDistance">the total distance of the current route (0 to start)</param>
        /// <param name="allStops">list of all stops that is being filled by this function</param>
        private static void stopsEqualTo(int[,] routeMatrix, ref List<TrainRoute> allTrainRoutes, int currTown, int endTown, int stopsEqTo, int routeDistance, List<char> allStops)
        {
            if (allStops.Count > 1) //handles if start and end town are the same
            {
                if (allStops.Count > stopsEqTo + 1)
                {
                    return; //if there are too many stops in the list for this route, this route will never work
                }
                if (townCharToInt(allStops[allStops.Count - 1]) == endTown && allStops.Count == (stopsEqTo + 1))
                {
                    TrainRoute newRoute = new TrainRoute(allStops, routeDistance);
                    if (!allTrainRoutes.Contains(newRoute))
                    {
                        allTrainRoutes.Add(newRoute);
                    }
                }
            }
            for (int i = 0; i < TotalNumberOfTowns; i++)
            {
                if (routeMatrix[currTown, i] != 0) //assuming all routes are > 0 based on problem prompt
                {
                    List<char> newAllStops = new List<char>();
                    int newRouteDistance = routeDistance;
                    for (int j = 0; j < allStops.Count; j++)
                    {
                        newAllStops.Add(allStops[j]);
                    }
                    newAllStops.Add(townIntToChar(i));
                    newRouteDistance += routeMatrix[currTown, i];
                    stopsEqualTo(routeMatrix, ref allTrainRoutes, i, endTown, stopsEqTo, newRouteDistance, newAllStops);
                }
            }
        }

        /// <summary>
        /// Recursive function to find fill a list of all routes with given start and end town with a total 
        /// distance less than the given.
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix of towns and distances</param>
        /// <param name="allTrainRoutes">list of all train routes</param>
        /// <param name="currTown">starting town</param>
        /// <param name="endTown">goal end town</param>
        /// <param name="distLessThan">distance user wants all routes to be less than</param>
        /// <param name="routeDistance">the total distance of the current route (0 to start)</param>
        /// <param name="allStops">list of all stops that is being filled by this function (empty to start)</param>
        private static void distanceLessThan(int[,] routeMatrix, ref List<TrainRoute> allTrainRoutes, int currTown, int endTown, int distLessThan, int routeDistance, List<char> allStops)
        {
            if (allStops.Count > 1) //handles if start and end town are the same
            {
                if (routeDistance >= distLessThan)
                {
                    return; //if there are too many stops in the list for this route, this route will never work
                }
                if (routeDistance < distLessThan && currTown == endTown)
                {
                    TrainRoute newRoute = new TrainRoute(allStops, routeDistance);
                    if (!allTrainRoutes.Contains(newRoute))
                    {
                        allTrainRoutes.Add(newRoute);
                    }                  
                }
            }
            for (int i = 0; i < TotalNumberOfTowns; i++)
            {
                if (routeMatrix[currTown, i] != 0) //assuming all routes are > 0 based on problem prompt
                {
                    List<char> newAllStops = new List<char>();
                    int newRouteDistance = routeDistance;
                    for(int j = 0; j < allStops.Count; j++)
                    {
                        newAllStops.Add(allStops[j]);
                    }
                    newAllStops.Add(townIntToChar(i));
                    newRouteDistance += routeMatrix[currTown, i];
                    distanceLessThan(routeMatrix, ref allTrainRoutes, i, endTown, distLessThan, newRouteDistance, newAllStops);
                }
            }
        }

        /// <summary>
        /// Returns the total distance of a given exact train route of towns.
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix of towns and distances</param>
        /// <param name="exactPath">exact path the user wants the distance of</param>
        /// <returns>distance of the given path</returns>
        public static string findDistGivenExactPath(int[,] routeMatrix, int[] exactPath = null)
        {
            int routeDistance = 0;
            for (int i = 0; i < exactPath.Length - 1 ; i++)
            {
                if (i < exactPath.Length - 1){
                    if(routeMatrix[exactPath[i], exactPath[i + 1]] != 0){
                        routeDistance = routeDistance + routeMatrix[exactPath[i], exactPath[i + 1]];
                    } else { //no value here so the path is invalid
                        routeDistance = 0;
                        break;
                    }                  
                }
            }
            return routeDistance.ToString();
        }

        /// <summary>
        /// Finds all paths with a distance less than 100 then searches them to find the path with the shortest route.
        /// 
        /// Optimization: with more time, this function would have been written to be an implementation of Dikjstra's 
        /// shortest path algorithm with a sigle source and destination. This also should not make an assumtion for 
        /// route distances, but given the sample input data for the problem, 100 can be assumed to be a safe number
        /// for now.
        /// </summary>
        /// <param name="routeMatrix">adjacency matrix of towns and distances</param>
        /// <param name="startAndEndTown">int array where [0] holds start town and [1] holds end town</param>
        /// <returns>string representation of the distance of the shortest path found</returns>
        public static string findDistGivenStartEnd(int[,] routeMatrix, int[] startAndEndTown)
        {
            List<TrainRoute> possibleRoutes = new List<TrainRoute>(); //gets filled with all eligible routes
            List<char> allStops = new List<char>();
            allStops.Add(townIntToChar(startAndEndTown[0])); //add the start town to the list
            distanceLessThan(routeMatrix, ref possibleRoutes, startAndEndTown[0], startAndEndTown[1], 100, 0, allStops);
            int shortestPath = int.MaxValue;
            foreach (TrainRoute route in possibleRoutes) //find shortest path of all returned paths
            { 
                if (route.getDistance() < shortestPath)
                {
                    shortestPath = route.getDistance();
                }
            }
            return shortestPath.ToString(); //distance of the shortest path found
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
            else //should not hit this case because of previous char value checking in townCharToInt() when program runs
            {
                Console.WriteLine("Error: A town index was not recognized.");
                return '0';
            }
        }

        /// <summary>
        /// Parses user's input. Returned char corresponds to one of the five char (or string) class constants
        /// in this class. If there was an error a ' ' char is returned.
        /// Also finds all town chars the user gives for the route and adds them to inputTowns and finds any
        /// numbers that the user gives for distance, stops, etc and adds it to numberUserPasses.
        /// (All numbers found in the input string will be concatonated together into one int!)
        /// </summary>
        /// <param name="userInput">char representing type of output user wants</param>
        /// <param name="inputTowns">list of towns (as letters) the user wants in their route</param>
        /// <param name="numberUserPasses">char representing type of ourput user wants.</param>
        /// <returns></returns>
        public static char parseUserInput(string userInput, ref List<char> inputTowns, ref int numberUserPasses)
        {
            //TODO break this up into three methods, for finding the towns, the number, and the type of data
            //TODO add quit option; if user enters Q, quit, etc, in main the program should exit the while loop
            char typeOfOutputToGive;

            string townPattern = @"[A-Ea-e]+"; //at least one characer between A-E, case insensitive
            Regex rgx = new Regex(townPattern, RegexOptions.IgnoreCase);
            MatchCollection matches = rgx.Matches(userInput);
            if (matches.Count > 0)
            {
                foreach(Match mtchs in matches){
                    inputTowns.Add(char.Parse(mtchs.ToString())); //copies the towns to the list reference
                }
            } else {
                return ' '; //no towns found, error message given in main()
            }

            string resultString = string.Join(string.Empty, Regex.Matches(userInput, @"\d+").OfType<Match>().Select(m => m.Value));
            //this gets every digit from user's input and puts it into a string (I didn't create it)
            if (!resultString.Equals("")) {
                numberUserPasses = Int32.Parse(resultString);
            }

            //find what kind of data the user wants
            if (userInput.ToLower().Contains(ShortestRoute))
            {
                typeOfOutputToGive = Char.Parse(ShortestRoute);
            }
            else if (userInput.ToLower().Contains(DistanceLessThan))
            {
                typeOfOutputToGive = Char.Parse(DistanceLessThan);
            }
            else if (userInput.ToLower().Contains(MaxNumberStops))
            {
                typeOfOutputToGive = Char.Parse(MaxNumberStops);
            }
            else if (userInput.ToLower().Contains(ExactNumberStops))
            {
                typeOfOutputToGive = Char.Parse(ExactNumberStops);
            }
            else if (userInput.ToLower().Contains(ExactPath))
            {
                typeOfOutputToGive = ExactPath;
            }
            else {
                Console.WriteLine("An error occured. Please enter a character that represents the type of data you need.");
                typeOfOutputToGive = ' ';
            }
            return typeOfOutputToGive;
        }
    }
}
