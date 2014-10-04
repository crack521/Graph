using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using Trains;

namespace TrainTests
{
    [TestClass]
    public class TrainTests
    {
        [TestMethod]
        public void RoutePlanner_ParseArgs()
        {
            string testFileDir = @"C:\Users\Public\TestFolder\";
            System.IO.Directory.CreateDirectory(testFileDir);
            string fileName = System.IO.Path.GetRandomFileName();
            string pathString = System.IO.Path.Combine(testFileDir, fileName);

            string sampleGraph = "AB3, AE2, BC4, BE9, CD8, DC8, DA5, ED3";
            System.IO.File.WriteAllText(pathString, sampleGraph);
            string[] testArgs = { pathString };
            string[] results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNotNull(results);
            Assert.AreEqual(8, results.Length);

            sampleGraph = "CD5";
            System.IO.File.WriteAllText(pathString, sampleGraph);
            results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNotNull(results);

            sampleGraph = " ";
            System.IO.File.WriteAllText(pathString, sampleGraph);
            results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNull(results);

            sampleGraph = "";
            System.IO.File.WriteAllText(pathString, sampleGraph);
            results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNull(results);

            sampleGraph = null;
            System.IO.File.WriteAllText(pathString, sampleGraph);
            results = RoutePlanner.parseArgs(testArgs);
            Assert.IsNull(results);
        }

        [TestMethod]
        public void RoutePlanner_VerifyArgCount()
        {
            string[] args = {"input.txt"};
            Assert.AreEqual(1, RoutePlanner.verifyArgCount(args));
        }

        [TestMethod]
        public void RoutePlanner_TownCharToInt()
        {
            Assert.AreEqual(0, RoutePlanner.townCharToInt('A'));
            Assert.AreEqual(3, RoutePlanner.townCharToInt('d'));
            Assert.AreEqual(-1, RoutePlanner.townCharToInt('q'));
            Assert.AreEqual(-1, RoutePlanner.townCharToInt('7'));
            Assert.AreEqual(-1, RoutePlanner.townCharToInt('%'));
        }

        [TestMethod]
        public void RoutePlanner_TownIntToChar()
        {
            Assert.AreEqual('A', RoutePlanner.townIntToChar(0));
            Assert.AreEqual('C', RoutePlanner.townIntToChar(2));
            Assert.AreEqual('0', RoutePlanner.townIntToChar(5));
            Assert.AreEqual('0', RoutePlanner.townIntToChar(5096300));
            Assert.AreEqual('0', RoutePlanner.townIntToChar(-10));
        }

        [TestMethod]
        public void RoutePlanner_BuildRoutesAdjacencyMatrix()
        {
            string[] allPaths1 = { "AB3", "BC40", "CD8", "ED3", "EB1000"};
            int[,] adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths1);
            Assert.AreEqual(adjMatrix[0, 1], 3);
            Assert.AreEqual(adjMatrix[1, 2], 40);
            Assert.AreEqual(adjMatrix[2, 3], 8);
            Assert.AreEqual(adjMatrix[4, 3], 3);
            Assert.AreEqual(adjMatrix[4, 1], 1000);
            Assert.AreEqual(adjMatrix[1, 1], 0);
            Assert.AreEqual(adjMatrix[4, 2], 0);

            string[] allPaths2 = {"ZA5"};
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            allPaths2[0] = "BCA3";
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            allPaths2[0] = "EA-3";
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            allPaths2[0] = "3857";
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            allPaths2[0] = "";
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths2);
            Assert.IsNull(adjMatrix);

            string[] allPaths3 = null;
            adjMatrix = RoutePlanner.buildRoutesAdjacencyMatrix(allPaths3);
            Assert.IsNull(adjMatrix);
        }

        [TestMethod]
        public void RoutePlanner_FindAllTrainRoutes()
        {
            int[,] routeMatrix = new int[5, 5];
            routeMatrix[0, 1] = 3;
            routeMatrix[1, 2] = 40;
            routeMatrix[2, 3] = 8;
            routeMatrix[3, 2] = 8;
            routeMatrix[2, 0] = 5;
            routeMatrix[0, 3] = 20;
            List<TrainRoute> allRoutes;

            //unit testing of distanceLessThan()
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, distLessThan: 10);
            Assert.AreEqual(0, allRoutes.Count);
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, distLessThan: 45);
            Assert.AreEqual(1, allRoutes.Count);
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, distLessThan: 51);
            Assert.AreEqual(1, allRoutes.Count);
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, distLessThan: 60);
            Assert.AreEqual(2, allRoutes.Count);

            //unit testing of stopsEqualTo()
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, stopsEqTo: 1);
            Assert.AreEqual(1, allRoutes.Count);
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, stopsEqTo: 2);
            Assert.AreEqual(0, allRoutes.Count);
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, stopsEqTo: 3);
            Assert.AreEqual(2, allRoutes.Count);
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, stopsEqTo: 4);
            Assert.AreEqual(2, allRoutes.Count);

            //unit tests for stopsLessThanEqualTo()
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, stopsLessEqTo: 1);
            Assert.AreEqual(1, allRoutes.Count);
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, stopsLessEqTo: 2);
            Assert.AreEqual(1, allRoutes.Count);
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, stopsLessEqTo: 3);
            Assert.AreEqual(3, allRoutes.Count);
            allRoutes = RoutePlanner.findAllTrainRoutes(routeMatrix, 0, 3, stopsLessEqTo: 4);
            Assert.AreEqual(5, allRoutes.Count);
        }

        [TestMethod]
        public void RoutePlanner_FindAllTrainDists()
        {
            int[,] routeMatrix = new int[5, 5];
            routeMatrix[0, 1] = 3;
            routeMatrix[1, 2] = 40;
            routeMatrix[2, 3] = 8;
            routeMatrix[3, 2] = 8;
            routeMatrix[2, 0] = 5;
            routeMatrix[0, 3] = 20;
            string distance;

            //unit tests for findDistGivenExactPath()
            int[] exactRouteA = {0, 1, 2, 3};
            distance = RoutePlanner.findAllTrainDists(routeMatrix, exactPath: exactRouteA);
            Assert.AreEqual(51, Int32.Parse(distance));

            int[] exactRouteB = { 0, 1, 2, 0 };
            distance = RoutePlanner.findAllTrainDists(routeMatrix, exactPath: exactRouteB);
            Assert.AreEqual(48, Int32.Parse(distance));

            //unit tests for findDistGivenStartEnd()
            int[] startAndEndTownA = {2, 0};
            distance = RoutePlanner.findDistGivenStartEnd(routeMatrix, startAndEndTownA);
            Assert.AreEqual(5, Int32.Parse(distance));

            int[] startAndEndTownB = { 1, 0 };
            distance = RoutePlanner.findDistGivenStartEnd(routeMatrix, startAndEndTownB);
            Assert.AreEqual(45, Int32.Parse(distance));

            int[] startAndEndTownC = { 2, 3 };
            distance = RoutePlanner.findDistGivenStartEnd(routeMatrix, startAndEndTownC);
            Assert.AreEqual(8, Int32.Parse(distance));
        }

        [TestMethod]
        public void RoutePlanner_ParseUserInput() //string userInput, ref List<char> inputTowns, ref int numberUserPasses)
        {
            string userInput = "A-C x -6"; //find all routes between A and C with exactly 6 stops
            List<char> inputTowns = new List<char>();
            int numberUserPasses = 0;
            char desiredOutput = RoutePlanner.parseUserInput(userInput, ref inputTowns, ref numberUserPasses);
            Assert.AreEqual('x', desiredOutput);
            Assert.AreEqual('A', inputTowns[0]);
            Assert.AreEqual('C', inputTowns[1]);
            Assert.AreEqual(6, numberUserPasses); //should ignore negative

            userInput = "B-E m 15"; //find all routes between B and E with 15 or fewer stops
            numberUserPasses = 0;
            inputTowns.Clear();
            desiredOutput = RoutePlanner.parseUserInput(userInput, ref inputTowns, ref numberUserPasses);
            Assert.AreEqual('m', desiredOutput);
            Assert.AreEqual('B', inputTowns[0]);
            Assert.AreEqual('E', inputTowns[1]);
            Assert.AreEqual(15, numberUserPasses);

            userInput = "B-D l 300"; //find all routes between B and D where distance is less than 300
            numberUserPasses = 0;
            inputTowns.Clear();
            desiredOutput = RoutePlanner.parseUserInput(userInput, ref inputTowns, ref numberUserPasses);
            Assert.AreEqual('l', desiredOutput);
            Assert.AreEqual('B', inputTowns[0]);
            Assert.AreEqual('D', inputTowns[1]);
            Assert.AreEqual(300, numberUserPasses);

            userInput = "E-A s 7"; //
            numberUserPasses = 0;
            inputTowns.Clear();
            desiredOutput = RoutePlanner.parseUserInput(userInput, ref inputTowns, ref numberUserPasses);
            Assert.AreEqual('s', desiredOutput);
            Assert.AreEqual('E', inputTowns[0]);
            Assert.AreEqual('A', inputTowns[1]);
            Assert.AreEqual(7, numberUserPasses);

            userInput = "B-E-A-D p"; //
            numberUserPasses = 0;
            inputTowns.Clear();
            desiredOutput = RoutePlanner.parseUserInput(userInput, ref inputTowns, ref numberUserPasses);
            Assert.AreEqual('p', desiredOutput);
            Assert.AreEqual('B', inputTowns[0]);
            Assert.AreEqual('E', inputTowns[1]);
            Assert.AreEqual('A', inputTowns[2]);
            Assert.AreEqual('D', inputTowns[3]);
            Assert.AreEqual(0, numberUserPasses);
        }

        [TestMethod]
        public void TrainRoute_ToString()
        {
            List<char> allStops = new List<char>();
            allStops.Add('A');
            allStops.Add('B');
            allStops.Add('E');
            allStops.Add('B');
            int distance = 27;
            char start = 'A';
            char end = 'B';

            TrainRoute routeA = new TrainRoute(allStops, distance, start, end);
            Console.WriteLine(routeA.toString());
            string expected = "starting town: " + start + " | ending town: " + end + " | all stops on route: "; 
            for (int i = 0; i < allStops.Count; i++)
            {
                expected += allStops[i];
            }
            expected += " | total distance: " + distance;
            Assert.AreEqual(expected, routeA.toString(), "Actual output from toString does not match expected output.");

            TrainRoute routeB = new TrainRoute(allStops, distance);
            Console.WriteLine(routeB.toString());
            expected = "starting town: " + start + " | ending town: " + end + " | all stops on route: ";
            for (int i = 0; i < allStops.Count; i++)
            {
                expected += allStops[i];
            }
            expected += " | total distance: " + distance;
            Assert.AreEqual(expected, routeB.toString(), "Actual output from toString does not match expected output.");
        }

        [TestMethod]
        public void TrainRoute_Equals()
        {
            List<char> stops = new List<char>();
            stops.Add('A');
            stops.Add('B');
            stops.Add('C');
            int dist = 23;
            TrainRoute first = new TrainRoute(stops, dist);
            TrainRoute second = new TrainRoute(stops, dist);
            Assert.IsTrue(first.Equals(second));

            List<char> diffStops = new List<char>();
            diffStops.Add('A');
            diffStops.Add('C');
            diffStops.Add('B');
            TrainRoute third = new TrainRoute(diffStops, dist);
            Assert.IsFalse(first.Equals(third));

            int diffDist = 25;
            TrainRoute fourth = new TrainRoute(diffStops, diffDist);
            Assert.IsFalse(first.Equals(fourth));
        }
    }
    
}

